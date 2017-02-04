using System;

using MyCoreFramework.Runtime.Validation;

namespace MyCoreFramework.UI.Inputs
{
    /// <summary>
    /// Combobox value UI type.
    /// </summary>
    [Serializable]
    [InputType("COMBOBOX")]
    public class ComboboxInputType : InputTypeBase
    {
        public ILocalizableComboboxItemSource ItemSource { get; set; }

        public ComboboxInputType()
        {

        }

        public ComboboxInputType(ILocalizableComboboxItemSource itemSource)
        {
            this.ItemSource = itemSource;
        }

        public ComboboxInputType(ILocalizableComboboxItemSource itemSource, IValueValidator validator)
            : base(validator)
        {
            this.ItemSource = itemSource;
        }
    }
}