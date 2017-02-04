using System;

using MyCoreFramework.Extensions;

namespace MyCoreFramework.Runtime.Validation
{
    [Serializable]
    [Validator("NUMERIC")]
    public class NumericValueValidator : ValueValidatorBase
    {
        public int MinValue
        {
            get { return (this["MinValue"] ?? "0").To<int>(); }
            set { this["MinValue"] = value; }
        }

        public int MaxValue
        {
            get { return (this["MaxValue"] ?? "0").To<int>(); }
            set { this["MaxValue"] = value; }
        }

        public NumericValueValidator()
        {

        }

        public NumericValueValidator(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is int)
            {
                return this.IsValidInternal((int)value);
            }

            if (value is string)
            {
                int intValue;
                if (int.TryParse(value as string, out intValue))
                {
                    return this.IsValidInternal(intValue);
                }
            }

            return false;
        }

        protected virtual bool IsValidInternal(int value)
        {
            return value.IsBetween(this.MinValue, this.MaxValue);
        }
    }
}