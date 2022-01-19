// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;

Console.WriteLine("Hello, World!");
if (Evaluator.Evaluate("25/5", null) == 5) Console.WriteLine("Happy Day!25/5 works");
if (Evaluator.Evaluate("5*5", null) == 25) Console.WriteLine("Happy Day!5*5 works");
if (Evaluator.Evaluate("20 /5", null) == 4) Console.WriteLine("Happy Day!20 /5 works");
if (Evaluator.Evaluate("(25/5)+3", null) == 8) Console.WriteLine("Happy Day!(25/5)+3 works");
if (Evaluator.Evaluate("((25/5)+3)*5", null) == 40) Console.WriteLine("Happy Day!((25/5)+3)*5 works");
if (Evaluator.Evaluate("(5+3) * 8 - 100", null) == -36) Console.WriteLine("Happy Day!(5+3) * 8 - 100 works");
if (Evaluator.Evaluate("(5+3) * 8 - ABCD1", VariableEvaluator) == -36) Console.WriteLine("Happy Day!(5+3) * 8 - ABCD1 works");

static int VariableEvaluator(string s)
{
    if (s.Equals(" ABCD1"))
        return 100;
    else
        return 50;
}
