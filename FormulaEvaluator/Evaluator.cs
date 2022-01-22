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
    /// - Evaluate method to evaluate expressions.
    /// - A series of helper methods to perform said operations.
    /// - Extensions to the Stack class.
    /// 
    /// This namespace and class is a formula evaluator using RegEx and delegates. The class contains a
    /// method, Evaluate, which evaluates an input string expression ("5+5/10") using standard order of 
    /// operations. It can also take variable inputs in the expression ("5+X1/10"), however variables must
    /// be one or more letters followed by one or more numbers. The variable lookup is handled with an input delegate 
    /// function passed as a parameter.
    /// </summary>
    public class Evaluator
    {
        //Regex object to check if a token is a variable (any # of letters followed by any # of digits)
        readonly static Regex VariableRegex = new ("[a-zA-Z]+[0-9]+", RegexOptions.IgnoreCase);


        public delegate int Lookup(String variable_name);

        /// <summary>
        /// This function takes in a string arithmetic expression and evaluates it. Variables are possible if
        /// the method is provided a lookup delegate function defined by the user of this method.
        /// </summary>
        /// <param name="expression"> a string expression including (,),+,-,*,/,int,or variables
        /// i.e. (5*2)/6+X1
        /// Variables are defined as at least one letter followed by at least one number.
        /// i.e. ABC123 is allowed while 1A3B is not.
        /// </param>
        /// <param name="variableEvaluator"> a delegate used for looking up input string variables</param>
        /// <returns> the integer result of the infix expression</returns>
        /// <exception cref="ArgumentException"> 
        /// If the expression is invalid or the operation is invalid, 
        /// an argument excpetion will be thrown
        /// </exception>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {

        Stack<string> Operator = new Stack<string>();
        Stack<int> Value = new Stack<int>();

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
                    if (val < 0)
                        throw new ArgumentException("Negative numbers not allowed.");

                    IntegerOrVariable(val, Operator, Value);
                }
                else if (VariableRegex.IsMatch(eqnPart))
                    IntegerOrVariable(variableEvaluator(eqnPart), Operator, Value);
                else
                {
                    switch(eqnPart)
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

                        default:
                            throw new ArgumentException("Invalid expression input.");
                    }

                }
            }

            if (Operator.Count == 0 && Value.Count > 1)
                throw new ArgumentException("Value stack has too many values .");
            else if (Operator.Count == 0)
                return Value.Pop();
            else if (Value.Count < 2)
                throw new ArgumentException("Value stack doesn't have enough values.");
            else if (Operator.Count > 1)
                throw new ArgumentException("Check for negative numbers.");
            else
                return AddOrSubtract(Value, Operator);

        }

        /// <summary>
        /// Helper method to handle when the + or - operators are encountered
        /// </summary>
        /// <param name="s">Current operator (either + or -)</param>
        /// <param name="Operator">Stack of operators</param>
        /// <param name="Value">Stack of values</param>
        private static void AddOrSubtractOperator(String s, Stack<string> Operator, Stack<int> Value)
        {
            if (Operator.HasOnTop("+") || Operator.HasOnTop("-"))
                Value.Push(AddOrSubtract(Value, Operator));

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
        private static int AddOrSubtract(Stack<int> Value, Stack<string> Operator)
        {
            if (Value.Count < 2)
                throw new ArgumentException("Can't perform operation: Not enough integers");

            int right = Value.Pop();
            int left = Value.Pop();
            int sum = 0;
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
        private static void IntegerOrVariable(int val, Stack<string> Operator, Stack<int> Value)
        {
           if(Operator.HasOnTop("*") || Operator.HasOnTop("/"))
            {
                if (Value.IsEmpty())
                    throw new ArgumentException("No values to multiply or divide, invalid input.");

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
        /// <exception cref="ArgumentException">
        /// If there aren't enough values in the value stack or the input has invalid syntax,
        /// an argument exception will be thrown.
        /// </exception>
        private static void RightParenthesis(Stack<string> Operator, Stack<int> Value)
        {
            if(Operator.HasOnTop("+") || Operator.HasOnTop("-"))
            {
                if (Value.Count < 2)
                    throw new ArgumentException("Invalid input, can't perform addition or subtraction.");
                Value.Push(AddOrSubtract(Value, Operator));
            }

            if (Operator.HasOnTop("("))
                Operator.Pop();
            else
                throw new ArgumentException("Expression wasn't opened with a parenthesis '('.");

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
        private static int MultiplyOrDivide(Stack<string> Operator, Stack<int> Value)
        {
            if (Value.Count < 2)
                throw new ArgumentException("Not enough integers to perform operation. Check for negative numbers.");

            int right = Value.Pop();
            int left = Value.Pop();
            String op = Operator.Pop();
            if (op.Equals("*"))
                return left * right;
            else
            {
                if (right == 0)
                {
                    throw new ArgumentException("Divide by zero error.");
                }
                return left / right;
            }
        }
    }

    /// <summary>
    /// Extension class for the evaluator class
    /// 
    /// Contains extensions to check if stack is empty and to check if an operator is on top.
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