using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;

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
        /// Your zero-argument constructor should create an empty spreadsheet that
        /// imposes no extra validity conditions, normalizes every cell name to itself, 
        /// and use the name "default" as the version.
        /// </summary>
        public Spreadsheet() :
            base(s => true, s => s, "default")
        {
            this.normalize = s => s;
            this.isValid = s => true;
            this.version = "default";
            this.changed = false;
            this.nonEmptyCells = new Dictionary<String, Cell>();
            this.cellDependencies = new DependencyGraph();
        }

        /// <summary>
        /// You should add a three-argument constructor to the Spreadsheet class. 
        /// Just like the zero-argument constructor, it should create an empty spreadsheet. 
        /// However, it should allow the user to provide a validity delegate (first parameter), 
        /// a normalization delegate (second parameter), and a version (third parameter).
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) :
            base(isValid, normalize, version)
        {
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
            this.changed = false;
            this.nonEmptyCells = new Dictionary<String, Cell>();
            this.cellDependencies = new DependencyGraph();
        }

        /// <summary>
        /// You should add a four-argument constructor to the Spreadsheet class. 
        /// It should allow the user to provide a string representing a path to a file 
        /// (first parameter), a validity delegate (second parameter), a normalization delegate
        /// (third parameter), and a version (fourth parameter). It should read a saved spreadsheet 
        /// from the file (see the Save method) and use it to construct a new spreadsheet. 
        /// The new spreadsheet should use the provided validity delegate, normalization delegate, 
        /// and version. Do not try to implement loading from file until after we have discussed XML in class. 
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) :
            base(isValid, normalize, version)
        {
            //TODO: handle loading from filepath
            this.isValid = isValid;
            this.normalize = normalize;
            this.version = version;
            this.changed = false;
            this.nonEmptyCells = new Dictionary<String, Cell>();
            this.cellDependencies = new DependencyGraph();
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true; //Idea from a discussion group
                using (XmlReader reader = XmlReader.Create(filepath, xmlReaderSettings))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            string lowerName = reader.Name.ToLower();
                            string cellName = "";
                            string contents = "";
                            switch (lowerName)
                            {
                                case "spreadsheet":
                                case "cell":
                                case "version":
                                    break;
                                case "name":
                                    reader.Read();
                                    cellName = reader.ReadContentAsString();
                                    cellName.Trim();
                                    break;
                                case "contents":
                                    reader.Read();
                                    contents = reader.ReadContentAsString();
                                    contents.Trim();
                                    SetContentsOfCell(cellName, contents);
                                    break;
                                default:
                                    throw new Exception("Invalid spreadsheet input.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException(ex.Message);
            }
        }

        ///<inheritdoc/>
        /// <param name="name">the name of the cell</param>
        /// <returns>The contents of the cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid or empty, throws InvalidNameException</exception>
        public override object GetCellContents(string name)
        {
            name = this.normalize(name);
            if (!VariableRegex.IsMatch(name))
                throw new InvalidNameException();
            if (!nonEmptyCells.ContainsKey(name))
                return "";

            return nonEmptyCells[name].Contents;
        }

        /// <inheritdoc/>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            name = this.normalize(name);
            if (Double.TryParse(content, out double value))
                return this.SetCellContents(name, value);
            else if (content.StartsWith("="))
                return this.SetCellContents(name, new Formula(content.Substring(1), this.normalize, this.isValid));
            else
                return this.SetCellContents(name, content);
        }

        /// <inheritdoc/>
        public override bool Changed
        {
            get => changed;
            protected set => changed = value;
        }

        /// <inheritdoc/>
        /// <param name="name">the name of the cell (will be normalized)</param>
        /// <returns>the value of the cell</returns>
        /// <exception cref="InvalidNameException">If the name is invalid, throws exception</exception>
        public override object GetCellValue(string name)
        {
            name = this.normalize(name);
            if (invalidVariable(name))
                throw new InvalidNameException();
            if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].Value;
            else
                return "";
        }

        /// <inheritdoc/>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.Name.Equals("spreadsheet"))
                            return reader.GetAttribute("version");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Unknown error happened when reading the file:" + ex.Message);
            }
            throw new SpreadsheetReadWriteException("Version not found in spreadsheet XML file");
        }

        //TODO: Implement
        //If the version of the saved spreadsheet does not match the version parameter provided to the constructor
        //If any of the names contained in the saved spreadsheet are invalid
        //If any invalid formulas or circular dependencies are encountered
        //If there are any problems opening, reading, or closing the file
        public override void Save(string filename)
        {
            try
            {
                //for formatting the xml correctly
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                XmlWriter xmlWriter = XmlWriter.Create(filename, settings);
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("spreadsheet");
                xmlWriter.WriteAttributeString("version", this.version);
                foreach (KeyValuePair<string, Cell> cells in nonEmptyCells)
                {
                    xmlWriter.WriteStartElement("cell");
                    xmlWriter.WriteElementString("name", cells.Key);
                    //value pertains to the value in key-value pair. The contents is the content in the cell
                    if (cells.Value.Contents is Formula)
                    {
                        string equalsFormula = "=" + cells.Value.Contents.ToString();
                        xmlWriter.WriteElementString("contents", equalsFormula);
                    }
                    else
                        xmlWriter.WriteElementString("contents", cells.Value.Contents.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Dispose();
                changed = false;
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Error while writing spreadsheet to XML: " + ex.Message);
            }
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
            name = this.normalize(name);
            if (invalidVariable(name))
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
            reevaluateCells(ListDependents);
            changed = true;
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
            name = this.normalize(name);
            if (invalidVariable(name))
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
            reevaluateCells(ListDependents);
            changed = true;
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
            name = this.normalize(name);
            if (invalidVariable(name))
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
                nonEmptyCells.Add(name, new Cell(name, formula, lookup));
                IEnumerable<string> variables = formula.GetVariables();
                cellDependencies.ReplaceDependees(name, formula.GetVariables());
                nameExists = false;
            }
            try
            {
                IList<string> ListDependents = GetCellsToRecalculate(name).ToList();
                reevaluateCells(ListDependents);
                changed = true;
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

        /// <summary>
        /// Helper method to determine if a variable is valid on both validity requirements.
        /// </summary>
        /// <param name="name">name of cell</param>
        /// <returns>true if the variable is invalid, false otherwise</returns>
        private bool invalidVariable(string name)
        {
            return !VariableRegex.IsMatch(name) && !this.isValid(name);
        }

        /// <summary>
        /// A helper method to help with the lookup of cell names (variables) 
        /// to evaluate the functions
        /// </summary>
        /// <param name="name">looked-up cell name</param>
        /// <returns>the double value of the referenced cell name</returns>
        /// <exception cref="ArgumentException">throws an argument exception if the looked up value is not a double</exception>
        private double lookup(string name)
        {
            if (nonEmptyCells[name].Value is not Double)
                throw new ArgumentException();
            else return (Double)nonEmptyCells[name].Value;
        }

        /// <summary>
        /// Helper method to re evaluate cells if a dependency has changed.
        /// </summary>
        /// <param name="affectedCells">a list of the cells affected because of the change</param>
        private void reevaluateCells(IList<string> affectedCells)
        {
            foreach (string cellName in affectedCells)
                nonEmptyCells[cellName].evaluate(lookup);
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

        //what is displayed in the cell without selection
        //Double or formula error
        private object _value;

        //TODO: Delete this
        /*        /// <summary>
                /// Creates an empty cell
                /// </summary>
                public Cell(string name)
                {
                    _name = name;
                    _contents = "";
                }*/

        /// <summary>
        /// Creates a cell with formula as contents and its result as its value.
        /// </summary>
        public Cell(string name, Formula formula, Func<string, double> lookup)
        {
            _name = name;
            _contents = formula;
            _value = formula.Evaluate(lookup);
        }

        /// <summary>
        /// Creates a cell with a number
        /// </summary>
        public Cell(string name, double number)
        {
            _name = name;
            _contents = number;
            _value = number;
        }

        /// <summary>
        /// Creates a cell with text
        /// </summary>
        public Cell(string name, string text)
        {
            _name = name;
            _contents = text;
            _value = text;
        }

        /// <summary>
        /// evaluates the contents of the cell if a dependency has changed in some way.
        /// i.e. re-evaluates cell.
        /// <remarks>
        /// if A1 contains 3, and B1 contains A1 + 5 and A1 is changed to 5, B1 needs to be re-evaluated.
        /// Note that if the cell contains a string or double already, the method just re-inputs them to reset
        /// the contents
        /// </remarks>
        /// </summary>
        /// <param name="lookup">the lookup function for formula evaluation</param>
        public void evaluate(Func<string, double> lookup)
        {
            if (this._contents is Double)
                this._contents = (Double)_contents;
            else if (this._contents is String)
                this._contents = (String)_contents;
            else if (this._contents is Formula)
                this._contents = ((Formula)_contents).Evaluate(lookup);
        }

        #region Properties
        //TODO: Do I want these all public?
        public string Name
        {
            get { return _name; }
        }

        public object Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        public object Value
        {
            get { return _value; }
            protected set { _value = value; }
        }

        #endregion
    }
}