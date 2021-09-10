using System;

namespace ApplicationExceptions
{
    public class EntityRelationAlreadyExistsException : Exception
    {
        public EntityRelationAlreadyExistsException(string message) : base(message)
        {

        }
    }
}
