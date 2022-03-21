using System;


namespace SettingsDb
{
    public class SettingsDbException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SettingsDb.SettingsDbException"/> class with its message string set
        /// to a default message.
        /// </summary>
        public SettingsDbException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SettingsDb.SettingsDbException"/> class with a specified message.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public SettingsDbException(string message)
          : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SettingsDb.SettingsDbException"/> class with a specified message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="inner">Exception that caused it.</param>
        public SettingsDbException(string message, Exception inner)
          : base(message, inner)
        {
        }
    }
}
