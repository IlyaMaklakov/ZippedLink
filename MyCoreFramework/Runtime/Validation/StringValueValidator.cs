using System;
using System.Globalization;
using System.Text.RegularExpressions;

using MyCoreFramework.Extensions;

namespace MyCoreFramework.Runtime.Validation
{
    [Serializable]
    [Validator("STRING")]
    public class StringValueValidator : ValueValidatorBase
    {
        public bool AllowNull
        {
            get { return (this["AllowNull"] ?? "false").To<bool>(); }
            set { this["AllowNull"] = value.ToString().ToLower(CultureInfo.InvariantCulture); }
        }

        public int MinLength
        {
            get { return (this["MinLength"] ?? "0").To<int>(); }
            set { this["MinLength"] = value; }
        }

        public int MaxLength
        {
            get { return (this["MaxLength"] ?? "0").To<int>(); }
            set { this["MaxLength"] = value; }
        }

        public string RegularExpression
        {
            get { return this["RegularExpression"] as string; }
            set { this["RegularExpression"] = value; }
        }

        public StringValueValidator()
        {
            
        }

        public StringValueValidator(int minLength = 0, int maxLength = 0, string regularExpression = null, bool allowNull = false)
        {
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.RegularExpression = regularExpression;
            this.AllowNull = allowNull;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return this.AllowNull;
            }

            if (!(value is string))
            {
                return false;
            }

            var strValue = value as string;
            
            if (this.MinLength > 0 && strValue.Length < this.MinLength)
            {
                return false;
            }

            if (this.MaxLength > 0 && strValue.Length > this.MaxLength)
            {
                return false;
            }

            if (!this.RegularExpression.IsNullOrEmpty())
            {
                return Regex.IsMatch(strValue, this.RegularExpression);
            }

            return true;
        }
    }
}