﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
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
        private readonly static Regex VariableRegex = new Regex(@"^[a-zA-Z_](?:[a-zA-Z_]|\d)*");
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
            VerifyParsing(GetTokens(formula), normalize, isValid);
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
            this.formula = formula;
            this.normalize = normalize;
            this.isValid = isValid;

            VerifyParsing(GetTokens(formula), normalize, isValid);
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
        /// <returns>
        /// A FormulaFormat Error if a divide by zero or invalid variable lookup
        /// occurs, or returns the result of the operation.
        /// </returns>
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
                    string eqnPart = token;

                    double val = 0;
                    if (double.TryParse(eqnPart, out val))
                    {
                        IntegerOrVariable(val, Operator, Value);
                    }
                    //if it's a variable
                    else if (VariableRegex.IsMatch(eqnPart))
                    {
                        IntegerOrVariable(lookup(normalize(eqnPart)), Operator, Value);
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
                        }

                    }
                }
                if (Operator.Count == 0)
                    return Value.Pop();
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
        /// <exception cref="DivideByZeroException">
        /// Helper method throws divide by zero exception.
        /// </exception>
        private static void IntegerOrVariable(double val, Stack<string> Operator, Stack<double> Value)
        {
            if (Operator.HasOnTop("*") || Operator.HasOnTop("/"))
            {
                Value.Push(val);
                double result = MultiplyOrDivide(Operator, Value);
                if (result == double.PositiveInfinity)
                    throw new DivideByZeroException();
                Value.Push(result);
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
            {
                double result = MultiplyOrDivide(Operator, Value);
                if (result == double.PositiveInfinity)
                    throw new DivideByZeroException();

                Value.Push(result);
            }
        }

        /// <summary>
        /// Helper method to handle multiplication and division and its accompanying errors.
        /// </summary>
        /// <param name="Operator">The Operator Stack</param>
        /// <param name="Value">The Value Stack</param>
        /// <returns>returns the result of the multiplication or division of two integers</returns>
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

        /// <summary>
        /// Method which validates tokens against specific parsing rules pasted below
        ///
        ///Specific Token Rule - the only valid tokens are (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation).
        ///One Token Rule - There must be at least one token.
        ///Right Parentheses Rule -  When reading tokens from left to right, at no point should the number of closing parentheses seen so far be greater than the number of opening parentheses seen so far.
        ///Balanced Parentheses Rule - The total number of opening parentheses must equal the total number of closing parentheses.
        ///Starting Token Rule - The first token of an expression must be a number, a variable, or an opening parenthesis.
        ///Ending Token Rule - The last token of an expression must be a number, a variable, or a closing parenthesis.
        ///Parenthesis/Operator Following Rule - Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.
        ///Extra Following Rule - Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        /// <param name="formula">Input formula</param>
        /// 
        private static void VerifyParsing(IEnumerable<string> formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            String doublePattern1 = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            Regex lpPattern = new Regex(@"\(");
            Regex varPattern = new Regex(@"^[a-zA-Z]+[0-9]+$");
            Regex doublePattern = new Regex(doublePattern1);
            Regex rpPattern = new Regex(@"\)");
            Regex opPattern = new Regex(@"[\+\-*/]");

            int lp_count = 0;
            int rp_count = 0;
            bool rp_prev = false;
            bool lp_prev = false;


            if (formula.Count() == 0)
                throw new FormulaFormatException("Formula can't be empty.");
            foreach (string token in formula)
            {

                //1
                if (!Double.TryParse(token, out double d) && token != ")" && token != "("
                    && !varPattern.IsMatch(token) && !opPattern.IsMatch(token))
                    throw new FormulaFormatException("Invalid token in the formula.");
                //7
                if (lp_prev)
                {
                    if ((!Double.TryParse(token, out double x) && opPattern.IsMatch(token)) || rpPattern.IsMatch(token))
                        throw new FormulaFormatException("Operators or closing parentheses can't follow an opening parenthesis.");
                    lp_prev = false;
                }

                //8
                if (rp_prev)
                {
                    if (!(opPattern.IsMatch(token) || rpPattern.IsMatch(token)))
                        throw new FormulaFormatException("Only operators or closing parentheses can follow numbers, variables, or closing parentheses.");
                    rp_prev = false;
                }

                //5 & 6
                if (!Double.TryParse(formula.First(), out double first))
                {
                    if (rpPattern.IsMatch(formula.First()) || opPattern.IsMatch(formula.First()))
                        throw new FormulaFormatException("Formula can't begin with a close parenthesis or operator.");
                }
                if (!Double.TryParse(formula.Last(), out double last))
                {
                    if (lpPattern.IsMatch(formula.Last()) || opPattern.IsMatch(formula.Last()))
                        throw new FormulaFormatException("Last token can't be an operator or (");
                }

                if (lpPattern.IsMatch(token))
                {
                    lp_count++;
                    lp_prev = true;
                }

                if (rpPattern.IsMatch(token))
                {
                    rp_count++;
                    rp_prev = true;
                }
                //3
                if (rp_count > lp_count)
                    throw new FormulaFormatException("More ) than ( in the formula.");

                if (!Double.TryParse(token, out double v) && opPattern.IsMatch(token))
                    lp_prev = true;
                if (Double.TryParse(token, out double o) || varPattern.IsMatch(token))
                    rp_prev = true;

                if (varPattern.IsMatch(token) && !isValid(normalize(token)))
                {
                    throw new FormulaFormatException("Variable not valid.");
                }
            }
            //4
            if (rp_count != lp_count)
                throw new FormulaFormatException("The number of ( doesn't match the number of ).");
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
        /// <returns>An IEnumberable of all the variables</returns>
        public IEnumerable<String> GetVariables()
        {
            Regex varPattern = new Regex(@"^[a-zA-Z_](?:[a-zA-Z_]|\d)*");

            HashSet<string> variables = new HashSet<string>();
            foreach (string token in GetTokens(formula))
            {
                if (varPattern.IsMatch(token) && isValid(normalize(token)))
                    variables.Add(normalize(token));
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
        /// <returns>A string version of the formula</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in GetTokens(formula))
            {
                //for sci notation and decimals
                if (Double.TryParse(s, out double d))
                    sb.Append(d.ToString());
                else if (VariableRegex.IsMatch(s) && isValid(normalize(s)))
                    sb.Append(normalize(s));
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
        /// <returns>True if equations are equal, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            //TODO: Figure out why null isn't working

            return this.ToString().Equals(obj?.ToString());
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        /// <returns>True if equations are equal, false otherwise</returns>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        /// <returns>True if equations are not equal, false otherwise</returns>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return f2.GetHashCode() != f1.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        /// <returns>The hashcode based on the toString value of the formula</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        /// <returns>An IEnumberable of the formula</returns>
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

