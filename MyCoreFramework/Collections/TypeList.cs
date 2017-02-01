using System;
using System.Collections;
using System.Collections.Generic;

namespace MyCoreFramework.Collections
{
    /// <summary>
    /// A shortcut for <see cref="TypeList{TBaseType}"/> to use object as base type.
    /// </summary>
    public class TypeList : TypeList<object>, ITypeList
    {
    }

    /// <summary>
    /// Extends <see cref="List{Type}"/> to add restriction a specific base type.
    /// </summary>
    /// <typeparam name="TBaseType">Base Type of <see cref="Type"/>s in this list</typeparam>
    public class TypeList<TBaseType> : ITypeList<TBaseType>
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => this.typeList.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => false;

        public Type this[int index]
        {
            get { return this.typeList[index]; }
            set
            {
                CheckType(value);
                this.typeList[index] = value;
            }
        }

        private readonly List<Type> typeList;

        public TypeList()
        {
            this.typeList = new List<Type>();
        }

        public void Add<T>() where T : TBaseType
        {
            this.typeList.Add(typeof(T));
        }

        public void Add(Type item)
        {
            CheckType(item);
            this.typeList.Add(item);
        }

        public void Insert(int index, Type item)
        {
            this.typeList.Insert(index, item);
        }

        public int IndexOf(Type item)
        {
            return this.typeList.IndexOf(item);
        }

        public bool Contains<T>() where T : TBaseType
        {
            return this.Contains(typeof(T));
        }

        public bool Contains(Type item)
        {
            return this.typeList.Contains(item);
        }

        public void Remove<T>() where T : TBaseType
        {
            this.typeList.Remove(typeof(T));
        }

        public bool Remove(Type item)
        {
            return this.typeList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.typeList.RemoveAt(index);
        }

        public void Clear()
        {
            this.typeList.Clear();
        }

        /// <inheritdoc/>
        public void CopyTo(Type[] array, int arrayIndex)
        {
            this.typeList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return this.typeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.typeList.GetEnumerator();
        }

        private static void CheckType(Type item)
        {
            if (!typeof(TBaseType).IsAssignableFrom(item))
            {
                throw new ArgumentException("Given item is not type of " + typeof(TBaseType).AssemblyQualifiedName, "item");
            }
        }
    }
}
