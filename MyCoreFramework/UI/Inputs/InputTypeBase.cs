using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Runtime.Validation;

namespace MyCoreFramework.UI.Inputs
{
    [Serializable]
    public abstract class InputTypeBase : IInputType
    {
        public virtual string Name
        {
            get
            {
                var type = this.GetType();
                if (type.IsDefined(typeof(InputTypeAttribute)))
                {
                    return type.GetCustomAttributes(typeof(InputTypeAttribute)).Cast<InputTypeAttribute>().First().Name;
                }

                return type.Name;
            }
        }

        /// <summary>
        /// Gets/sets arbitrary objects related to this object.
        /// Gets null if given key does not exists.
        /// </summary>
        /// <param name="key">Key</param>
        public object this[string key]
        {
            get { return this.Attributes.GetOrDefault(key); }
            set { this.Attributes[key] = value; }
        }

        /// <summary>
        /// Arbitrary objects related to this object.
        /// </summary>
        public IDictionary<string, object> Attributes { get; private set; }

        public IValueValidator Validator { get; set; }

        protected InputTypeBase()
            :this(new AlwaysValidValueValidator())
        {

        }

        protected InputTypeBase(IValueValidator validator)
        {
            this.Attributes = new Dictionary<string, object>();
            this.Validator = validator;
        }
    }
}