using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        /************* EXCEPTION TESTS **************/

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
        /// See title (more complex version 2)
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest2()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("B1");
            Formula formula2 = new Formula("C1");
            Formula formula3 = new Formula("A1 + B1");
            spreadsheet.SetCellContents("A1", formula1);
            spreadsheet.SetCellContents("B1", formula2);
            spreadsheet.SetCellContents("C1", formula3);
        }

        /// <summary>
        /// See title (more complex version 3)
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest3()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula2 = new Formula("(((2 + _x1+B1)*3e-2))");
            Formula formula3 = new Formula("B3+C4+C5");
            spreadsheet.SetCellContents("A1", formula3);
            spreadsheet.SetCellContents("B1", formula2);
        }

        /// <summary>
        /// See title (more complex version 3)
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionSameCellTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula3 = new Formula("C1");
            Formula formula4 = new Formula("B1+B1");
            spreadsheet.SetCellContents("C1", formula4);
            spreadsheet.SetCellContents("B1", formula3);
        }

        /// <summary>
        /// See title (more complex version 3)
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionReplaceTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula3 = new Formula("C1");
            Formula formula4 = new Formula("B1+B1");
            spreadsheet.SetCellContents("C1", formula4);
            spreadsheet.SetCellContents("B1", "Apple");
            spreadsheet.SetCellContents("B1", formula3);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.GetCellContents("3g4");
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTextInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("33pop", "Hi there friend.");
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsNumberInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("333", 400);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("3_g4", new Formula("x1*2"));
        }


        /********** GET NAMES TESTS ***************/

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

        /********************* SET CELL CONTENTS TESTS ************************/

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsStringTest()
        {
            SS.Spreadsheet sheet = new SS.Spreadsheet();
            IList<string> expected = new List<string>();
            IList<string> cellNames = sheet.SetCellContents("A1", "Apple");
            expected.Add("A1");
            foreach (string cell in cellNames)
                Assert.IsTrue(expected.Contains(cell));

            Assert.AreEqual(sheet.GetCellContents("A1"), "Apple");

        }

        /// <summary>
        /// Replace a cell that already has data
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsTextReplaceTest()
        {
            SS.Spreadsheet sheet = new SS.Spreadsheet();
            IList<string> cellNames1 = sheet.SetCellContents("A1", "Apple");
            IList<string> cellNames2 = sheet.SetCellContents("A1", "Pear");
            Assert.AreEqual(1, cellNames2.Count);
            IList<string> expected = new List<string>();
            expected.Add("A1");
            foreach (string cell in cellNames2)
                Assert.IsTrue(expected.Contains(cell));
            Assert.AreEqual(sheet.GetCellContents("A1"), "Pear");
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsFormulaTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetCellContents("B1", formula2);
            spreadsheet.SetCellContents("C1", formula2);
            IList<string> cellNames = spreadsheet.SetCellContents("A1", formula1);
            IList<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            foreach (string cell in cellNames)
                Assert.IsTrue(expected.Contains(cell));

            Assert.IsTrue((Formula)spreadsheet.GetCellContents("A1") == formula1);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsFormulaReplaceTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-3 + B4");
            Formula formula2 = new Formula("3005 + 3e-150 + B24");
            IList<string> cellNames1 = spreadsheet.SetCellContents("A1", formula1);
            Assert.IsTrue((Formula)spreadsheet.GetCellContents("A1") == formula1);
            IList<string> cellNames2 = spreadsheet.SetCellContents("A1", formula2);
            Assert.IsTrue((Formula)spreadsheet.GetCellContents("A1") == formula2);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsNullFormulaTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula? formula = null;
            spreadsheet.SetCellContents("A1", formula);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsNumberTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 30);
            Assert.AreEqual(30.0, spreadsheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Replace existing data in a cell
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsReplaceTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 30);
            spreadsheet.SetCellContents("A1", 40);
            Assert.AreEqual(40.0, spreadsheet.GetCellContents("A1"));
        }

        /****************** SET CELL CONTENTS LIST TESTS ***************/

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsNumberListTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetCellContents("B1", formula2);
            spreadsheet.SetCellContents("C1", formula2);
            IList<string> cellNames = spreadsheet.SetCellContents("A1", 30);
            IList<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            foreach (string cell in cellNames)
                Assert.IsTrue(expected.Contains(cell));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetCellContentsTextListTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetCellContents("B1", formula2);
            spreadsheet.SetCellContents("C1", formula2);
            IList<string> cellNames = spreadsheet.SetCellContents("A1", "This is a test.");
            IList<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            foreach (string cell in cellNames)
                Assert.IsTrue(expected.Contains(cell));
        }


        /******************* CELL CLASS TESTS ***************/
        /// <summary>
        /// Replace existing data in a cell
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void CellConstructorTest()
        {
            Spreadsheet.Cell cell = new Spreadsheet.Cell("A1");
            Assert.AreEqual("A1", cell.Name);
        }
    }
}