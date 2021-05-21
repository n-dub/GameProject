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
            private static readonly PropertyInfo[] properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            public static readonly Action<T, T> Cloner;

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

        public static CollisionInfo FindMaximumImpulseContact(PhysicsBody body)
        {
            return body.Collisions.Aggregate((0f, null as CollisionInfo?),
                (t, x) => t.Item1 < x.NormalImpulse
                    ? (x.NormalImpulse, x)
                    : t).Item2.GetValueOrDefault();
        }


        public static T[,] RunBreadthFirstSearch<T>(T[,] map, Point start, HashSet<Point> visited = null)
        {
            return RunBreadthFirstSearch(map, start, x => x == null, visited);
        }

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

        public static Vector2F GetGlobalPosition(this IRenderShape shape)
        {
            return shape.Offset.TransformBy(shape.Transform);
        }
    }
}