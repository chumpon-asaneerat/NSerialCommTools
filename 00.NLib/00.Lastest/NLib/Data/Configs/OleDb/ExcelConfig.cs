#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : ExcelConfig
  - Re-Implement Database Config classes for Excel File.

======================================================================================================================
Update 2010-02-03
=================
- OleDb Config inherited classes ported
  - ExcelConfig class ported and re-implements.
  - Excel sql model and related class ported.

======================================================================================================================
Update 2008-10-21
=================
- Sql Model (OleDb) updated.  
  [Excel]
  - Add new class ExcelSqlModel.ExcelDDLFormatter for handle DDL generate script.
  - Implement method CreateDDLFormatter.
  - Implement method GenerateViewScript (incompleted) in it's DDLFormatter.
  - Implement method GenerateTableScript in it's DDLFormatter.
  - Implement method GenerateTableColumnScript in it's DDLFormatter.
  - Implement method GenerateTableConstraintScript in it's DDLFormatter.

======================================================================================================================
Update 2008-07-07
=================
- DataAccess library new Features add
  - Implement Task for Excel maintance routine.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using NLib;
using NLib.Design;
using NLib.Data;
using NLib.Data.Design;
using NLib.Reflection;
using NLib.Xml;
using NLib.Utils;

#endregion

namespace NLib.Data.Design
{
    #region Excel Design time Editor

    #region Open Excel

    /// <summary>
    /// *.xls Open File Editor
    /// </summary>
    public class OpenExcelFileNameEditor : System.Windows.Forms.Design.FileNameEditor
    {
        #region Override For Dialog

        /// <summary>
        /// Initialize Dialog before process Opening
        /// </summary>
        /// <param name="openFileDialog">Open Dialog Instance</param>
        protected override void InitializeDialog(
            System.Windows.Forms.OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.Filter = "Ms Excel Files (*.xls, *.xlsx, *.xlsb, *.xlsm)|*.xls;*.xlsx;*.xlsb;*.xlsm|All Files (*.*)|*.*";
        }

        #endregion
    }

    #endregion

    #region Save Excel

    /// <summary>
    /// *.xls Save File Editor
    /// </summary>
    public class SaveExcelFileNameEditor : SaveFileEditor
    {
        #region Override For Dialog

        /// <summary>
        /// Initialize Dialog before process Saving
        /// </summary>
        /// <param name="saveFileDialog">Save Dialog Instance</param>
        protected override void InitializeDialog(
            System.Windows.Forms.SaveFileDialog saveFileDialog)
        {
            base.InitializeDialog(saveFileDialog);
            saveFileDialog.Filter = "Ms Excel Files (*.xls, *.xlsx, *.xlsb, *.xlsm)|*.xls;*.xlsx;*.xlsb;*.xlsm|All Files (*.*)|*.*";
        }

        #endregion
    }

    #endregion

    #endregion
}

namespace NLib.Data
{
    #region Common classes and Enums

    #region Enums for Excel

    #region ExcelDriver

    /// <summary>
    /// Excel Driver.
    /// </summary>
    public enum ExcelDriver
    {
        /// <summary>
        /// Used Jet
        /// </summary>
        Jet,
        /// <summary>
        /// Used ACE
        /// </summary>
        ACE
    }

    #endregion

    #region Excel Version Enum

    /// <summary>
    /// Excel Driver Version
    /// </summary>
    public enum ExcelVersion
    {
        /// <summary>
        /// Excel 95 version compatible
        /// </summary>
        Excel95,
        /// <summary>
        /// Excel 97 version compatible
        /// </summary>
        Excel97,
        /// <summary>
        /// Excel 2K version compatible
        /// </summary>
        Excel2K,
        /// <summary>
        /// Excel XP (2002) version compatible
        /// </summary>
        ExcelXP,
        /// <summary>
        /// Excel 2003 version compatible
        /// </summary>
        Excel2003,
        /// <summary>
        /// Excel 2007 version compatible
        /// </summary>
        Excel2007,
        /// <summary>
        /// Excel 2010 version compatible
        /// </summary>
        Excel2010,
        /// <summary>
        /// Excel 2012 version compatible
        /// </summary>
        Excel2012
    }

    #endregion

    #region Inter Mixed

