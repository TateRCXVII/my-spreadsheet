using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet
{
    internal class Cell
    {
        //Name of the cell
        private string _name;

        //String, double (including result from Formula.evaluate), or formula error
        //displayed on screen without selection
        private object _value;

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
            _value = "";
            _contents = "";
        }

        /// <summary>
        /// Creates a cell with formula as contents and its result as its value.
        /// </summary>
        public Cell(string name, Formula formula)
        {
            _name = name;
            _value = formula;
            _contents = formula;
        }

        /// <summary>
        /// Creates a cell with a number
        /// </summary>
        public Cell(string name, double number)
        {
            _name = name;
            _value = number;
            _contents = number;
        }

        /// <summary>
        /// Creates a cell with text
        /// </summary>
        public Cell(string name, string text)
        {
            _name = name;
            _value = text;
            _contents = text;
        }

        #region Properties
        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public object Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        #endregion
    }
}
