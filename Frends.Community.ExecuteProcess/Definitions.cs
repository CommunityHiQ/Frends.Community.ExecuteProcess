#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Frends.Community.ExecuteProcess
{
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
