using Spreadsheet;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private readonly static Regex VariableRegex = new(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*");
        private Dictionary<String, Cell> nonEmptyCells;
        private DependencyGraph cellDependencies;

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet()
        {
            nonEmptyCells = new Dictionary<String, Cell>();
            cellDependencies = new DependencyGraph();
        }

        /// <summary>
        /// Returns the contents of the cell. 
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <returns>The contents of the cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        public override object GetCellContents(string name)
        {
            if (!VariableRegex.IsMatch(name) || !nonEmptyCells.ContainsKey(name) || name is null)
                throw new InvalidNameException();

            return nonEmptyCells[name].Contents;
        }

        /// <summary>
        /// Returns an IEnumberable object that contains a list of all 
        /// non-empty cells.
        /// </summary>
        /// <returns>Names of Non Empty cells</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return nonEmptyCells.Keys;
        }


        /// <summary>
        /// Sets the contents of the cell to a number. 
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="number">the double value to set the cell to</param>
        /// <returns>
        /// A list of the cell, and all the cells which directly or indirectly depend 
        /// on the named cell
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        public override IList<string> SetCellContents(string name, double number)
        {
            if (!VariableRegex.IsMatch(name) || name is null)
                throw new InvalidNameException();

            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].Contents = number;
                cellDependencies.ReplaceDependees(name, new List<string>());
            }
            else
                nonEmptyCells.Add(name, new Cell(name, number));

            IEnumerable<string> EnumDependents = GetCellsToRecalculate(name);
            IList<string> ListDependents = GetCellsToRecalculate(name).ToList();
            return ListDependents;
        }

        /// <summary>
        /// Sets the contents of the cell to some text. 
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="text">the text to be displayed in the cell</param>
        /// <returns>
        /// A list of the cell, and all the cells which directly or indirectly depend 
        /// on the named cell
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">If the text is null, throws ArgumentNullException</exception>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        public override IList<string> SetCellContents(string name, string text)
        {
            if (!VariableRegex.IsMatch(name) || name is null)
                throw new InvalidNameException();
            if (text is null)
                throw new ArgumentNullException();

            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].Contents = text;
                cellDependencies.ReplaceDependees(name, new List<string>());
            }
            else
                nonEmptyCells.Add(name, new Cell(name, text));

            IEnumerable<string> EnumDependents = GetCellsToRecalculate(name);
            IList<string> ListDependents = GetCellsToRecalculate(name).ToList();
            return ListDependents;
        }

        /// <summary>
        /// Sets the contents of the cell to a formula. 
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="formula">the formula to be displayed in the cell</param>
        /// <returns>
        /// A list of the cell, and all the cells which directly or indirectly depend 
        /// on the named cell
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">If the Formula is null, throws ArgumentNullException</exception>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        /// <exception cref="CircularException">If setting the cell creates a circular dependency, throws CircularException</exception>
        public override IList<string> SetCellContents(string name, Formula formula)
        {
            if (!VariableRegex.IsMatch(name) || name is null)
                throw new InvalidNameException();
            if (formula is null)
                throw new ArgumentNullException();

            if (nonEmptyCells.ContainsKey(name))
            {
                IEnumerable<string> variables = new List<string>();
                foreach (string variable in variables)
                {
                    if (cellDependencies.HasDependents(variable))
                        throw new CircularException();
                    cellDependencies.AddDependency(name, variable);
                }
                nonEmptyCells[name].Contents = formula;
            }
            else
            {
                IEnumerable<string> variables = new List<string>();
                foreach (string variable in variables)
                {
                    if (cellDependencies.HasDependents(variable))
                        throw new CircularException();
                    cellDependencies.AddDependency(name, variable);
                }
                nonEmptyCells.Add(name, new Cell(name, formula));
            }

            IEnumerable<string> EnumDependents = GetCellsToRecalculate(name);
            IList<string> ListDependents = GetCellsToRecalculate(name).ToList();
            return ListDependents;
        }

        /// <summary>
        /// Returns an IEnumberable list of all the direct dependents of the named cell.
        /// </summary>
        /// <param name="name">the name of the cell which dependents will be returned</param>
        /// <returns>an enumeration of the dependents of the named cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (!VariableRegex.IsMatch(name) || name == "")
                throw new InvalidNameException();

            return cellDependencies.GetDependents(name);
        }

        private void SetStringOrText(object name)
        {
            if (!(name is double) || !(name is string))
                return;


        }
    }
}