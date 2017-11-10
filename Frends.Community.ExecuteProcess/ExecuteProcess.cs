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
        /// Argument
        /// </summary>
        public class Argument
        {
            /// <summary>
            /// Argument name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Argument value
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Input
        {
            /// <summary>
            /// Script path
            /// </summary>
            public string ScriptPath { get; set; }
            /// <summary>
            /// Arguments used
            /// </summary>
            public Argument[] Arguments { get; set; }
            /// <summary>
            /// Wait for response
            /// </summary>
            public bool WaitForResponse { get; set; }
            /// <summary>
            /// Timeout in milliseconds
            /// </summary>
            public int TimeoutMS { get; set; }
        }

        /// <summary>
        /// Return object
        /// </summary>
        public class Output
        {
            /// <summary>
            /// Request result
            /// </summary>
            public string Result { get; set; }
            /// <summary>
            /// Execute status
            /// </summary>
            public bool Status { get; set; }
        }
        
        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Output ExecuteProcess(Input input)
        {
            return input.WaitForResponse ? ExecuteProcessWithResult(input) : ExecuteProcessWithoutResult(input);
        }

        /// <summary>
        /// Execute process and wait for result
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="TimeoutException"></exception>
        private static Output ExecuteProcessWithResult(Input input)
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

        /// <summary>
        /// Execute process asynchronously
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static Output ExecuteProcessWithoutResult(Input input)
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

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                return new Output { Status = process.Start() };
            }
        }
    }
}