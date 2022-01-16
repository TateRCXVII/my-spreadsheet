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
        private Stack<string> Operator = new Stack<string>();
        private Stack<int> Value = new Stack<int>();


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
            return 0;
        }
    }
}