using System;
using System.Collections.Generic;

namespace MyCoreFramework.UI.Inputs
{
    public class StaticLocalizableComboboxItemSource : ILocalizableComboboxItemSource
    {
        public ICollection<ILocalizableComboboxItem> Items { get; private set; }

        public StaticLocalizableComboboxItemSource(params ILocalizableComboboxItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Length <= 0)
            {
                throw new ArgumentException("Items can not be empty!");
            }

            this.Items = items;
        }
    }
}