    /// <summary>
    /// Enum for Inter Mixed Parameter for Excel Driver
    /// </summary>
    public enum IMex
    {
        /// <summary>
        /// not include in connection string
        /// </summary>
        Default = -1,
        /// <summary>
        /// set ExportMode
        /// </summary>
        ExportMode = 0,
        /// <summary>
        /// set ImportMixedType=Text in registry (force mixed data to Text)
        /// </summary>
        ImportMode = 1,
        /// <summary>
        /// full update capabilities
        /// </summary>
        LinkedMode = 2
    }

    #endregion

    #endregion

    #region Common classes for Serialization connection config

    /// <summary>
    /// The ExcelOptions class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class ExcelOptions
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExcelOptions()
            : base()
        {
            this.FileName = string.Empty;
            this.HeaderInFirstRow = true;
            this.Version = ExcelVersion.Excel2007;
            this.IMexMode = IMex.Default;
            this.Driver = ExcelDriver.Jet;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets Hash Code.
        /// </summary>
        /// <returns>Returns hashcode of current object.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        /// <summary>
        /// Conpare if object is equals.
        /// </summary>
        /// <param name="obj">The target objct to compare.</param>
        /// <returns>Returns true if object is the same.</returns>
        public override bool Equals(object obj)
        {
            if (null == obj || obj.GetType() != this.GetType())
                return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Returns string that represents an object.</returns>
        public override string ToString()
        {
            return string.Format(@"{0}, Mode = {1}, Driver = {2}",
                this.FileName, this.IMexMode, this.Driver);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Excel File Name.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets the Excel File Name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Editor(typeof(OpenExcelFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PropertyOrder(1)]
        [XmlAttribute]
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the IMEX Mode (need to see MAXROWSCAN Extended Properties documents).
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets IMEX Mode (need to see MAXROWSCAN Extended Properties documents).")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(2)]
        [XmlAttribute]
        public IMex IMexMode { get; set; }
        /// <summary>
        /// Gets or sets Excel Header Options.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets Excel Header Options.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(3)]
        [XmlAttribute]
        public bool HeaderInFirstRow { get; set; }
        /// <summary>
        /// Gets or sets the excel version.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets excel version.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(4)]
        [XmlAttribute]
        public ExcelVersion Version { get; set; }
        /// <summary>
        /// Gets or sets the excel driver.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets excel driver.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(5)]
        [XmlAttribute]
        public ExcelDriver Driver { get; set; }

        #endregion
    }

    #endregion

    #endregion

    #region ExcelConfig

    /// <summary>
    /// Microsoft Excel Connection Config class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class ExcelConfig : OleDbConfig
    {
        #region Internal Variables

        private ExcelOptions _datasource = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExcelConfig()
            : base()
        {
            // set default user name, password and authentication mode.
            this.Security.UserName = string.Empty;
            this.Security.Password = string.Empty;
            this.Security.Authentication = AuthenticationMode.Server;
            // set default server and database name.
            this.DataSource.Driver = ExcelDriver.Jet;
            // set default server date format.
            this.Optional.ServerDateFormat = string.Empty;
            this.Optional.EnableMARS = false;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~ExcelConfig()
        {

        }

        #endregion

        #region Abstract Implements

        #region GetUniqueName/GetConnectionString/GetFactory

        /// <summary>
        /// Define Each Connection Unique Name.
        /// </summary>
        /// <returns>Unique Name for Connection</returns>
        protected override string GetUniqueName()
        {
            return this.DataSource.ToString();
        }
        /// <summary>
        /// Get Connection String.
        /// </summary>
        /// <returns>Connection string based on property settings</returns>
        protected override string GetConnectionString()
        {
            #region Sample of Connection string

            // OleDbConnection
            // ===============			
            //	#Standard:
            //		"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\MyExcel.xls;Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1"""
            //		"HDR=Yes;" indicates that the first row contains columnnames, not data
            //		"IMEX=1;" tells the driver to always read "intermixed" data columns as text
            //		
            //	TIP! SQL syntax: "SELECT * FROM [sheet1$]" - i.e. worksheet name followed by a "$" and wrapped in "[" "]" brackets.
            //
            //	OdbcConnection
            //	==============
            //	#Standard:
            //		"Driver={Microsoft Excel Driver (*.xls)};DriverId=790;Dbq=C:\MyExcel.xls;DefaultDir=c:\mypath;"
            //		
            //		TIP! SQL syntax: "SELECT * FROM [sheet1$]" - i.e. worksheet name followed by a "$" and wrapped in "[" "]" brackets.

            #endregion

            string result = string.Empty;

            string sProvider = string.Empty;
            string ExtProp = string.Empty;

            if (this.DataSource.Driver == ExcelDriver.ACE)
            {
                #region Ace
                
                sProvider = "Microsoft.ACE.OLEDB.12.0";
                if (!string.IsNullOrWhiteSpace(this.DataSource.FileName))
                {
                    string ext = string.Empty;
                    try { ext = System.IO.Path.GetExtension(this.DataSource.FileName); }
                    catch { ext = string.Empty; }

                    if (!string.IsNullOrWhiteSpace(ext))
                    {
                        if (ext.ToLower().Contains("xlsx"))
                        {
                            // Version 2007 or later xlsx file (2007 Marco disable).
                            // This one is for connecting to Excel 2007 files with the Xlsx file extension. 
                            // That is the Office Open XML format with macros disabled.
                            ExtProp = "Excel 12.0 xml";
                        }
                        else if (ext.ToLower().Contains("xlsb"))
                        {
                            // Version 2007 or later xlsb file (2007 Binary)
                            // This one is for connecting to Excel 2007 files with the Xlsb file extension. 
                            // That is the Office Open XML format saved in a binary format. 
                            // I.e. the structure is similar but it's not saved in a text readable 
                            // format as the Xlsx files and can improve performance if the file 
                            // contains a lot of data.
                            ExtProp = "Excel 12.0";
                        }
                        else if (ext.ToLower().Contains("xlsm"))
                        {
                            // Version 2007 or later xlsm file (2007 Marco enable).
                            // This one is for connecting to Excel 2007 files with the Xlsm file extension. 
                            // That is the Office Open XML format with macros enabled.
                            ExtProp = "Excel 12.0 Macro";
                        }
                        else ExtProp = "Excel 8.0"; // assume used older file
                    }
                    else ExtProp = "Excel 8.0"; // cannot extract extension
                }
                else ExtProp = "Excel 8.0"; // no file assigned

                #endregion
            }
            else // Jet
            {
                #region Jet
                
                // Excel 5.0 = Excel 5, Excel 7
                // Excel 8.0 = Excel 97-2000 or later
                sProvider = "Microsoft.Jet.OLEDB.4.0";

                if (this.DataSource.Version == ExcelVersion.Excel95)
                    ExtProp = "Excel 5.0";
                else ExtProp = "Excel 8.0";

                #endregion
            }

            if (!this.DataSource.HeaderInFirstRow) ExtProp += ";HDR=NO";
            else ExtProp += ";HDR=YES";

            if (this.DataSource.IMexMode != IMex.Default)
            {
                ExtProp += ";IMEX=" + ((int)this.DataSource.IMexMode).ToString().Trim();
            }

            // Clear and Setup parameters
            this.ConnectionStrings.Clear();

            this.ConnectionStrings["Provider"] = sProvider;
            this.ConnectionStrings["Data Source"] = this.DataSource.FileName;
            this.ConnectionStrings["Extended Properties"] = "\"" + ExtProp + "\"";
            this.ConnectionStrings["Persist Security Info"] = "False";

            // Build result connection string.
            result = this.ConnectionStrings.GetConnectionString();

            if (!string.IsNullOrWhiteSpace(this.Optional.ExtendConnectionString))
            {
                // Append extend connection string.
                result += this.Optional.ExtendConnectionString;
            }

            return result;
        }
        /// <summary>
        /// Create database factory provider.
        /// </summary>
        /// <returns>Returns instance of database factory provider.</returns>
        protected override NDbFactory CreateFactory()
        {
            ExcelConnectionFactory result = new ExcelConnectionFactory();
            result.SetConfig(this);
            return result;
        }

        #endregion

        #endregion

        #region Public Properties

        #region Data Source

        /// <summary>
        /// Gets or sets Excel Connection Options.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Excel Connection Options.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PropertyOrder(ConfigOrders.DataSource)]
        public ExcelOptions DataSource
        {
            get
            {
                _datasource = this.CheckVar(_datasource);
                return _datasource;
            }
            set
            {
                _datasource = value;
                _datasource = this.CheckVar(_datasource);
                RaiseConfigChanged();
            }
        }

        #endregion

        #endregion

        #region Static Methods

        #region Get Provider Name

        /// <summary>
        /// Get Connection Provider Name.
        /// </summary>
        public static new string DbProviderName
        {
            get { return "MS Excel (OLEDB)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDbConfig Instance.
        /// </summary>
        /// <returns>Returns NDbConfig Instance.</returns>
        public static new NDbConfig Create() { return new ExcelConfig(); }

        #endregion

        #endregion
    }

    #endregion
}
