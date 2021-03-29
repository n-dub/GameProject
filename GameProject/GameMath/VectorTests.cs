using NUnit.Framework;

namespace GameProject.GameMath
{
    [TestFixture]
    internal class VectorTests
    {
        private const float Sqrt2 = 1.4142135623730f;
        private const float Sqrt3 = 1.7320508075688f;

        [TestCase(1f, 1f, Sqrt2)]
        [TestCase(2f, 2f, 2 * Sqrt2)]
        [TestCase(2f, -2f, 2 * Sqrt2)]
        [TestCase(0, 0, 0)]
        public void LengthTests2D(float x, float y, float length)
        {
            Assert.AreEqual(length, new Vector2F(x, y).Length, 1e-6);
        }

        [TestCase(
            1, 3,
            2, 4,
            3, 7)]
        [TestCase(
            0, 0,
            0, 0,
            0, 0)]
        [TestCase(
            1, 1,
            -1, -1,
            0, 0)]
        public void AddSubTests2D(float x1, float y1, float x2, float y2, float xr, float yr)
        {
            Assert.AreEqual(new Vector2F(x1, y1) + new Vector2F(x2, y2), new Vector2F(xr, yr));
            Assert.AreEqual(new Vector2F(x1, y1) - new Vector2F(-x2, -y2), new Vector2F(xr, yr));
        }

        [TestCase(
            1, 0,
            0, 1,
            0)]
        [TestCase(
            2, 3,
            4, 5,
            23)]
        [TestCase(
            -1, 2,
            1, -2,
            -5)]
        public void DotProductTests2D(float x1, float y1, float x2, float y2, float result)
        {
            Assert.AreEqual(new Vector2F(x1, y1) * new Vector2F(x2, y2), result, 1e-6);
        }

        [TestCase(1f, 1f, 1f, Sqrt3)]
        [TestCase(2f, 2f, 2f, 2 * Sqrt3)]
        [TestCase(2f, -2f, 2f, 2 * Sqrt3)]
        [TestCase(0, 0, 0, 0)]
        public void LengthTests3D(float x, float y, float z, float length)
        {
            Assert.AreEqual(length, new Vector3F(x, y, z).Length, 1e-6);
        }

        [TestCase(
            1, 3, 1,
            2, 4, 2,
            3, 7, 3)]
        [TestCase(
            0, 0, 0,
            0, 0, 0,
            0, 0, 0)]
        [TestCase(
            1, 1, 1,
            -1, -1, -1,
            0, 0, 0)]
        public void AddSubTests3D(float x1, float y1, float z1,
            float x2, float y2, float z2,
            float xr, float yr, float zr)
        {
            Assert.AreEqual(new Vector3F(x1, y1, z1) + new Vector3F(x2, y2, z2), new Vector3F(xr, yr, zr));
            Assert.AreEqual(new Vector3F(x1, y1, z1) - new Vector3F(-x2, -y2, -z2), new Vector3F(xr, yr, zr));
        }

        [TestCase(
            1, 0, 1,
            0, 1, 0,
            0)]
        [TestCase(
            2, 3, 2,
            4, 5, 3,
            29)]
        [TestCase(
            -1, 2, 1,
            1, -2, -1,
            -6)]
        public void DotProductTests3D(float x1, float y1, float z1,
            float x2, float y2, float z2,
            float result)
        {
            Assert.AreEqual(new Vector3F(x1, y1, z1) * new Vector3F(x2, y2, z2), result, 1e-6);
        }
    }
}
