// Copyright 2019, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads
{
    /// <summary>
    /// A unit type.
    /// </summary>
    public readonly struct Unit : IEquatable<Unit>, IUnit
    {
        /// <summary>
        /// Provides a shared, boxed instance of <see cref="Unit"/> for
        /// situations where the struct would not be stack allocated.
        /// </summary>
        public static readonly IUnit Instance = new Unit();

        /// <inheritdoc />
        public bool Equals(Unit other)
        {
            return true;
        }

        /// <inheritdoc />
        public bool Equals(IUnit other)
        {
            return !ReferenceEquals(null, other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 37;
        }
    }
}
