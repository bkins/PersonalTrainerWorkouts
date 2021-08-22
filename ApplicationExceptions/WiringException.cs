using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationExceptions
{
    /// <summary>
    /// Exceptions that are a result of bad wiring.
    /// e.g.: Connections between classes are not setup correctly.  There is null reference when accessing a data member within a class
    /// </summary>
    [Serializable]
    public class WiringException : Exception
    {
        public string CallingClass { get; set; }
        public string CalledClass  { get; set; }

        public WiringException(string message) : base(message) { }

        public WiringException(string    message
                             , Exception inner) : base(message, inner) { }

        public WiringException(string    message
                             , string    callingClass
                             , string    calledClass
                             , Exception inner) : base(message, inner)
        {
            CallingClass = callingClass;
            CalledClass  = calledClass;
        }

        public WiringException(string message
                             , string callingClass
                             , string calledClass) : base(message)
        {
            CallingClass = callingClass;
            CalledClass  = calledClass;
        }
    }
}
