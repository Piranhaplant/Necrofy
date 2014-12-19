using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>Log is a simple class for managing a list of log messages</summary>
    class Log
    {
        // If necessary, this will be changed to a more robust structure that will store the level of the message as a separate value.
        private List<string> messages;

        /// <summary>Creates a new empty Log</summary>
        public Log() {
            messages = new List<string>();
        }

        /// <summary>Records an informational message in the log.</summary>
        /// <param name="message">The message</param>
        public void LogInfo(string message) {
            messages.Add("INFO: " + message);
        }

        /// <summary>Records a warning message in the log.</summary>
        /// <param name="message">The message</param>
        public void LogWarn(string message) {
            messages.Add("WARN: " + message);
        }

        /// <summary>Records an error message in the log.</summary>
        /// <param name="message">The message</param>
        public void LogError(string message) {
            messages.Add("ERROR: " + message);
        }

        /// <summary>Gets a value indicating whether the log has no records in it.</summary>
        public bool Empty {
            get { return messages.Count == 0; }
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            foreach (string s in messages) {
                builder.AppendLine(s);
            }
            return builder.ToString();
        }
    }
}
