using System;

namespace BindingsRx.Exceptions
{
    public class SetterNotProvidedException : Exception
    {
        public SetterNotProvidedException() : base("Setter has not been provided for binding")
        {}
    }
}