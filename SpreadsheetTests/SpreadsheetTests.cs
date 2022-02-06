using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {

        }

        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameExceptionTest()
        {

        }

        [TestMethod]
        [Timeout(5000)]
        public void GetNamesTest()
        {

        }


        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsStringTest()
        {

        }

        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsNumberTest()
        {

        }

        [TestMethod]
        [Timeout(5000)]
        public void GetDirectDependentsTest()
        {

        }

        [TestMethod]
        [Timeout(5000)]
        public void GetCellsToRecalculateTest()
        {

        }
    }
}