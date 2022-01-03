using System;
using NUnit.Framework;
using SharpTori;

namespace Test
{
    public class TestSharpTori
    {
        private byte[] _sample;

        [SetUp]
        public void Setup()
        {
            _sample = new byte[4] { 1, 2, 3, 4 };
        }

        [Test]
        public void TestInt()
        {
            Assert.AreEqual(typeof(int), _sample.ToStructure<int>().GetType());
        }
    }
}