using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {

        /************* EXCEPTION TESTS **************/

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        public void LargeConstructorXMLTest()
        {
            using (XmlWriter writer = XmlWriter.Create("test.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "version1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B2");
                writer.WriteElementString("contents", "= A2 + 30");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "world");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "30");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet sheet = new Spreadsheet("test.txt", s => true, s => s, "version1");
            Assert.AreEqual("world", sheet.GetCellContents("A1"));
            Assert.AreEqual(30.0, sheet.GetCellContents("A2"));
            Assert.AreEqual(60.0, sheet.GetCellValue("B2"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        [Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void DifferentVersionsConstructorTest()
        {
            using (XmlWriter writer = XmlWriter.Create("test.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "version1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B2");
                writer.WriteElementString("contents", "= A1 + 30");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "world");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "30");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet sheet = new Spreadsheet("test.txt", s => true, s => s, "version2");
            Assert.AreEqual(30.0, sheet.GetCellContents("A1"));
            Assert.AreEqual("world", sheet.GetCellContents("A2"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetUtilities.FormulaFormatException))]
        public void InvalidVariableSetTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=XYZ");
            Assert.IsTrue(spreadsheet.GetCellContents("A1") is FormulaFormatException);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        [Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void EmptyFilePathConstructorTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("C:\\tate\\Hello.txt", s => true, s => s, "version");
        }
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
            spreadsheet.SetContentsOfCell("A1", "=A1+B1");
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
            spreadsheet.SetContentsOfCell("A1", "=B1");
            spreadsheet.SetContentsOfCell("B1", "=C1");
            spreadsheet.SetContentsOfCell("C1", "=A1 + B1");
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
            Formula formula2 = new Formula("(((2 + x1+B1)*3e-2))");
            Formula formula3 = new Formula("B3+C4+C5");
            spreadsheet.SetContentsOfCell("B1", "=(((2 + x1+B1)*3e-2))");
            spreadsheet.SetContentsOfCell("A1", "=B3+C4+C5");
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
            spreadsheet.SetContentsOfCell("C1", "=" + formula4.ToString());
            spreadsheet.SetContentsOfCell("B1", "Apple");
            spreadsheet.SetContentsOfCell("B1", "=" + formula3.ToString());
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
        public void SetContentsOfCellTextInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("33pop", "Hi there friend.");
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellNumberInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("333", "400");
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellFormulaInvalidNameExceptionTest()
        {
            AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("3_g4", "=x1*2");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void EmptyFilePathExceptionTest()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s, "v1");
            sheet.SetContentsOfCell("T1", "= 1");
            sheet.SetContentsOfCell("E1", "23");
            sheet.SetContentsOfCell("F1", "500");
            sheet.SetContentsOfCell("B1", "=5 + E1");
            sheet.Save("");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void EmptyFilePathGetSavedExceptionTest()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s, "asdfas");
            sheet.SetContentsOfCell("T1", "= 1");
            sheet.SetContentsOfCell("E1", "23");
            sheet.SetContentsOfCell("F1", "500");
            sheet.SetContentsOfCell("B1", "=5 + E1");
            sheet.Save("C:\\Users\\tate\\files\\nonexistentPath.xml");
            string version = sheet.GetSavedVersion("");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void NonExistentFilePathGetSavedExceptionTest()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s, "asdfas");
            sheet.SetContentsOfCell("T1", "= 1");
            sheet.SetContentsOfCell("E1", "23");
            sheet.SetContentsOfCell("F1", "500");
            sheet.SetContentsOfCell("B1", "=5 + E1");
            sheet.Save("C:\\Users\\tate\\files\\nonexistentPath.xml");
            string version = sheet.GetSavedVersion("C:\\Users\\tate\\files\\nonexistentPath.xml");
        }

        /********** CONSTRUCTOR TESTS *************/




        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void DefaultConstructorFormulaEvaluationTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.IsFalse(sheet.Changed);
            sheet.SetContentsOfCell("A1", "=A5+x4");
            sheet.SetContentsOfCell("A5", "5");
            sheet.SetContentsOfCell("x4", "10");
            Assert.AreEqual("default", sheet.Version);
            Assert.AreEqual(new Formula("A5+x4"), sheet.GetCellContents("A1"));
            Assert.AreEqual(15.0, sheet.GetCellValue("A1"));
            Assert.IsTrue(sheet.Changed);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void EmptySheetConstructorTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void NoNormalizerConstructorTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=A5+x4");
            sheet.SetContentsOfCell("a1", "2");
            Assert.AreEqual(2.0, sheet.GetCellContents("a1"));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void ToUpperConstructorTest()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "sup");
            sheet.SetContentsOfCell("A1", "=A5+x4");
            sheet.SetContentsOfCell("a1", "= 2+5");
            Assert.AreEqual(7.0, sheet.GetCellValue("A1"));
        }

        /********** GET NAMES TESTS ***************/

        [TestMethod]
        [Timeout(5000)]
        public void GetNamesSimpleTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "30");
            spreadsheet.SetContentsOfCell("B1", "30");
            spreadsheet.SetContentsOfCell("C1", "30");
            spreadsheet.SetContentsOfCell("D1", "30");
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

        /********************* GETTERS AND SETTERS ****************************/
        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void ChangedTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "Hello");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("Save.xml"); //TODO: Filepath?
            Assert.IsFalse(sheet.Changed);
        }

        /********************* SET CELL CONTENTS TESTS ************************/

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetContentsOfCellStringTest()
        {
            SS.Spreadsheet sheet = new SS.Spreadsheet();
            IList<string> expected = new List<string>();
            IList<string> cellNames = sheet.SetContentsOfCell("A1", "Apple");
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
        public void SetContentsOfCellTextReplaceTest()
        {
            SS.Spreadsheet sheet = new SS.Spreadsheet();
            IList<string> cellNames1 = sheet.SetContentsOfCell("A1", "Apple");
            IList<string> cellNames2 = sheet.SetContentsOfCell("A1", "Pear");
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
        public void SetContentsOfCellFormulaTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetContentsOfCell("B1", "=" + formula2.ToString());
            spreadsheet.SetContentsOfCell("C1", "=" + formula2.ToString());
            IList<string> cellNames = spreadsheet.SetContentsOfCell("A1", "=" + formula1.ToString());
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
        public void SetContentsOfCellFormulaReplaceTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-3 + B4");
            Formula formula2 = new Formula("3005 + 3e-150 + B24");
            IList<string> cellNames1 = spreadsheet.SetContentsOfCell("A1", "=" + formula1.ToString());
            Assert.IsTrue((Formula)spreadsheet.GetCellContents("A1") == formula1);
            IList<string> cellNames2 = spreadsheet.SetContentsOfCell("A1", "=" + formula2.ToString());
            Assert.IsTrue((Formula)spreadsheet.GetCellContents("A1") == formula2);
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetContentsOfCellNumberTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "30");
            Assert.AreEqual(30.0, spreadsheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Replace existing data in a cell
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetContentsOfCellReplaceTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "30");
            spreadsheet.SetContentsOfCell("A1", "40");
            Assert.AreEqual(40.0, spreadsheet.GetCellContents("A1"));
        }

        /****************** SET CELL CONTENTS LIST TESTS ***************/

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SetContentsOfCellNumberListTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetContentsOfCell("B1", "=" + formula2.ToString());
            spreadsheet.SetContentsOfCell("C1", "=" + formula2.ToString());
            IList<string> cellNames = spreadsheet.SetContentsOfCell("A1", "30");
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
        public void SetContentsOfCellTextListTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetContentsOfCell("B1", "=" + formula2.ToString());
            spreadsheet.SetContentsOfCell("C1", "=" + formula2.ToString());
            IList<string> cellNames = spreadsheet.SetContentsOfCell("A1", "This is a test.");
            IList<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            foreach (string cell in cellNames)
                Assert.IsTrue(expected.Contains(cell));
        }

        /***************** Get Vals *************************/

        /// <summary>
        /// Replace existing data in a cell
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void GetCellValueTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "30");
            spreadsheet.SetContentsOfCell("A1", "40");
            Assert.AreEqual(40.0, spreadsheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Replace existing data in a cell
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void GetCellFormulaValueTest()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Formula formula1 = new Formula("30 + 30");
            Formula formula2 = new Formula("A1*2");
            spreadsheet.SetContentsOfCell("A1", "=" + formula1.ToString());
            spreadsheet.SetContentsOfCell("C1", "=" + formula2.ToString());
            Assert.AreEqual(120.0, spreadsheet.GetCellValue("C1"));
        }


        /******************** Save test  ****************/

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        public void EmptySaveTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => s, "Version1");
            sheet.Save("Sheet.txt");
            Assert.AreEqual("Version1", new Spreadsheet().GetSavedVersion("Sheet.txt"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        public void SimpleSaveTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => s, "Version1");
            Formula formula1 = new Formula("300 + 3e-15 + B2");
            Formula formula2 = new Formula("A1*2");
            sheet.SetContentsOfCell("B1", "=" + formula2.ToString());
            sheet.SetContentsOfCell("C1", "=" + formula2.ToString());
            sheet.SetContentsOfCell("C1", "40");
            sheet.SetContentsOfCell("E1", "string");
            sheet.Save("Sheet.xml");
            Assert.AreEqual("Version1", new Spreadsheet().GetSavedVersion("Sheet.txt"));
            Spreadsheet spreadsheet = new Spreadsheet("Sheet.xml", s => true, s => s, "Version1");
        }

        /******************* CELL CLASS TESTS ***************/
        /// <summary>
        /// Replace existing data in a cell
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void CellConstructorTest()
        {
            Cell cell = new Cell("A1", "30");
            Assert.AreEqual("A1", cell.Name);
        }
    }
}