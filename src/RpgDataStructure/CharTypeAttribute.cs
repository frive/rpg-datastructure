using System;

namespace Rpg
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CharTypeAttribute: Attribute
    {
        public int Length { get; set; }

        public CharTypeAttribute(int length)
        {
            Length = length;
        }
    }
}
