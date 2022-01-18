﻿using System.Text.RegularExpressions;

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


        public delegate int Lookup(String variable_name);

        /// <summary>
        /// This function takes in a string arithmetic expression and evaluates it.
        /// </summary>
        /// <param name="expression"> a string expression including (,),+,-,*,/,int,or string variables
        /// i.e. (5*2)/6+X
        /// </param>
        /// <param name="variableEvaluator"> a delegate used for looking up input string variables</param>
        /// <returns></returns>
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
                    if(Operator.Count == 0)
                    {
                        Value.Push(val);
                    }
                    // if multiplication or division is needed, pop the value stack 
                    //and perform the operation on the current int and popped val int 
                    else if(Operator.Peek() == "*")
                    {
                        Operator.Pop();
                        int num1 = Value.Pop();
                        int result = val * num1;
                        Value.Push(result);
                    } 
                    else if (Operator.Peek() == "/")
                    {
                        Operator.Pop();
                        int num1 = Value.Pop();
                        int result = val / num1;
                        Value.Push(result);
                    }
                    else
                        Value.Push(val);
                }
                else if (false)
                {
                    //TODO: Check if it's a variable
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
                            //Operator.Push(s);
                            break;
                    }

                }
            }
            if(Operator.Count == 0)
                return Value.Pop();
            else
            {
                int num1 = Value.Pop();
                int num2 = Value.Pop();
                int sum = 0;
                string op = Operator.Pop();
                if (op.Equals("+"))
                    sum = num1 + num2;
                else
                    sum = num1 - num2;
                return sum;
            }

        }

        /// <summary>
        /// Method to handle if the string is + or -
        /// </summary>
        /// <param name="s"> string s the operator</param>
        private static void AddOrSubtract(String s)
        {
            if(Operator.Count == 0 || !Operator.Peek().Equals("+") || !Operator.Peek().Equals("-"))
                Operator.Push(s);
            else 
            { 
                int num1 = Value.Pop();
                int num2 = Value.Pop();
                int sum = 0;
                string op = Operator.Pop();
                if(op.Equals("+"))
                    sum = num1 + num2;
                else
                    sum = num1 - num2; //TODO: make sure this is the right order!
                Value.Push(sum);
            }
        }
    }
}