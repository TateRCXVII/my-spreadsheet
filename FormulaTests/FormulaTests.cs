using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
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
            Formula? form1 = new Formula("40+30.5*100/Xy1"); ;
            Formula form2 = new Formula("40 + 30.5 * 100 / xY1");
            Assert.IsTrue(form1 == form2);
            Assert.IsTrue(form1.Equals(form2));
        }
    }
}