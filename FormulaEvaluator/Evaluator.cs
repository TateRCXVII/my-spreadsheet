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
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            //iterate through the substrings and perform operations accordingly
            foreach (string s in substrings)
            {

                //if it's an integer
                int val = 0;
                if (int.TryParse(s, out val))
                {
                        IntegerOrVariable(val);
                }
                else if (VariableRegex.IsMatch(s))
                {
                    IntegerOrVariable(variableEvaluator(s));
                }
                else
                {
                    switch(s)
                    {
                        case "+":
                            AddOrSubtract(s);
                            break;
                        case "-":
                            AddOrSubtract(s);
                            break;
                        case "*":
                            Operator.Push(s);
                            break;
                        case "/":
                            Operator.Push(s);
                            break;
                        case "(":
                            Operator.Push(s);
                            break;
                        case ")":
                            RightParenthesis(s);
                            break;
                    }

                }
            }
            if (Operator.Count == 0 && Value.Count > 1)
                throw new ArgumentException();
            else if (Operator.Count == 0)
                return Value.Pop();
            else if (Value.Count < 2)
                throw new ArgumentException();
            //perform + or - operation
            else
            {
                int num1 = Value.Pop();
                int num2 = Value.Pop();
                int sum = 0;
                string op = Operator.Pop();
                if (op.Equals("+"))
                    sum = num2 + num1;
                else
                    sum = num2 - num1;
                return sum;
            }

        }

        /// <summary>
        /// Method to handle if the string is + or -
        /// </summary>
        /// <param name="s"> string s the operator</param>
        private static void AddOrSubtract(String s)
        {
            //TODO: Improve readability of this statement
            if(Operator.Count == 0 || !Operator.Peek().Equals("+") || !Operator.Peek().Equals("-"))
                Operator.Push(s);
            else 
            { 
                int num1 = Value.Pop();
                int num2 = Value.Pop();
                int sum = 0;
                string op = Operator.Pop();
                if(op.Equals("+"))
                    sum = num2 + num1;
                else
                    sum = num2 - num1;
                Value.Push(sum);
            }
        }

        private static void IntegerOrVariable(int val)
        {
            if (Operator.Count == 0)
            {
                Value.Push(val);
            }
            // if multiplication or division is needed, pop the value stack 
            //and perform the operation on the current int and popped val int 
            else if (Operator.Peek().Equals("*"))
            {
                Operator.Pop();
                int num1 = Value.Pop();
                int result = val * num1;
                Value.Push(result);
            }
            else if (Operator.Peek().Equals("/"))
            {
                Operator.Pop();
                int num1 = Value.Pop();
                if(val == 0) throw new ArgumentException();
                int result = num1 / val;
                Value.Push(result);
            }
            else
                Value.Push(val);
        }

        /// <summary>
        /// Method to handle if the string is ")"
        /// </summary>
        /// <param name="s"> string s the operator</param>
        /// <exception cref="ArgumentException"></exception>
        private static void RightParenthesis(String s)
        {
            //if + or -, pop the value stack twice and perform the operation
            //recursively using Evaluate
            if(Operator.Peek().Equals("+") || Operator.Peek().Equals("-"))
            {
                //todo switch left and right
                int right = Value.Pop();
                int left = Value.Pop();
                String op = Operator.Pop();
                if (op.Equals("+"))
                    Value.Push(right + left);
                else
                    Value.Push(right - left);
                //String expression = left + op + right;
                //TODO: Test with variables... 
                //Value.Push(Evaluate(expression, null));

            }
            //if statement needed to handle exception
            //if the ( doesn't exist, the input is invalid
            if (Operator.Peek().Equals("("))
                Operator.Pop();
            else
                throw new ArgumentException();
            //ensure there is somehting left in the op stack to check/evaluate
            if (Operator.Count > 0)
            {
                if (Operator.Peek().Equals("*") || Operator.Peek().Equals("/"))
                {
                    int right = Value.Pop();
                    int left = Value.Pop();
                    String op = Operator.Pop();
                    if(op.Equals("*"))
                        Value.Push(right*left);
                    else
                        Value.Push(right/left);
                    //String expression = left + op + right;
                    //TODO: Test with variables
                   // Value.Push(Evaluate(expression, null));

                }
            }
            //else throw new ArgumentException();
        }
    }
}