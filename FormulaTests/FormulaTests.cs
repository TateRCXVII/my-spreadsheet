using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        // ************************** TESTS ON CONSTRUCTOR ************************* //

        /// <summary>
        ///Simple constructor test with simple normalizer/validator
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Constructor")]
        public void SimpleNormalizerToLowerTest()
        {
            Formula notEmpty = new Formula("40+30.5*100+(X1+Y1)", s => s.ToLower(), s => true);
            Assert.AreEqual("40+30.5*100+(x1+y1)", notEmpty.ToString());
        }

        /// <summary>
        ///Simple constructor test with normalizer to remove white space
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Constructor")]
        public void SimpleNormalizerWhiteSpaceTest()
        {
            Formula notEmpty = new Formula("40 +30.5 *100 +(X1+ Y1)", s => Regex.Replace(s, @"\s+", ""), s => true);
            Assert.AreEqual("40+30.5*100+(X1+Y1)", notEmpty.ToString());
        }

        /// <summary>
        ///Simple constructor test with simple normalizer/validator
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Constructor")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidFormulaConstructor()
        {
            Formula notEmpty = new Formula("40+30.5*100+(X1+Y1)", s => s.ToLower(), s => false);
        }


        // ************************** TESTS ON EVALUATION ************************* //

        /// <summary>
        ///Simple formula evaluation test
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void SimpleEvaluationTest()
        {
            Formula notEmpty = new Formula("40 +30.5 *100 +(X1+ Y1)", s => Regex.Replace(s, @"\s+", ""), s => true);
            Assert.AreEqual("40+30.5*100+(X1+Y1)", notEmpty.ToString());
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestAddition()
        {
            Formula add = new Formula("5+3", s => s, s => true);
            Assert.AreEqual(8, add.Evaluate(f => 0));
        }
        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestSubtraction()
        {
            Formula sub = new Formula("18-10", s => s, s => true);
            Assert.AreEqual(8, sub.Evaluate(s => 0));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestMultiplication()
        {
            Formula mult = new Formula("2*4", s => s, s => true);
            Assert.AreEqual(8, mult.Evaluate(s => 0));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("6")]
        public void TestDivision()
        {
            Formula div = new Formula("16/2", s => s, s => true);
            Assert.AreEqual(8, div.Evaluate(s => 0));
        }

        // ************************** TESTS ON ERRORS ************************* //



        // ************************** TESTS ON EQUALITY ************************* //

        /// <summary>
        ///Null formula shouldn't equal non-null formula
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void NullNotEqualNonNullTest()
        {
            Formula? empty = null;
            Formula notEmpty = new Formula("40+30.5*100");
            Assert.IsFalse(notEmpty.Equals(empty));
        }

        /// <summary>
        ///Null formula should equal other null formula (?)
        ///</summary>
        //TODO: Figure out if this is a valid test case
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void NullEqualNullTest()
        {
            Formula? empty = null;
            Formula? notEmpty = null;
            //bool hello = notEmpty?.Equals(empty);
            Assert.IsTrue(notEmpty?.Equals(empty));
        }

        /// <summary>
        ///Normalized Formulas should equal eachother
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void NormalizedEqualityTest()
        {
            Formula? form1 = new Formula("40+30.5*100/XY1", s => s.ToLower(), s => true); ;
            Formula form2 = new Formula("40 + 30.5 * 100 / xy1");
            Assert.IsTrue(form1 == form2);
            Assert.IsTrue(form1.Equals(form2));
        }


        /// <summary>
        ///Test sci notation
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void SciNotationTest()
        {
            Formula? form1 = new Formula("40 + 3.05e1 * 100 / XY1", s => s, s => true); ;
            Formula form2 = new Formula("40 + 30.5 * 100 / XY1");
            Assert.IsTrue(form1 == form2);
            Assert.IsTrue(form1.Equals(form2));
        }



        // ************************** TESTS ON HASHCODE ************************* //

        /// <summary>
        ///Same formulas should have same hashcode
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void NormalizedHashCodeTest()
        {
            Formula? form1 = new Formula("40+30.5*100/XY1", s => s.ToLower(), s => true); ;
            Formula form2 = new Formula("40 + 30.5 * 100 / xy1");
            Assert.IsTrue(form1.GetHashCode() == form2.GetHashCode());
            Assert.IsTrue(form1.GetHashCode().Equals(form2.GetHashCode()));
        }
    }
}