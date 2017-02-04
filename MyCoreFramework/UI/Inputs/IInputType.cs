using System.Collections.Generic;

using MyCoreFramework.Runtime.Validation;

namespace MyCoreFramework.UI.Inputs
{
    public interface IInputType
    {
        string Name { get; }

        object this[string key] { get; set; }

        IDictionary<string, object> Attributes { get; }

        IValueValidator Validator { get; set; }
    }
}