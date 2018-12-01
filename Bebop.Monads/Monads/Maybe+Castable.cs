// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Reflection;

namespace Bebop.Monads
{
    public static partial class Maybe
    {
        /// <summary>
        /// Provides non-generic mechanisms to create instances of <see cref="IMaybe"/> that are
        /// down-castable to <see cref="Maybe{T}"/>. This is done by using Reflection. The methods are 
        /// significantly slower than what is provided by <see cref="Maybe.From(Type, object)"/> and
        /// <see cref="Maybe.Nothing(Type)"/>.
        /// Instances created with <see cref="Maybe.From(Type, object)"/> or <see cref="Maybe.Nothing(Type)"/>
        /// are not down-castable to <see cref="Maybe{T}"/> because they return a different implementation 
        /// that only satisfies the <see cref="IMaybe"/> interface.
        /// </summary>
        public static class Castable
        {
            /// <summary>
            /// Creates an empty <see cref="Maybe{T}"/>.
            /// This instance is created via <see cref="Activator.CreateInstance(Type)"/>
            /// and is directly castable to a <see cref="Maybe{T}"/>. This is in contrast to what
            /// <see cref="Maybe.Nothing(Type)"/> produces, which only satisfies the <see cref="IMaybe"/>
            /// interface but has different implementation.
            /// This method is significantly slower than <see cref="Maybe.Nothing(Type)"/>.
            /// </summary>
            public static IMaybe Nothing(Type type)
            {
                _VerifyTypeNotNull(type);

                return (IMaybe) Activator.CreateInstance(typeof(Maybe<>).MakeGenericType(type));
            }
            
            /// <summary>
            /// Creates an <see cref="Maybe{T}"/> instance that contains the given <paramref name="value"/>.
            /// This instance is created via <see cref="Activator.CreateInstance(Type)"/>
            /// and is directly castable to a <see cref="Maybe{T}"/>. This is in contrast to what
            /// <see cref="Maybe.From(Type, Object)"/> produces, which only satisfies the <see cref="IMaybe"/>
            /// interface but has different implementation.
            /// This method is significantly slower than <see cref="Maybe.From(Type, Object)"/>.
            /// </summary>
            public static IMaybe From(Type type, object value)
            {
                _VerifyTypeNotNull(type);
                _VerifyNotNull(value);
                _VerifyValueAssignable(type, value);

                return (IMaybe) Activator.CreateInstance(
                    typeof(Maybe<>).MakeGenericType(type),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new [] {value},
                    CultureInfo.InvariantCulture);
            }
        }
    }
}