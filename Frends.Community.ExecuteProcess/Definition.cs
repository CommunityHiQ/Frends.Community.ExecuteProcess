#pragma warning disable 1591

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.ExecuteProcess
{
    public class Argument
    {
        /// <summary>
        /// Argument name. Use /C to provide command to cmd.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Argument value. When using cmd and /C as argument name, write here actual command being executed.
        /// </summary>
        public string Value { get; set; }
    }

    public class Input
    {
        /// <summary>
        /// Path to script or program being executed, use cmd to execute command on "command line".
        /// </summary>
        [DefaultValue("cmd")]
        [DisplayFormat(DataFormatString = "Text")]
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
        /// Timeout in full seconds
        /// </summary>
        [DefaultValue(10)]
        [UIHint(nameof(WaitForResponse), "", true)]
        public int TimeoutSeconds { get; set; }
    }

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


    public class RunProcessParameters
    {
        /// <summary>
        /// An application or document with which to start a process. Use cmd.exe to execute command on "command line".
        /// </summary>
        [DefaultValue("cmd.exe")]
        [DisplayFormat(DataFormatString = "Text")]
        public string FileName { get; set; }
        /// <summary>
        /// Command-line arguments to use when starting the application.
        /// </summary>
        public Argument[] Arguments { get; set; }
       
    }

    public class RunProcessOptions
    {
        /// <summary>
        /// Timeout in full seconds
        /// </summary>
        [DefaultValue(30)]
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// Kill the process after timeout
        /// </summary>
        [DefaultValue(false)]
        public bool KillProcessAfterTimeout { get; set; }

        /// <summary>
        /// true if input should be read from StandardInput; otherwise, false
        /// </summary>
        [DefaultValue(true)]
        public bool RedirectStandardInput { get; set; }

        /// <summary>
        /// true if the task should throw exception when return code is not 0.
        /// </summary>
        [DefaultValue(false)]
        public bool ThrowExceptionOnErrorResponse { get; set; }
    }

    public class RunProcessResult
    {
       
        /// <summary>
        /// The status that the process returned when it exited.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// The process normal output (STDOUT)
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// /// The process error output (STDERR)
        /// </summary>
        public string StdErr { get; set; }
    }
}
