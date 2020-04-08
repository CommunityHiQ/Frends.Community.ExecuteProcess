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

    /// <summary>
    /// Parameters class usually requires parameters that are required.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Something that will be repeated.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Message;
    }

    /// <summary>
    /// Options class provides additional parameters.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Number of times input is echoed.
        /// </summary>
        public int Amount;

        /// <summary>
        /// How repeats of input are separated.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Delimiter;
    }

    public class Result
    {
        /// <summary>
        /// Contains input repeated specified times.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Replication;
    }
}
