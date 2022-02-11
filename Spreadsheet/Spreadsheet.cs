using Spreadsheet;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        //private readonly static Regex VariableRegex = new Regex("^[a-zA-Z_]+[0-9]*");
        private readonly static Regex VariableRegex = new Regex(@"^[a-zA-Z_](?:[a-zA-Z_]|\d)*");
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

        ///<inheritdoc/>
        /// <param name="name">the name of the cell</param>
        /// <returns>The contents of the cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        public override object GetCellContents(string name)
        {
            if (!VariableRegex.IsMatch(name) || !nonEmptyCells.ContainsKey(name))
                throw new InvalidNameException();

            return nonEmptyCells[name].Contents;
        }

        ///<inheritdoc/>
        /// <returns>Names of Non Empty cells</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return nonEmptyCells.Keys;
        }

        ///<inheritdoc/>
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
            if (!VariableRegex.IsMatch(name))
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

        ///<inheritdoc/>
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
            if (!VariableRegex.IsMatch(name))
                throw new InvalidNameException();

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

        ///<inheritdoc/>
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
            if (!VariableRegex.IsMatch(name))
                throw new InvalidNameException();
            if (formula is null)
                throw new ArgumentNullException();

            bool nameExists;//for catching and resetting the circular dependencies
            Object previousContents = "";

            if (nonEmptyCells.ContainsKey(name))
            {
                previousContents = nonEmptyCells[name].Contents;
                cellDependencies.ReplaceDependees(name, formula.GetVariables());
                nonEmptyCells[name].Contents = formula;
                nameExists = true;
            }
            else
            {
                nonEmptyCells.Add(name, new Cell(name, formula));
                IEnumerable<string> variables = formula.GetVariables();
                cellDependencies.ReplaceDependees(name, formula.GetVariables());
                nameExists = false;
            }
            try
            {
                IList<string> ListDependents = GetCellsToRecalculate(name).ToList();
                return ListDependents;
            }
            //Code help from TA office hrs
            catch (CircularException e)
            {
                if (nameExists)
                    nonEmptyCells[name].Contents = previousContents;
                else
                    nonEmptyCells.Remove(name);
                throw e;
            }
        }

        ///<inheritdoc/>
        /// <param name="name">the name of the cell which dependents will be returned</param>
        /// <returns>an enumeration of the dependents of the named cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return cellDependencies.GetDependents(name);
        }
    }
}