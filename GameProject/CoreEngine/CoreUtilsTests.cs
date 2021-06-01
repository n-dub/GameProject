using System.Drawing;
using NUnit.Framework;

namespace GameProject.CoreEngine
{
    [TestFixture]
    internal class CoreUtilsTests
    {
        [Test]
        public void TestBfsSimple()
        {
            var testMatrix = new int?[,]
            {
                {1, 1, 1, null, null},
                {1, 1, 1, null, null},
                {1, 1, null, null, null}
            };
            MakeBfsTest(testMatrix, Point.Empty, testMatrix);
        }

        [Test]
        public void TestBfsStartWithNull()
        {
            var testMatrix = new int?[,]
            {
                {1, 1, null, null, null},
                {1, 1, null, null, null},
                {1, 1, null, null, null},
                {1, 1, null, null, null},
                {1, 1, 1, 1, 1},
                {1, 1, null, 1, 1},
                {1, 1, 1, 1, 1}
            };

            var nullMatrix = new int?[testMatrix.GetLength(0), testMatrix.GetLength(1)];

            MakeBfsTest(testMatrix, new Point(0, 2), nullMatrix);
            MakeBfsTest(testMatrix, new Point(0, 3), nullMatrix);
            MakeBfsTest(testMatrix, new Point(5, 2), nullMatrix);
        }

        [Test]
        public void TestBfsSeparate1()
        {
            var testMatrix = new int?[,]
            {
                {1, 1, 1, null, 1},
                {1, 1, 1, null, 1},
                {1, 1, null, 1, 1}
            };
            var expMatrix = new int?[,]
            {
                {1, 1, 1, null, null},
                {1, 1, 1, null, null},
                {1, 1, null, null, null}
            };
            MakeBfsTest(testMatrix, Point.Empty, expMatrix);
        }

        [Test]
        public void TestBfsSeparate2()
        {
            var testMatrix = new int?[,]
            {
                {1, 1, 1, null, 1},
                {1, 1, 1, null, 1},
                {1, 1, null, 1, 1}
            };
            var expMatrix = new int?[,]
            {
                {null, null, null, null, 1},
                {null, null, null, null, 1},
                {null, null, null, 1, 1}
            };
            MakeBfsTest(testMatrix, new Point(2, 4), expMatrix);
        }

        private static void MakeBfsTest<T>(T[,] source, Point start, T[,] expected)
        {
            var result = CoreUtils.RunBreadthFirstSearch(source, start);
            Assert.IsTrue(AreMatricesEqual(expected, result));
        }

        private static bool AreMatricesEqual<T>(T[,] left, T[,] right)
        {
            var (width, height) = (left.GetLength(0), left.GetLength(1));
            if (width != right.GetLength(0) || height != right.GetLength(1))
                return false;

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                if (!left[i, j].Equals(right[i, j]))
                    return false;

            return true;
        }
    }
}