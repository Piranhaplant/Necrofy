using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Necrofy;

namespace Tests
{
    [TestClass]
    public class FreespaceTests
    {
        [TestMethod]
        public void TestIntersectsWith() {
            Freespace.FreeBlock baseBlock = new Freespace.FreeBlock(10, 20);

            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(10, 20)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(0, 30)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(10, 30)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(0, 20)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(12, 18)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(15, 25)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(5, 15)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(0, 10)));
            Assert.IsTrue(baseBlock.IntersectsWith(new Freespace.FreeBlock(20, 30)));

            Assert.IsFalse(baseBlock.IntersectsWith(new Freespace.FreeBlock(0, 9)));
            Assert.IsFalse(baseBlock.IntersectsWith(new Freespace.FreeBlock(21, 30)));
        }

        [TestMethod]
        public void TestMerge() {
            Freespace.FreeBlock baseBlock;

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(10, 20));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(0, 30));
            Assert.AreEqual(0, baseBlock.Start);
            Assert.AreEqual(30, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(10, 30));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(30, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(0, 20));
            Assert.AreEqual(0, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(12, 18));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(15, 25));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(25, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(5, 15));
            Assert.AreEqual(5, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(0, 10));
            Assert.AreEqual(0, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Merge(new Freespace.FreeBlock(20, 30));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(30, baseBlock.End);
        }

        [TestMethod]
        public void TestSubtractInteger() {
            Freespace.FreeBlock baseBlock;

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Subtract(0);
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Subtract(5);
            Assert.AreEqual(15, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            baseBlock.Subtract(10);
            Assert.AreEqual(20, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);
        }

        [TestMethod]
        public void TestSubtractBlock() {
            Freespace.FreeBlock baseBlock, result;

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(10, 20)));
            Assert.AreEqual(0, baseBlock.Size);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(0, 30)));
            Assert.AreEqual(0, baseBlock.Size);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(10, 30)));
            Assert.AreEqual(0, baseBlock.Size);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(0, 20)));
            Assert.AreEqual(0, baseBlock.Size);

            baseBlock = new Freespace.FreeBlock(10, 20);
            result = baseBlock.Subtract(new Freespace.FreeBlock(12, 18));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(12, baseBlock.End);
            Assert.AreEqual(18, result.Start);
            Assert.AreEqual(20, result.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(15, 25)));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(15, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(5, 15)));
            Assert.AreEqual(15, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(0, 10)));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);

            baseBlock = new Freespace.FreeBlock(10, 20);
            Assert.IsNull(baseBlock.Subtract(new Freespace.FreeBlock(20, 30)));
            Assert.AreEqual(10, baseBlock.Start);
            Assert.AreEqual(20, baseBlock.End);
        }

        [TestMethod]
        public void TestAdd() {
            Freespace freespace;

            freespace = new Freespace(Freespace.BankSize);
            freespace.Add(0x10, 0x10);
            Assert.AreEqual("", freespace.ToString());

            freespace.Add(0, 0x10);
            Assert.AreEqual("000000-000010 (000010)\r\n", freespace.ToString());

            freespace.Add(0x20, 0x30);
            freespace.Sort();
            Assert.AreEqual("000000-000010 (000010)\r\n000020-000030 (000010)\r\n", freespace.ToString());

            freespace.Add(0x30, 0x40);
            freespace.Sort();
            Assert.AreEqual("000000-000010 (000010)\r\n000020-000040 (000020)\r\n", freespace.ToString());

            freespace.Add(0x10, 0x20);
            Assert.AreEqual("000000-000040 (000040)\r\n", freespace.ToString());

            freespace.AddSize(0x40, 0x10);
            Assert.AreEqual("000000-000050 (000050)\r\n", freespace.ToString());

            freespace = new Freespace(Freespace.BankSize * 3);
            freespace.Add(Freespace.BankSize - 0x10, Freespace.BankSize * 2 + 0x10);
            freespace.Sort();
            Assert.AreEqual("007FF0-008000 (000010)\r\n008000-010000 (008000)\r\n010000-010010 (000010)\r\n", freespace.ToString());

            freespace = new Freespace(Freespace.BankSize * 3);
            freespace.Add(Freespace.BankSize - 0x10, Freespace.BankSize);
            freespace.Add(Freespace.BankSize, Freespace.BankSize + 0x10);
            freespace.Sort();
            Assert.AreEqual("007FF0-008000 (000010)\r\n008000-008010 (000010)\r\n", freespace.ToString());
        }

        [TestMethod]
        public void TestClaim() {
            Freespace freespace;

            freespace = new Freespace(Freespace.BankSize);
            freespace.Add(0x10, 0x20);
            Assert.AreEqual("000010-000020 (000010)\r\n", freespace.ToString());
            Assert.AreEqual(0x10, freespace.Claim(0x8));
            Assert.AreEqual("000018-000020 (000008)\r\n", freespace.ToString());

            Assert.AreEqual(0x18, freespace.Claim(0x8));
            Assert.AreEqual("", freespace.ToString());

            freespace.Add(0x10, 0x30);
            freespace.Add(0x40, 0x50);
            freespace.Sort();
            Assert.AreEqual("000010-000030 (000020)\r\n000040-000050 (000010)\r\n", freespace.ToString());
            Assert.AreEqual(0x40, freespace.Claim(0x8));
            Assert.AreEqual("000010-000030 (000020)\r\n000048-000050 (000008)\r\n", freespace.ToString());

            freespace = new Freespace(Freespace.BankSize);
            freespace.Add(0x10, 0x20);
            freespace.Add(0x30, 0x50);
            freespace.Sort();
            Assert.AreEqual("000010-000020 (000010)\r\n000030-000050 (000020)\r\n", freespace.ToString());
            Assert.AreEqual(0x30, freespace.Claim(0x18));
            Assert.AreEqual("000010-000020 (000010)\r\n000048-000050 (000008)\r\n", freespace.ToString());

            Assert.AreEqual(Freespace.BankSize, freespace.Claim(0x100));
            freespace.Sort();
            Assert.AreEqual("000010-000020 (000010)\r\n000048-000050 (000008)\r\n008100-010000 (007F00)\r\n", freespace.ToString());
        }

        [TestMethod]
        public void TestReserve() {
            Freespace freespace;

            freespace = new Freespace(Freespace.BankSize);
            freespace.Reserve(0, 0x10);
            Assert.AreEqual("", freespace.ToString());

            freespace.Add(0x10, 0x20);
            Assert.AreEqual("000010-000020 (000010)\r\n", freespace.ToString());
            freespace.Reserve(0x10, 0x8);
            Assert.AreEqual("000018-000020 (000008)\r\n", freespace.ToString());

            freespace.Reserve(0x18, 0x8);
            Assert.AreEqual("", freespace.ToString());

            freespace.Add(0x10, 0x20);
            Assert.AreEqual("000010-000020 (000010)\r\n", freespace.ToString());
            freespace.Reserve(0x12, 0x6);
            freespace.Sort();
            Assert.AreEqual("000010-000012 (000002)\r\n000018-000020 (000008)\r\n", freespace.ToString());

            freespace = new Freespace(Freespace.BankSize);
            freespace.Add(0x10, 0x20);
            freespace.Add(0x30, 0x40);
            freespace.Sort();
            Assert.AreEqual("000010-000020 (000010)\r\n000030-000040 (000010)\r\n", freespace.ToString());
            freespace.Reserve(0x0, 0x50);
            Assert.AreEqual("", freespace.ToString());
        }
    }
}
