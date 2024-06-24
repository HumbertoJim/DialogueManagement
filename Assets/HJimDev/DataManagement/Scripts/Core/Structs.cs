using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataManagement
{
    namespace Structs
    {
        public struct CoreAttribute<T>
        {
            private T value;
            private bool isFixed;

            public T Value
            {
                get
                {
                    if (isFixed) return value;
                    else throw new Exceptions.NullAttributeException();
                }
                set
                {
                    if (!isFixed)
                    {
                        this.value = value;
                        isFixed = true;
                    }
                    else throw new Exceptions.MultipleSetAttributeException();
                }
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        [Serializable]
        public struct Document
        {
            public string name;
            public TextAsset file;

            public override string ToString()
            {
                return name;
            }
        }
    }
}