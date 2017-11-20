using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Frends.Community.ExecuteProcess
{
    /// <summary>
    /// Execute process
    /// </summary>
    public class ExecuteProcessCommand
    {
        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Output ExecuteProcess(Input input)
        {
            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                Arguments = string.Join(" ", input.Arguments.Select(x => x.Name + " " + x.Value).ToArray()),
                FileName = input.ScriptPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            if(input.WaitForResponse)
                return ExecuteProcessWithResult(input, processStartInfo);
            else
                using (var process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    return new Output { Status = process.Start() };
                }
        }

        /// <summary>
        /// Execute process and wait for result
        /// </summary>
        /// <param name="input"></param>
        /// <param name="processStartInfo"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="TimeoutException"></exception>
        private static Output ExecuteProcessWithResult(Input input, ProcessStartInfo processStartInfo)
        {
            using (var process = Process.Start(processStartInfo))
            {
                var output = new StringBuilder();
                var error = new StringBuilder();

                using (var outputWaitHandle = new AutoResetEvent(false))
                using (var errorWaitHandle = new AutoResetEvent(false))
                {
                    void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
                    {
                        try
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        }
                        catch (Exception exception)
                        {
                            Trace.TraceError($"Error while executing process {input.ScriptPath} and handling output: {exception}");
                        }
                    }

                    void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
                    {
                        try
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                error.AppendLine(e.Data);
                            }
                        }
                        catch (Exception exception)
                        {
                            Trace.TraceError($"Error while executing process {input.ScriptPath} and handling error output: {exception}");
                        }
                    }

                    try
                    {
                        process.OutputDataReceived += ProcessOnOutputDataReceived;
                        process.ErrorDataReceived += ProcessOnErrorDataReceived;

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(input.TimeoutMS) && outputWaitHandle.WaitOne(input.TimeoutMS) && errorWaitHandle.WaitOne(input.TimeoutMS))
                        {
                            if (process.ExitCode > 0)
                            {
                                throw new ApplicationException("External process execution failed with returncode: " + process.ExitCode + " and output: " + Environment.NewLine + error);
                            }
                            else
                            {
                                return new Output { Result = "External process execution was successful with output: " + Environment.NewLine + output, Status = true };
                            }
                        }
                        else
                        {
                            throw new TimeoutException("External process execution timed out after " + input.TimeoutMS + " milliseconds.");
                        }
                    }
                    finally
                    {
                        process.OutputDataReceived -= ProcessOnOutputDataReceived;
                        process.ErrorDataReceived -= ProcessOnErrorDataReceived;
                    }
                }
            }
        }
    }
}