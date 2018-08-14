using System;

namespace Bebop.Monads
{
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
            if (InternalType.IsValueType)
                return Activator.CreateInstance(InternalType);

            return null;
        }

        #endregion

        #region Equaliy

        public override bool Equals(object obj)
        {
            return Equals(obj as IMaybe);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public bool Equals(IMaybe other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (HasValue)
                return other.HasValue && _value.Equals(other.GetValueOrDefault());

            return !other.HasValue;
        }

        #endregion
    }
}
