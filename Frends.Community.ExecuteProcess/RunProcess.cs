using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Frends.Community.ExecuteProcess
{
    class RunProcess
    {
        public static RunProcessResult RunProcessSync(RunProcessParameters input,RunProcessOptions options)
        {
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = string.Join(" ", input.Arguments.Select(x => x.Name + " " + x.Value).ToArray());
                process.StartInfo.FileName = input.FileName;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = options.RedirectStandardInput;

                var stdoutSb = new StringBuilder();
                var stderrSb = new StringBuilder();

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
                                stdoutSb.AppendLine(e.Data);
                            }
                        }
                        catch (Exception exception)
                        {
                            Trace.TraceError($"Error while executing process {input.FileName} and handling output: {exception.ToString()}");
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
                                stderrSb.AppendLine(e.Data);
                            }
                        }
                        catch (Exception exception)
                        {
                            Trace.TraceError($"Error while executing process {input.FileName} and handling error output: {exception.ToString()}");
                        }
                    }

                    try
                    {
                        process.OutputDataReceived += ProcessOnOutputDataReceived;
                        process.ErrorDataReceived += ProcessOnErrorDataReceived;

                        // Start the process. All event handlers etc. are now registered
                        process.Start();
                        
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        // convert timeout seconds to milliseconds
                        var timeoutMS = options.TimeoutSeconds * 1000;

                        // Also wait events to be done
                        if (process.WaitForExit(timeoutMS) && outputWaitHandle.WaitOne(timeoutMS) && errorWaitHandle.WaitOne(timeoutMS))
                        {
                            if (process.HasExited)
                            {
                                // Existed - return object
                                return new RunProcessResult()
                                {
                                    ExitCode = process.ExitCode,
                                    Output = stdoutSb.ToString(),
                                    StdErr = stderrSb.ToString()
                                };
                            }
                            else
                            {
                                // Timeout & process is runnig
                                if (options.KillProcessAfterTimeout)
                                {
                                    process.Kill();
                                }

                                throw new TimeoutException($"External process <{process.Id}> execution timed out after {options.TimeoutSeconds} seconds. (1)");
                            }
                        }
                        else
                        {
                            // Timeout & process is runnimg
                            if (process.HasExited == false && options.KillProcessAfterTimeout == true)
                            {
                                process.Kill();
                            }
                            throw new TimeoutException($"External process <{process.Id}> execution timed out after {options.TimeoutSeconds} seconds. (2)");
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
