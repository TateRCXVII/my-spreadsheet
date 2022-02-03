// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        //Regex object to check if a token is a variable (any # of letters followed by any # of digits)
        private readonly static Regex VariableRegex = new(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*");
        private readonly string formula;
        private readonly Func<string, string> normalize;
        private readonly Func<string, bool> isValid;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            this.formula = formula;
            this.normalize = s => s;
            this.isValid = s => true;
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            this.formula = normalize(formula);
            //TODO: Is this helpful?
            //bool validFormula = isValid(formula);
            this.normalize = normalize;
            this.isValid = isValid;



            if (!isValid(formula))
            {
                throw new FormulaFormatException("The input formula doesn't match the validator function.");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //TODO: Delete if (!this.isValid(formula)) return new FormulaError("The input formula doesn't match validator standards. Check your validator against your formula");
            Stack<string> Operator = new Stack<string>();
            Stack<double> Value = new Stack<double>();

            IEnumerable<string> substrings = GetTokens(formula);
            try
            {

                //iterate through the substrings and perform operations accordingly
                foreach (string token in substrings)
                {
                    //if (token.Equals(" ") || token.Equals("")) continue; //TODO: REMOVE?

                    string eqnPart = token.Trim();

                    double val = 0;
                    if (double.TryParse(eqnPart, out val))
                    {
                        IntegerOrVariable(val, Operator, Value);
                        //TODO: DELETE else return new FormulaError("Only operators or ) can follow numbers, variables, or )");
                    }
                    //if it's a variable
                    else if (VariableRegex.IsMatch(eqnPart))
                    {
                        IntegerOrVariable(lookup(normalize(eqnPart)), Operator, Value);
                        //TODO: DELETE else return new FormulaError("Only operators or ) can follow numbers, variables, or )");
                    }
                    else
                    {
                        switch (eqnPart)
                        {
                            case "+":
                                AddOrSubtractOperator(eqnPart, Operator, Value);
                                break;
                            case "-":
                                AddOrSubtractOperator(eqnPart, Operator, Value);
                                break;
                            case "*":
                                Operator.Push(eqnPart);
                                break;
                            case "/":
                                Operator.Push(eqnPart);
                                break;
                            case "(":
                                Operator.Push(eqnPart);
                                break;
                            case ")":
                                RightParenthesis(Operator, Value);
                                break;

                                //default:
                                //    return new FormulaError("Invalid expression input. Check for typos or make sure +,-,/,* are the only operators.");
                                //    //throw new ArgumentException("Invalid expression input.");
                        }

                    }
                }

                //if (Operator.Count == 0 && Value.Count > 1)
                //    return new FormulaError("Invalid expression input. More values than there are operators.");
                /* else*/
                if (Operator.Count == 0)
                    return Value.Pop();
                //else if (Value.Count != 2)
                //    return new FormulaError("Invalid expression input. More values than there are operators.");
                //else if (Operator.Count > 1)
                //    return new FormulaError("Invalid expression input. More operators than there are values.");
                else
                    return AddOrSubtract(Value, Operator);
            }
            catch (Exception e)
            {
                if (e is DivideByZeroException)
                    return new FormulaError("Invalid formula: Divide by zero error.");
                else if (e is ArgumentException)
                    return new FormulaError("Invalid formula: Undefined variable.");
                else
                    return new FormulaError("Invalid formula.");
            }

        }

        /// <summary>
        /// Helper method to handle when the + or - operators are encountered
        /// </summary>
        /// <param name="s">Current operator (either + or -)</param>
        /// <param name="Operator">Stack of operators</param>
        /// <param name="Value">Stack of values</param>
        private static void AddOrSubtractOperator(String s, Stack<string> Operator, Stack<double> Value)
        {
            if (Operator.HasOnTop("+") || Operator.HasOnTop("-"))
            {
                double result = AddOrSubtract(Value, Operator);

                Value.Push(Convert.ToDouble(result));
            }

            Operator.Push(s);
        }

        /// <summary>
        /// A helper method to perform addition or subtraction from a value stack and operator stack
        /// </summary>
        /// <param name="Value">Stack of values</param>
        /// <param name="Operator">Stack of operators</param>
        /// <returns> the result of summing the top two values with + or - </returns>
        /// <exception cref="ArgumentException">
        /// If there aren't enough values on the value stack, an error is thrown.
        /// </exception>
        private static double AddOrSubtract(Stack<double> Value, Stack<string> Operator)
        {
            double right = Value.Pop();
            double left = Value.Pop();
            double sum = 0;
            string op = Operator.Pop();
            if (op.Equals("+"))
                sum = left + right;
            else
                sum = left - right;
            return sum;
        }


        /// <summary>
        /// A helper method to handle if an input is an integer or a variable.
        /// </summary>
        /// <param name="val">The value being handled, which represents the right value in operations.</param>
        /// <param name="Operator"> Stack of operators</param>
        /// <param name="Value">Stack of values</param>
        /// <exception cref="ArgumentException">
        /// If there aren't values to perform operations on, an error is thrown.
        /// </exception>
        private static void IntegerOrVariable(double val, Stack<string> Operator, Stack<double> Value)
        {
            if (Operator.HasOnTop("*") || Operator.HasOnTop("/"))
            {
                Value.Push(val);
                Value.Push(MultiplyOrDivide(Operator, Value));
            }
            else
                Value.Push(val);
        }


        /// <summary>
        /// Helper method to handle the operations which accompany a ")"
        /// </summary>
        /// <param name="Operator">Operator stack</param>
        /// <param name="Value">Value stack</param>
        private static void RightParenthesis(Stack<string> Operator, Stack<double> Value)
        {
            if (Operator.HasOnTop("+") || Operator.HasOnTop("-"))
            {
                Value.Push(AddOrSubtract(Value, Operator));
            }

            if (Operator.HasOnTop("("))
                Operator.Pop();

            if (Operator.HasOnTop("*") || Operator.HasOnTop("/"))
                Value.Push(MultiplyOrDivide(Operator, Value));
        }

        /// <summary>
        /// Helper method to handle multiplication and division and its accompanying errors.
        /// </summary>
        /// <param name="Operator">The Operator Stack</param>
        /// <param name="Value">The Value Stack</param>
        /// <returns>returns the result of the multiplication or division of two integers</returns>
        /// <exception cref="ArgumentException">
        /// If the value stack doesn't have enough integers or a division by 0 occurs, an error is thrown.
        /// </exception>
        private static double MultiplyOrDivide(Stack<string> Operator, Stack<double> Value)
        {
            double right = Value.Pop();
            double left = Value.Pop();
            String op = Operator.Pop();
            if (op.Equals("*"))
                return left * right;
            else
            {
                return left / right;
            }
        }

        private static void VerifyParsing(IEnumerable<string> formula)
        {
            foreach (string token in formula)
            {

            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            Regex varPattern = new Regex(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*");
            List<string> variables = new List<string>();
            foreach (string token in GetTokens(formula))
            {
                if (varPattern.IsMatch(token))
                    variables.Append(token);
            }
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in GetTokens(formula))
            {
                //for sci notation
                if (Double.TryParse(s, out double d))
                    sb.Append(d.ToString());
                else
                    sb.Append(s);
            }
            return sb.ToString();

        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            //TODO: Figure out why null isn't working
            if (obj == null)
                return false;
            return this.ToString().Equals(obj?.ToString());
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f2.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            //TODO: Confirm this is a good idea
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

