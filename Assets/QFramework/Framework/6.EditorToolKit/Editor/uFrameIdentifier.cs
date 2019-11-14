using System;

namespace QF
{
    /// <summary>
    /// Used by the injection container to determine if a property or field should be injected.
    /// </summary>
    public class uFrameIdentifier : Attribute
    {
        public uFrameIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }

        public uFrameIdentifier()
        {
        }
    }
}