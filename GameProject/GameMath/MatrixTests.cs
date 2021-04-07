using NUnit.Framework;
using System;

namespace GameProject.GameMath
{
    [TestFixture]
    public class MatrixTests
    {
        private readonly Random random = new Random(24184718);

        private Matrix3F GenerateRandomMatrix()
        {
            var array = new float[3, 3];
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    array[i, j] = GetRandomFloat();

            return Matrix3F.CreateFromArray(array);
        }

        private Vector2F GenerateRandomVector2(float factor = 1000f) =>
            new Vector2F(GetRandomFloat(factor), GetRandomFloat(factor));

        private Matrix3F CreateMatrixFromArray(float[] array)
        {
            var m = new float[3, 3];
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    m[i, j] = array[3 * i + j];

            return Matrix3F.CreateFromArray(m);
        }

        private float GetRandomFloat(float factor = 1000f)
        {
            return (float)random.NextDouble() * factor;
        }

        [Test]
        [Repeat(256)]
        public void TestAddSub()
        {
            var a = GenerateRandomMatrix();
            var b = GenerateRandomMatrix();
            var c = a + b - b - b + b;

            Assert.IsTrue(Matrix3F.AreAlmostEqual(a, c, 1e-3f));
        }

        [Test]
        [Repeat(256)]
        public void TestTranspose()
        {
            var m = GenerateRandomMatrix();
            var t = m.Transposed;

            Assert.AreEqual(m.Row1, t.Column1);
            Assert.AreEqual(m.Row2, t.Column2);
            Assert.AreEqual(m.Row3, t.Column3);
        }

        [TestCase(
            new[]
            {
                12.3f, 4f, 12.3f,
                12f, 7.90f, 3.46f,
                82f, 367.8f, 24f
            },
            new[]
            {
                54.65f, 15.1f, 7f,
                78.6f, 3f, 4f,
                7.34f, 787f, 35.4f
            },
            new[]
            {
                1076877f / 1000f, 987783f / 100f, 13438f / 25f,
                3255341f / 2500f, 73198f / 25f, 59521f / 250f,
                1678327f / 50f, 106148f / 5f, 14474f / 5f
            })]
        [TestCase(
            new[]
            {
                12.3f, 4f, 12.3f,
                12f, 7.90f, 3.46f,
                82f, 367.8f, 24f
            },
            new[]
            {
                54.65f,
                78.6f,
                7.34f
            },
            new[]
            {
                1076877f / 1000f,
                3255341f / 2500f,
                1678327f / 50f
            })]
        public void TestMultiplication(float[] m1, float[] m2, float[] result)
        {
            if (m2.Length == 9)
            {
                Assert.IsTrue(Matrix3F.AreAlmostEqual(
                    CreateMatrixFromArray(result),
                    CreateMatrixFromArray(m1) * CreateMatrixFromArray(m2),
                    1e-3f
                ));
                return;
            }
            Assert.IsTrue(Vector3F.AreAlmostEqual(
                new Vector3F(result[0], result[1], result[2]),
                CreateMatrixFromArray(m1) * new Vector3F(m2[0], m2[1], m2[2]),
                1e-3f
            ));
        }

        [Test]
        [Repeat(256)]
        public void TestTranslate()
        {
            var v0 = GenerateRandomVector2();
            var dv = GenerateRandomVector2();

            Assert.AreEqual(v0 + dv, v0.TransformBy(Matrix3F.CreateTranslation(dv)));
        }

        [Test]
        [Repeat(256)]
        public void TestScaling()
        {
            var v0 = GenerateRandomVector2();
            var scale = GenerateRandomVector2();
            var scaled = new Vector2F(v0.X * scale.X, v0.Y * scale.Y);

            Assert.AreEqual(scaled, v0.TransformBy(Matrix3F.CreateScale(scale)));
        }

        [TestCase(1, 0, MathF.PI / 2, 0, 1)]
        [TestCase(1, 0, MathF.PI, -1, 0)]
        [TestCase(0, 1, MathF.PI / 2, -1, 0)]
        [TestCase(0, 1, MathF.PI, 0, -1)]
        public void TestRotate(float x0, float y0, float angle, float x, float y)
        {
            var v0 = new Vector2F(x0, y0);
            var expect = new Vector2F(x, y);
            var v = v0.TransformBy(Matrix3F.CreateRotation(angle));

            Assert.IsTrue(Vector2F.AreAlmostEqual(expect, v));
        }
    }
}
