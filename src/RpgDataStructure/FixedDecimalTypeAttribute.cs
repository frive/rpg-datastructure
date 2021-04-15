using System;

namespace Rpg
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FixedDecimalTypeAttribute: Attribute
    {
        public int Precision { get; set; }
        public int Scale { get; set; }

        public FixedDecimalTypeAttribute(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
