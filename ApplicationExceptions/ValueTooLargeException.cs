using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationExceptions
{
    [Serializable]
    public class ValueTooLargeException : Exception
    {
        public string NameOfFieldThatHasTooLargeOfNumber  { get; set; }
        public string ValueOfFieldThatHasTooLargeOfNumber { get; set; }
        public string TypeOfFieldThatHasTooLargeOfNumber  { get; set; }
        public object FieldThatHasTooLargeOfNumber        { get; set; }
        public string WhyIsTheValueTooLarge               { get; set; }

        public ValueTooLargeException(string nameOfFieldThatHasTooLargeOfNumber
                                    , string valueOfFieldThatHasTooLargeOfNumber
                                    , string typeOfFieldThatHasTooLargeOfNumber
                                    , object fieldThatHasTooLargeOfNumber
                                    , string whyIsTheValueTooLarge) 
                : this($"The value ({valueOfFieldThatHasTooLargeOfNumber}) in the field ({nameOfFieldThatHasTooLargeOfNumber}) is too large because, {whyIsTheValueTooLarge}"
                     , nameOfFieldThatHasTooLargeOfNumber
                     , valueOfFieldThatHasTooLargeOfNumber
                     , typeOfFieldThatHasTooLargeOfNumber
                     , fieldThatHasTooLargeOfNumber
                     , whyIsTheValueTooLarge)
        {

        }

        public ValueTooLargeException(string message
                                    , string nameOfFieldThatHasTooLargeOfNumber
                                    , string valueOfFieldThatHasTooLargeOfNumber
                                    , string typeOfFieldThatHasTooLargeOfNumber
                                    , object fieldThatHasTooLargeOfNumber
                                    , string whyIsTheValueTooLarge) : base(message)
        {
            NameOfFieldThatHasTooLargeOfNumber  = nameOfFieldThatHasTooLargeOfNumber;
            ValueOfFieldThatHasTooLargeOfNumber = valueOfFieldThatHasTooLargeOfNumber;
            TypeOfFieldThatHasTooLargeOfNumber  = typeOfFieldThatHasTooLargeOfNumber;
            FieldThatHasTooLargeOfNumber        = fieldThatHasTooLargeOfNumber;
            WhyIsTheValueTooLarge               = whyIsTheValueTooLarge;
        }
    }
}
