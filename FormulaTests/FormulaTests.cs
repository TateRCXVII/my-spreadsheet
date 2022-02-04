using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
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
            Assert.AreEqual(8.0, add.Evaluate(f => 0));
        }
        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestSubtraction()
        {
            Formula sub = new Formula("18-10", s => s, s => true);
            Assert.AreEqual(8.0, sub.Evaluate(s => 0));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestMultiplication()
        {
            Formula mult = new Formula("2*4", s => s, s => true);
            Assert.AreEqual(8.0, mult.Evaluate(s => 0));
        }

        /// <summary>
        /// See name
        /// </summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("6")]
        public void TestDivision()
        {
            Formula div = new Formula("16/2", s => s, s => true);
            Assert.AreEqual(8.0, div.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestComplexMultiVar()
        {
            Formula form = new Formula("y1*3-8/2+4*(8-9*2)/14*x7", s => s, s => true);
            Assert.AreEqual(5.142857142857142, form.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestComplexNestedParensRight()
        {
            Formula form = new Formula("x1+(x2+(x3+(x4+(x5+x6))))", s => s, s => true);
            Assert.AreEqual(6.0, form.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("Evaluation")]
        public void TestComplexNestedParensLeft()
        {
            Formula form = new Formula("((((x1+x2)+x3)+x4)+x5)+x6", s => s, s => true);
            Assert.AreEqual(12.0, form.Evaluate(s => 2));
        }

        // ************************** TESTS ON ERRORS ************************* //

        /// <summary>
        ///Empty formula should throw exception
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EmptyFormulaTest()
        {
            Formula formula = new Formula("");
        }

        /// <summary>
        ///Formula can't be opened with )
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OpenWithInRParenTest()
        {
            Formula formula = new Formula(")5+5");
        }

        /// <summary>
        ///Formula with invalid token
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokenTest()
        {
            Formula formula = new Formula("12^7");
        }

        /// <summary>
        ///Empty formula should throw exception
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        public void LookupTurnsVarInvalid()
        {
            Formula formula = new Formula("x1 + y1 + 4");
            FormulaError error = new FormulaError();
            error = (FormulaError)formula.Evaluate(s => Convert.ToDouble("a"));
            Assert.AreEqual("Invalid formula.", error.Reason);
        }


        /// <summary>
        ///Simple constructor test with simple normalizer/validator
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidFormulaConstructor()
        {
            Formula notEmpty = new Formula("40+30.5*100+(X1+Y1)", s => s.ToLower(), s => false);
        }

        /// <summary>
        ///Normalizer sets var to lower case, validator checks if var is upper case
        ///Shoule fail
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ContradictingValidatorNormalizerTest()
        {
            Regex UpperVariableRegex = new("[A-Z]+[0-9]+");
            Formula notEmpty = new Formula("40+30.5*100+(X1+Y1)", s => s.ToLower(), s => UpperVariableRegex.IsMatch(s));
        }

        /// <summary>
        ///Imbalanced left parentheses
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ImbalancedLParenTest()
        {
            Formula notEmpty = new Formula("40+30.5*100+((X1+Y1)");
        }

        /// <summary>
        ///Imbalanced right parentheses
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ImbalancedRParenTest()
        {
            Formula notEmpty = new Formula("40+30.5*100+((X1+Y1)))");
        }


        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorCantFollowLParen()
        {
            Formula notEmpty = new Formula("40+30.5*100+((*X1+Y1))");
        }

        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorCantFollowOperator()
        {
            Formula notEmpty = new Formula("40++30.5*100+((X1+Y1))");
        }

        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NumberCantFollowRParen()
        {
            Formula notEmpty = new Formula("40+30.5*100+((X1+Y1))5");
        }
        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EmptyParenTest()
        {
            Formula notEmpty = new Formula("()");
        }

        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        public void DivideByZeroError()
        {
            Formula notEmpty = new Formula("40/0");
            FormulaError error = (FormulaError)notEmpty.Evaluate(s => 1);
            Assert.AreEqual("Invalid formula: Divide by zero error.", error.Reason);
        }


        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        public void DivideByZeroErrorRParen()
        {
            Formula notEmpty = new Formula("((40/50)/0)");
            FormulaError error = (FormulaError)notEmpty.Evaluate(s => 1);
            Assert.AreEqual("Invalid formula: Divide by zero error.", error.Reason);
        }

        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void LastTokenOperator()
        {
            Formula notEmpty = new Formula("(40/0)*");
        }

        /// <summary>
        ///See name
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("Errors")]
        public void NonexistentVariableError()
        {
            Formula notEmpty = new Formula("(40/X2)");
            FormulaError error = (FormulaError)notEmpty.Evaluate(s => throw new ArgumentException());
            Assert.AreEqual("Invalid formula: Undefined variable.", error.Reason);
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

        /// <summary>
        ///Test inequality operator
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Equality")]
        public void NotEqualsTest()
        {
            Formula? form1 = new Formula("40 + 3.05e1 * 100 / XY1", s => s, s => true);
            Formula form2 = new Formula("40.8 + 30.5 * 100 / XY1");
            Assert.IsTrue(form1 != form2);
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

        // ************************** TESTS ON GETTOKENS ************************* //

        /// <summary>
        ///Formula with valid tokens should return all variables
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Get Tokens")]
        public void GetVariablesSimpleNormalizerTest()
        {
            Formula form1 = new Formula("40+30.5*100/XY1+AB2+X3", s => s.ToLower(), s => true);
            List<string> expectedVars = new List<string>();
            expectedVars.Add("xy1");
            expectedVars.Add("ab2");
            expectedVars.Add("x3");
            IEnumerable<string> list = form1.GetVariables();
            foreach (string variable in form1.GetVariables())
            {
                Assert.IsTrue(expectedVars.Contains(variable));
            }
        }

        /// <summary>
        ///Formula with valid tokens but different cases should return all variables, but won't match
        ///</summary>
        [TestMethod(), Timeout(2000)]
        [TestCategory("Get Tokens")]
        public void GetVariablesDifferentCasesSimpleTest()
        {
            Formula form1 = new Formula("40+30.5*100/xy1+Ab2+x3", s => s, s => true);
            List<string> expectedVars = new List<string>();
            expectedVars.Add("XY1");
            expectedVars.Add("AB2");
            expectedVars.Add("X3");
            foreach (string variable in form1.GetVariables())
            {
                Assert.IsTrue(expectedVars.Contains(variable));
            }
        }
    }
}