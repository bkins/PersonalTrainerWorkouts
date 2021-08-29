using System;

namespace ApplicationExceptions
{
    public class UnnamedEntityException : Exception
    {
        public UnnamedEntityException(string message) : base(message)
        {

        }
    }
}
