using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private readonly static Regex VariableRegex = new Regex(@"^[a-zA-Z](?:[a-zA-Z]|\d)*"); //removed _ as valid var input
        private Dictionary<String, Cell> nonEmptyCells;
        private DependencyGraph cellDependencies;
        //indicates if the sheet has been changed and not saved
        private bool changed;
        private string version;
        private readonly Func<string, string> normalize;
        private readonly Func<string, bool> isValid;

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet() //TODO: Remove (?)
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
            if (!VariableRegex.IsMatch(name))
                throw new InvalidNameException();
            if (!nonEmptyCells.ContainsKey(name))
                return "";

            return nonEmptyCells[name].Contents;
        }

        //TODO: Implement
        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        //TODO: Implement
        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        //TODO: Implement
        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        //TODO: Implement
        public override void Save(string filename)
        {
            //TODO: manage the changed variable here (i.e. set back to false)
            throw new NotImplementedException();
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
        protected override IList<string> SetCellContents(string name, double number)
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
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (!VariableRegex.IsMatch(name))
                throw new InvalidNameException();

            if (text == "")
                return new List<string>();

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
        protected override IList<string> SetCellContents(string name, Formula formula)
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

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            throw new NotImplementedException();
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



    /// <summary>
    /// Represents a cell in the spreadsheet.
    /// Cells have a name, contents, and a value.
    /// Contents are displayed when the cell is in edit mode.
    /// Value is displayed on screen.
    /// The name must fit the permitted variable name of the overall project.
    /// </summary>
    public class Cell
    {
        //Name of the cell
        private string _name;

        //String, double, or formula
        //empty string "" means empty cell
        //Displayed when the cell is selected (double clicked)
        //new SS = ""
        private object _contents;

        /// <summary>
        /// Creates an empty cell
        /// </summary>
        public Cell(string name)
        {
            _name = name;
            _contents = "";
        }

        /// <summary>
        /// Creates a cell with formula as contents and its result as its value.
        /// </summary>
        public Cell(string name, Formula formula)
        {
            _name = name;
            _contents = formula;
        }

        /// <summary>
        /// Creates a cell with a number
        /// </summary>
        public Cell(string name, double number)
        {
            _name = name;
            _contents = number;
        }

        /// <summary>
        /// Creates a cell with text
        /// </summary>
        public Cell(string name, string text)
        {
            _name = name;
            _contents = text;
        }

        #region Properties
        public string Name
        {
            get { return _name; }
        }

        public object Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        #endregion
    }
}