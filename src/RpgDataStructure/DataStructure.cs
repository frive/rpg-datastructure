using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rpg
{
    public abstract class DataStructure
    {
        private Dictionary<string, int> _negativeCharacterList = new Dictionary<string, int>
        {
            { "@", 0 },
            { "A", 1 },
            { "B", 2 },
            { "C", 3 },
            { "D", 4 },
            { "E", 5 },
            { "F", 6 },
            { "G", 7 },
            { "H", 8 },
            { "I", 9 }
        };

        #region API

        public void Clear()
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                propertyInfo.SetValue(this, null);
            }
        }

        public string Dump()
        {
            string output = "";

            using (var sw = new StringWriter())
            {
                var line = SortedProperties()
                    .Aggregate("", SerializeProperties);

                sw.Write(line);
                output = sw.ToString();
            }

            return output;
        }

        public void Load(string source)
        {
            var properties = SortedProperties();
            int dsLength = 0;

            foreach (PropertyInfo prop in properties)
            {
                var propLength = GetPropLength(prop);

                dsLength += propLength;
            }

            if (source.Length < dsLength)
            {
                throw new DataStructureException("Source cannot be less than data structure length");
            }

            foreach (PropertyInfo prop in properties)
            {
                int propLength = GetPropLength(prop);

                string strValue = source.Substring(0, propLength);

                var decimalProp = prop.GetCustomAttribute<FixedDecimalTypeAttribute>();
                if (decimalProp != null)
                {
                    // Check if the current field is not an empty string before parsing to decimal.
                    if (string.IsNullOrWhiteSpace(strValue))
                    {
                        strValue = "0";
                    }
                    else
                    {
                        strValue = strValue.Insert(decimalProp.Precision - decimalProp.Scale, ".");
                    }

                    strValue = DecodeNegativeValue(strValue);
                    prop.SetValue(this, decimal.Parse(strValue, CultureInfo.InvariantCulture));
                }
                else
                {
                    prop.SetValue(this, strValue);
                }

                // Update Source
                source = source.Substring(propLength);
            }
        }

        #endregion API

        private List<PropertyInfo> SortedProperties()
        {
            return GetType().GetProperties()
                .Where(p =>
                {
                    var charTypeAttribute = p.GetCustomAttribute<CharTypeAttribute>();
                    var decimalTypeAttribute = p.GetCustomAttribute<FixedDecimalTypeAttribute>();

                    return (charTypeAttribute != null || decimalTypeAttribute != null);
                })
                .OrderBy(p =>
                {
                    var indexAttribute = p.GetCustomAttribute<IndexAttribute>();

                    if (indexAttribute != null) return indexAttribute.Index;
                    return int.MaxValue;
                })
                .ToList();
        }

        private int GetPropLength(PropertyInfo prop)
        {
            var charTypeAttribute = prop.GetCustomAttribute<CharTypeAttribute>();
            var decimalTypeAttribute = prop.GetCustomAttribute<FixedDecimalTypeAttribute>();

            if (charTypeAttribute != null)
            {
                return charTypeAttribute.Length;
            }

            if (decimalTypeAttribute != null)
            {
                return decimalTypeAttribute.Precision;
            }

            throw new Exception("Invalid length");
        }

        private string SerializeProperties(string acc, PropertyInfo prop)
        {
            // Maximum length allowed for the property value.
            // Anything that exceeds the length will be discarded.
            int valueLength = 0;

            var charTypeAttribute = prop.GetCustomAttribute<CharTypeAttribute>();
            var decimalTypeAttribute = prop.GetCustomAttribute<FixedDecimalTypeAttribute>();
            object propertyValue = prop.GetValue(this);
            string paddedPropertyValue = "";
            string paddingFormat = null;
            CultureInfo culture = CultureInfo.InvariantCulture;

            if (charTypeAttribute != null)
            {
                valueLength = charTypeAttribute.Length;

                // If property is defined and not initialized, set to empty string
                if (propertyValue == null) propertyValue = "";

                if (propertyValue.ToString().Length > valueLength)
                {
                    // Truncate the string to the length given similar to what AVR does
                    propertyValue = propertyValue.ToString().Substring(0, valueLength);
                }

                paddedPropertyValue = propertyValue.ToString().PadRight(valueLength);
            }

            if (decimalTypeAttribute != null)
            {
                if (decimalTypeAttribute.Scale > 0)
                {
                    valueLength = decimalTypeAttribute.Precision - decimalTypeAttribute.Scale;

                    paddingFormat = new string('0', valueLength);

                    string paddingFractional = new string('0', decimalTypeAttribute.Scale);
                    paddingFormat += $".{paddingFractional}";

                    paddedPropertyValue = Convert.ToDecimal(propertyValue, culture).ToString(paddingFormat, culture);
                    paddedPropertyValue = paddedPropertyValue.Replace(".", "");
                    paddedPropertyValue = EncodeNegativeValue(paddedPropertyValue);
                }
                else
                {
                    valueLength = decimalTypeAttribute.Precision;
                    paddingFormat = new string('0', valueLength);
                    paddedPropertyValue = Convert.ToDecimal(propertyValue, culture).ToString(paddingFormat, culture);
                }

                if (paddedPropertyValue.Replace(".", "").Length > decimalTypeAttribute.Precision)
                {
                    throw new DataStructureException($"Length of property `{prop.Name}` value exceeds length or precision attribute");
                }
            }

            return acc + paddedPropertyValue;
        }

        /// <summary>
        /// Encodes last character of negative decimal value based from AVR's implementation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string EncodeNegativeValue(string value)
        {
            decimal decimalValue = Convert.ToDecimal(value);
            string lastCharacter = "";
            string encodedValue = "";

            if (decimalValue >= 0) return value;

            value = value.Replace("-", "");

            lastCharacter = value.Substring(value.Length - 1);
            encodedValue = _negativeCharacterList
                .First(c => c.Value == Convert.ToInt32(lastCharacter))
                .Key;
            value = value.Substring(0, value.Length - 1) + encodedValue;

            return value;
        }

        /// <summary>
        /// Decode last character of a negative decimal value based from AVR's implementation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string DecodeNegativeValue(string value)
        {
            string lastCharacter = value.Substring(value.Length - 1);
            string decodedValue = "";

            if (!_negativeCharacterList.ContainsKey(lastCharacter)) return value;

            decodedValue = _negativeCharacterList[lastCharacter].ToString();
            value = "-" + value.Substring(0, value.Length - 1) + decodedValue;

            return value;
        }
    }
}
