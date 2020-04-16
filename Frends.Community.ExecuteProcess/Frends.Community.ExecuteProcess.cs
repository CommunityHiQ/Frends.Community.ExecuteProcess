using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

#pragma warning disable 1591

namespace Frends.Community.ExecuteProcess
{
    /// <summary>
    /// Execute process
    /// </summary>
    public class ExecuteProcessCommand
    {
        /// <summary>
        /// Starts the process and waits results.
        /// </summary>
        /// <param name="input">Process data</param>
        /// <param name="options">Run options</param>
        /// <returns>{RunProcessResult}</returns>
        public static RunProcessResult RunProcess([PropertyTab] RunProcessParameters input, [PropertyTab] RunProcessOptions options)
        {
            return Frends.Community.ExecuteProcess.RunProcess.RunProcessSync(input, options);
        }

        /// <summary>
        /// Execute process
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.ExecuteProcess
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

            if (input.WaitForResponse)
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

                        // convert timeout seconds to milliseconds
                        var timeoutMS = input.TimeoutSeconds * 1000;

                        if (process.WaitForExit(timeoutMS) && outputWaitHandle.WaitOne(timeoutMS) && errorWaitHandle.WaitOne(timeoutMS))
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
                            throw new TimeoutException("External process execution timed out after " + input.TimeoutSeconds + " seconds.");
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
