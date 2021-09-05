using System;

namespace ApplicationExceptions
{
    public class ExerciseTypeRelationAlreadyExistsException : Exception
    {
        public ExerciseTypeRelationAlreadyExistsException(string message) : base(message)
        {

        }
    }
}
