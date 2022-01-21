using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary> 
    /// Author:    Tate Reynolds 
    /// Partner:   None 
    /// Date:      1/13/22 
    /// Course:    CS 3500, University of Utah, School of Computing 
    /// Copyright: CS 3500 and Tate Reynolds - This work may not be copied for use in Academic Coursework. 
    /// 
    /// I, Tate Reynolds, certify that I wrote this code from scratch and did not copy it in part or whole from  
    /// another source.  All references used in the completion of the assignment are cited in my README file. 
    /// 
    /// File Contents 
    /// 
    /// This namespace and class is a formula evaluator using RegEx and delegates. 
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// stacks used for the infix operations
        /// </summary>
        private static Stack<string> Operator = new Stack<string>();
        private static Stack<int> Value = new Stack<int>();

        //Regex object to check if a token is a variable (any # of letters followed by any # of digits)
        readonly static Regex VariableRegex = new ("[a-zA-Z]+[0-9]+", RegexOptions.IgnoreCase);


        public delegate int Lookup(String variable_name);

        /// <summary>
        /// This function takes in a string arithmetic expression and evaluates it.
        /// </summary>
        /// <param name="expression"> a string expression including (,),+,-,*,/,int,or string variables
        /// i.e. (5*2)/6+X
        /// </param>
        /// <param name="variableEvaluator"> a delegate used for looking up input string variables</param>
        /// <returns> the result of the infix expression</returns>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            if(expression == null) throw new ArgumentException("Null expression error.");
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            //iterate through the substrings and perform operations accordingly
            foreach (string token in substrings)
            {
                if (token.Equals(" ") || token.Equals("")) continue;

                string eqnPart = token.Trim();

                int val = 0;
                if (int.TryParse(eqnPart, out val))
                {
                    if (val < 0) throw new ArgumentException("Negative numbers not allowed.");
                    IntegerOrVariable(val);
                }
                else if (VariableRegex.IsMatch(eqnPart))
                {
                    IntegerOrVariable(variableEvaluator(eqnPart));
                }
                else
                {
                    switch(eqnPart)
                    {
                        case "+":
                            AddOrSubtractOperator(eqnPart);
                            break;
                        case "-":
                            AddOrSubtractOperator(eqnPart);
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
                            RightParenthesis(eqnPart);
                            break;

                        default:
                            throw new ArgumentException("Invalid expression input.");
                    }

                }
            }

            if (Operator.Count == 0 && Value.Count > 1)
                throw new ArgumentException("Value stack has too many values.");
            else if (Operator.Count == 0)
                return Value.Pop();
            else if (Value.Count < 2)
                throw new ArgumentException("Value stack doesn't have enough values.");

            else
            {
                int right = Value.Pop();
                int left = Value.Pop();
                return AddOrSubtract(left, right);
            }

        }

        /// <summary>
        /// Method to handle if the string is + or -
        /// </summary>
        /// <param name="s"> string s the operator</param>
        private static void AddOrSubtractOperator(String s)
        {
            if(!Operator.HasOnTop("+")||!Operator.HasOnTop("-")||Operator.IsEmpty())
                Operator.Push(s);
            else if (Value.Count >= 2) 
            { 
                int right = Value.Pop();
                int left = Value.Pop();
                Value.Push(AddOrSubtract(left, right));
            }
            else throw new ArgumentException("Invalid input: Unable to perform addition or subtraction.");
        }

        /// <summary>
        /// Helper method for addition or subtraction problems
        /// </summary>
        /// <param name="right">right integer in expression</param>
        /// <param name="left">left integer in expression</param>
        /// <returns>returns the sum of the right and left integers</returns>
        private static int AddOrSubtract(int left, int right)
        {
            int sum = 0;
            string op = Operator.Pop();
            if (op.Equals("+"))
                sum = left + right;
            else
                sum = left - right;
            return sum;
        }

        /// <summary>
        /// Method to handle integer inputs or variable inputs after lookup
        /// </summary>
        /// <param name="val">the integer input</param>
        /// <exception cref="ArgumentException">Throws argument exception if
        /// syntax is invalid or dividing by 0</exception>
        private static void IntegerOrVariable(int val)
        {
           if(Operator.HasOnTop("*") || Operator.HasOnTop("/"))
            {
                if (Value.IsEmpty()) throw new ArgumentException("No values to multiply or divide, invalid input.");
                int right = val;
                int left = Value.Pop();
                Value.Push(MultiplyOrDivide(left, right));
            }
           else
                Value.Push(val);
        }

        /// <summary>
        /// Method to handle if the string is ")"
        /// </summary>
        /// <param name="s"> string s the operator</param>
        /// <exception cref="ArgumentException"> Throws if input is invalid</exception>
        private static void RightParenthesis(String s)
        {
            if(Operator.HasOnTop("+") || Operator.HasOnTop("-"))
            {
                if(Value.Count < 2) throw new ArgumentException("Invalid input, can't perform addition or subtraction.");

                int right = Value.Pop();
                int left = Value.Pop();
                Value.Push(AddOrSubtract(left, right));
            }

            if (Operator.Peek().Equals("("))
                Operator.Pop();
            else
                throw new ArgumentException("Expression wasn't opened with a parenthesis '('.");

            if (Operator.HasOnTop("*") || Operator.HasOnTop("/"))
            {
                int right = Value.Pop();
                int left = Value.Pop();
                Value.Push(MultiplyOrDivide(left, right));
            }
        }

        /// <summary>
        /// Helper method to perform multiplication or division operations
        /// </summary>
        /// <param name="left"> the left integer</param>
        /// <param name="right"> the right integer </param>
        /// <returns>the product or quotient of the operation</returns>
        /// <exception cref="ArgumentException"> Throws if dividing by zero </exception>
        private static int MultiplyOrDivide(int left, int right)
        {
            String op = Operator.Pop();
            if (op.Equals("*"))
                return left * right;
            else
            {
                if (right == 0) throw new ArgumentException("Divide by zero error.");
                return left / right;
            }
        }
    }

    /// <summary>
    /// Extension class for the evaluator class
    /// </summary>
    public static class EvaluatorExtensions
    {
        /// <summary>
        /// Extension for checking if a value is present at the top of the stack
        /// </summary>
        /// <typeparam name="T"> int or string value in stack</typeparam>
        /// <param name="stack">stack to be checked</param>
        /// <param name="value">the value being checked</param>
        /// <returns> true if the value is on top, false otherwise</returns>
        public static bool HasOnTop<T>(this Stack<T> stack, T value)
        {
            if (!stack.IsEmpty() && stack.Peek().Equals(value))
                return true;
            return false;

        }

        /// <summary>
        /// Extension to check if the stack is empty
        /// </summary>
        /// <typeparam name="T">Type in the stack</typeparam>
        /// <param name="stack">Stack to be checked</param>
        /// <returns>true if stack is empty, false otherwise </returns>
        public static bool IsEmpty<T>(this Stack<T> stack)
        {
            return stack.Count == 0;
        }
    }
}