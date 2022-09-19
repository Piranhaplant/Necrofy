using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Necrofy;

namespace Tests
{
    [TestClass]
    public class NumericStringComparerTests
    {
        [TestMethod]
        public void TestBasic() {
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc", "def") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("def", "abc") > 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc", "abc") == 0);
        }

        [TestMethod]
        public void TestNumbers() {
            Assert.IsTrue(NumericStringComparer.instance.Compare("1", "2") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("2", "1") > 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("1", "1") == 0);

            Assert.IsTrue(NumericStringComparer.instance.Compare("1", "02") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("02", "1") > 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("1", "01") > 0);

            Assert.IsTrue(NumericStringComparer.instance.Compare("1", "10") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("2", "10") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("01", "10") < 0);
        }

        [TestMethod]
        public void TestCombined() {
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc1", "abc2") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc2", "abc1") > 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc1", "abc1") == 0);

            Assert.IsTrue(NumericStringComparer.instance.Compare("abc1", "abc10") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc10", "abc1") > 0);

            Assert.IsTrue(NumericStringComparer.instance.Compare("abc", "abc1") < 0);
            Assert.IsTrue(NumericStringComparer.instance.Compare("abc1", "abc") > 0);
        }
    }
}
