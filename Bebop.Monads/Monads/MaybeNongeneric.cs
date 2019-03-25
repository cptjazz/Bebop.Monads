// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;
using System.Reflection;

namespace Bebop.Monads
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal sealed class MaybeNongeneric : IMaybe, IEquatable<IMaybe>
    {
        private readonly object _value;

        #region Construction

        public MaybeNongeneric(Type internalType)
        {
            InternalType = internalType; 
        }

        public MaybeNongeneric(Type internalType, object value)
        {
            InternalType = internalType;
            _value = value;
        }

        #endregion

        #region IMaybe interface

        public bool HasValue => !ReferenceEquals(_value, null);

        public Type InternalType { get; }

        public object GetValueOrDefault()
        {
            if (!ReferenceEquals(_value, null))
                return _value;

            return _CreateDefault();
        }

        private object _CreateDefault()
        {
            if (InternalType.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(InternalType);

            return null;
        }

        public object Value => _value;

        #endregion

        #region Equality

        public override bool Equals(object obj)
        {
            return Equals(obj as IMaybe);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? InternalType.GetHashCode();
        }

        public bool Equals(IMaybe other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (HasValue)
                return other.HasValue && _value.Equals(other.Value);

            return !other.HasValue;
        }

        #endregion

        #region Debugger

        internal string DebuggerDisplay => HasValue
            ? _value.ToString()
            : $"Nothing<{InternalType.Name}>";

        #endregion

        #region String representation

        /// <summary>
        /// </summary>
        public override string ToString() => HasValue
            ? _value.ToString()
            : $"Nothing<{InternalType.Name}>";

        #endregion
    }
}