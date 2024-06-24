using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataManagement
{
    namespace Exceptions
    {
        public class NullAttributeException : Exception
        {
            public NullAttributeException() : base("Attribute was not fixed.") { }
            public NullAttributeException(string name) : base("Attribute " + name + " was not fixed.") { }
        }

        public class MultipleSetAttributeException : Exception
        {
            public MultipleSetAttributeException() : base("Trying to set Attribute more than once.") { }
            public MultipleSetAttributeException(string name) : base("Trying to set the Attribute " + name + " more than once.") { }
        }
    }
}