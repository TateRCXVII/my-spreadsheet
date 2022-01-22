// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;

//simple tests
if (Evaluator.Evaluate("25/5", null) == 5) Console.WriteLine("Happy Day!25/5 works");
if (Evaluator.Evaluate("5*5", null) == 25) Console.WriteLine("Happy Day!5*5 works");
if (Evaluator.Evaluate("20 /5", null) == 4) Console.WriteLine("Happy Day!20 /5 works");
if (Evaluator.Evaluate("(25/5)+3", null) == 8) Console.WriteLine("Happy Day!(25/5)+3 works");
if (Evaluator.Evaluate("((25/5)+3)*5", null) == 40) Console.WriteLine("Happy Day!((25/5)+3)*5 works");
if (Evaluator.Evaluate("(5+3) * 8 - 100", null) == -36) Console.WriteLine("Happy Day!(5+3) * 8 - 100 works");
if (Evaluator.Evaluate("(120/((5+3) * 8)) - 100 * B8923", VariableEvaluator) == -199) Console.WriteLine("Happy Day!(120/((5+3) * 8)) - 100 * B8923 works");
if (Evaluator.Evaluate("(5+3) * 8 - ABCD1", VariableEvaluator) == -36) Console.WriteLine("Happy Day!(5+3) * 8 - ABCD1 works");

//more in depth and revised test suite
TestEvaluate("(120/((5+3) * 8)) - 100 * B8923", VariableEvaluator, -199, "complex arithmatic with a variable:");
TestEvaluate("((((5+5))))", null, 10, "lots of parentheses:");
TestEvaluateException("(10*10)/0", null, 0, "divide by zero exception test: ");
TestEvaluateException("(10*10)/charlie1", VariableEvaluator, 0, "divide by zero with a variable exception test: ");
TestEvaluate("((3))", null, 3, "single integer performance: ");
TestEvaluateException(null, null, 0, "Null expression error test: ");
TestEvaluateException("3+3)/200", null, 0, "Invalid input, incorrect parenthesis: ");
TestEvaluateException(" 3 + -5", null, 2, "Negative number exception test: ");
TestEvaluateException("8*(-3)", null, 0, "Negative multiplication test: ");
TestEvaluate("()5", null, 5, "Random parentheses test: "); //is this allowed?
TestEvaluateException("5/1c4f", VariableEvaluator, 0, "Bad variable exception test: ");
TestEvaluate("100-50+ABCD1", VariableEvaluator, 150, "Complex equation with no spaces: ");
TestEvaluate("1(1)++1", VariableEvaluator, 3, "Piazza post test: ");
TestEvaluate("XD12 + DX21", VariableEvaluator, 21, "Two variables test: ");
TestEvaluate("XD12 * 0", VariableEvaluator, 0, "Two variables multiply 0 test: ");
TestEvaluateException("#$%#$%@", null, 1, "Invalid input test:");


/// <summary>
/// Variable lookup evaluator for testing variables
/// </summary>
static int VariableEvaluator(string s)
{
    if (s.Equals("ABCD1"))
        return 100;
    else if (s.Equals("B8923"))
        return 2;
    else if (s.Equals("charlie1"))
        return 0;
    else if (s.Equals("XD12"))
        return 9;
    else if (s.Equals("DX21"))
        return 12;
    else throw new ArgumentException("Invalid variable lookup.");
}

/// <summary>
/// If a message is output, the expression wasn't evaluated correctly
/// </summary>
void TestEvaluate(string expression, Evaluator.Lookup var, int expected, string description)
{
    try
    {
       int actual = Evaluator.Evaluate(expression, var);
        if (actual != expected)
            Console.WriteLine(description + " " + expression + " error.");
    } catch(ArgumentException e)
    {
       Console.WriteLine(e.Message + " Invalid input or divide by 0");
    }
}

/// <summary>
/// If a message is output, the exception didn't occur when it needed to
/// </summary>
void TestEvaluateException(string expression, Evaluator.Lookup var, int expected, string description)
{
    try
    {
        int actual = Evaluator.Evaluate(expression, var);
        Console.WriteLine(description + " " + expression + " error.");
    }
    catch (ArgumentException e)
    {
        //Console.WriteLine(e.Message + description);
    }
}
