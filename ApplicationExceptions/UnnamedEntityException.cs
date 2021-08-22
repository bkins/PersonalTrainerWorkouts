using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationExceptions
{
    public class UnnamedEntityException : Exception
    {
        public UnnamedEntityException(string message) : base(message)
        {

        }
    }
}
