using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GameProject.CoreEngine
{
    public static class CoreUtils
    {
        public static void CopyPropertiesFrom<T>(this T destination, T source)
        {
            PropCopier<T>.Cloner(destination, source);
            // foreach (var property in PropCopier<T>.Properties)
            //     property.SetValue(destination, property.GetValue(source));
        }

        public static void InvokeAll(this IEnumerable<Action> delegates)
        {
            foreach (var action in delegates)
                action();
        }

        public static int ToLinearIndex(this Point point, int columns)
        {
            return point.X + point.Y * columns;
        }

        /// <summary>
        ///     Used for optimization of property getting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private abstract class PropCopier<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static readonly PropertyInfo[] Properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            public static readonly Action<T, T> Cloner;

            static PropCopier()
            {
                var destination = Expression.Parameter(typeof(T), "destination");
                var source = Expression.Parameter(typeof(T), "source");
                var expressions = Properties
                    .Select(property => Expression
                        .Call(destination, property.SetMethod, Expression
                            .Call(source, property.GetMethod)))
                    .Cast<Expression>();

                var block = Expression.Block(expressions);
                Cloner = Expression.Lambda<Action<T, T>>(block, destination, source).Compile();
            }
        }
    }
}