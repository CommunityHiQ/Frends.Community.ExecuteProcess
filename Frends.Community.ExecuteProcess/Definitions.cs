#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
}
