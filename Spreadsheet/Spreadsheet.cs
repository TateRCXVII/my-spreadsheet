using Spreadsheet;
using SpreadsheetUtilities;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, Cell> nonEmptyCells

        /// <summary>
        /// Creates an empty spreadsheet
        /// </summary>
        public Spreadsheet()
        {

        }

        /// <summary>
        /// Returns the contents of the cell. 
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <returns>The contents of the cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        public override object GetCellContents(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an IEnumberable object that contains a list of all 
        /// non-empty cells.
        /// </summary>
        /// <returns>Names of Non Empty cells</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an IEnumberable list of all the direct dependents of the named cell.
        /// </summary>
        /// <param name="name">the name of the cell which dependents will be returned</param>
        /// <returns>an enumeration of the dependents of the named cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }
    }
}