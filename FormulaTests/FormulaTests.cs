using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

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
        public void SimpleNormalizerTest()
        {
            Formula notEmpty = new Formula("40+30.5*100+(X1+Y1)", s => s.ToLower(), s => true);
            Assert.AreEqual("40+30.5*100+(x1+y1)", notEmpty.ToString());
        }

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
            Assert.IsFalse(empty == notEmpty);
            Assert.IsFalse(empty.Equals(notEmpty));
        }

        /// <summary>
        ///Null formula should equal other null formula
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void NullEqualNullTest()
        {
            Formula? empty = null;
            Formula? notEmpty = null;
            Assert.IsTrue(empty == notEmpty);
            Assert.IsTrue(empty.Equals(notEmpty));
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
    }
}