using System;
using System.Runtime.CompilerServices;

namespace Rpg
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IndexAttribute : Attribute
    {
        public int Index { get; }

        public IndexAttribute([CallerLineNumber]int index = 0)
        {
            Index = index;
        }
    }
}