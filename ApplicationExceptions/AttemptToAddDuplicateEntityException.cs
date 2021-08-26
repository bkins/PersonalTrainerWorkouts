using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationExceptions
{
    [Serializable]
    public class AttemptToAddDuplicateEntityException : Exception
    {
        public string TypeDuplicated       { get; set; }
        public string NameOfTypeDuplicated { get; set; }
        public object ObjectDuplicated     { get; set; }
        public string FieldDuplicated      { get; set; }

        public AttemptToAddDuplicateEntityException(string nameOfTypeDuplicated
                                                  , object objectDuplicated
                                                  , string fieldDuplicated) 

                : this($"Duplicate records cannot be added to {nameOfTypeDuplicated}"
                                                             , nameOfTypeDuplicated
                                                             , nameOfTypeDuplicated
                                                             , objectDuplicated
                                                             , fieldDuplicated)
        { }

        public AttemptToAddDuplicateEntityException(string message
                                                  , string typeDuplicated
                                                  , string nameOfTypeDuplicated
                                                  , object objectDuplicated
                                                  , string fieldDuplicated) : base(message)
        {
            TypeDuplicated       = typeDuplicated;
            NameOfTypeDuplicated = nameOfTypeDuplicated;
            ObjectDuplicated     = objectDuplicated;
            FieldDuplicated      = fieldDuplicated;
        }
    }
}
