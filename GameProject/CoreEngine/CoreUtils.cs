using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.CoreEngine
{
    internal static class CoreUtils
    {
        public static void CopyPropertiesFrom<T>(this T destination, T source)
        {
            PropCopier<T>.Cloner(destination, source);
        }

        public static void InvokeAll(this IEnumerable<Action> delegates)
        {
            foreach (var action in delegates)
                action?.Invoke();
        }

        /// <summary>
        ///     Convert an (X, Y) coordinate point into a zero-based linear array index
        /// </summary>
        /// <param name="point">The source coordinates</param>
        /// <param name="columns">Number of columns if source array</param>
        /// <returns>The calculated index</returns>
        public static int ToLinearIndex(this Point point, int columns)
        {
            return point.X + point.Y * columns;
        }

        public static CollisionInfo FindMaximumImpulseContact(PhysicsBody body)
        {
            return body.Collisions.Aggregate((0f, null as CollisionInfo?),
                (t, x) => t.Item1 < x.NormalImpulse
                    ? (x.NormalImpulse, x)
                    : t).Item2.GetValueOrDefault();
        }

        /// <summary>
        ///     Run BSF on a map if null cell is a "wall"
        /// </summary>
        /// <param name="map">Map to run BFS on</param>
        /// <param name="start">Starting point</param>
        /// <param name="visited">Set of visited points</param>
        /// <typeparam name="T">Type of map cell</typeparam>
        /// <returns>The new map with only non-wall cells connected to the starting point</returns>
        public static T[,] RunBreadthFirstSearch<T>(T[,] map, Point start, HashSet<Point> visited = null)
        {
            return RunBreadthFirstSearch(map, start, x => x == null, visited);
        }

        /// <summary>
        ///     Run BSF on a map
        /// </summary>
        /// <param name="map">Map to run BFS on</param>
        /// <param name="start">Starting point</param>
        /// <param name="predicate">Indicates if cell is a "wall"</param>
        /// <param name="visited">Set of visited points</param>
        /// <typeparam name="T">Type of map cell</typeparam>
        /// <returns>The new map with only non-wall cells connected to the starting point</returns>
        public static T[,] RunBreadthFirstSearch<T>(T[,] map, Point start, Func<T, bool> predicate,
            HashSet<Point> visited = null)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var result = new T[width, height];
            visited = visited ?? new HashSet<Point>();

            if (predicate(map[start.X, start.Y]))
            {
                visited.Add(start);
                result[start.X, start.Y] = map[start.X, start.Y];
                return result;
            }

            var queue = new Queue<Point>();
            queue.Enqueue(start);
            while (queue.Count != 0)
            {
                var point = queue.Dequeue();
                visited.Add(point);
                result[point.X, point.Y] = map[point.X, point.Y];

                for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    var p = new Point(point.X + dx, point.Y + dy);
                    if (dx != 0 && dy != 0) continue;
                    if (p.X < 0 || p.X >= width || p.Y < 0 || p.Y >= height
                        || predicate(map[p.X, p.Y]) || visited.Contains(p))
                        continue;
                    queue.Enqueue(p);
                }
            }

            return result;
        }

        /// <summary>
        ///     Get global position of a render shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static Vector2F GetGlobalPosition(this IRenderShape shape)
        {
            return shape.Offset.TransformBy(shape.Transform);
        }

        public static Vector2F GetCenter(this RectangleF rect)
        {
            return new Vector2F(rect.Left + rect.Width * 0.5f, rect.Top + rect.Height * 0.5f);
        }

        public static Vector2F GetSize(this RectangleF rect)
        {
            return new Vector2F(rect.Width, rect.Height);
        }

        /// <summary>
        ///     Used for optimization of property copying
        /// </summary>
        /// <typeparam name="T">Type of object to copy</typeparam>
        private abstract class PropCopier<T>
        {
            /// <summary>
            ///     Method that takes to objects and copies properties of right to the left
            /// </summary>
            public static readonly Action<T, T> Cloner;

            private static readonly PropertyInfo[] properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            /// <summary>
            ///     Use Linq expressions to precompile copying code for a certain type
            /// </summary>
            static PropCopier()
            {
                var destination = Expression.Parameter(typeof(T), "destination");
                var source = Expression.Parameter(typeof(T), "source");
                var expressions = properties
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