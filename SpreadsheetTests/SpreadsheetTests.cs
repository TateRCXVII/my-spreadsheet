using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula = new Formula("A1+B1");
            spreadsheet.SetCellContents("A1", formula);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.GetCellContents("3g4");
        }

        [TestMethod]
        [Timeout(5000)]
        public void GetNamesSimpleTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 30);
            spreadsheet.SetCellContents("B1", 30);
            spreadsheet.SetCellContents("C1", 30);
            spreadsheet.SetCellContents("D1", 30);
            List<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            expected.Add("D1");
            IEnumerable<string> cellNames = spreadsheet.GetNamesOfAllNonemptyCells();
            foreach (string name in cellNames)
            {
                Assert.IsTrue(expected.Contains(name));
            }
        }


        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsStringTest()
        {
            SS.Spreadsheet sheet = new SS.Spreadsheet();
            sheet.SetCellContents("A1", "Apple");
            Assert.AreEqual(sheet.GetCellContents("A1"), "Apple");
        }

        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsNullFormulaTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula? formula = null;
            spreadsheet.SetCellContents("A1", formula);
        }

        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsNumberTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 30);
            Assert.AreEqual(30, spreadsheet.GetCellContents("A1"));
        }
    }
}