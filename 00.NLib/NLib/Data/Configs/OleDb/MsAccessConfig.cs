#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-12-07
=================
- DataAccess : MsAccessConfig
  - Re-Implement Database Config classes for Ms Access Database File.

======================================================================================================================
Update 2013-03-08
=================
- OleDb Config inherited classes changed
  - MsAccessConfig class ported and redesigned.

======================================================================================================================
Update 2010-02-03
=================
- OleDb Config inherited classes ported
  - MsAccessConfig class ported and re-implements.
  - MsAccess sql model and related class ported.

======================================================================================================================
Update 2008-10-25
=================
- Sql Model (OleDb) updated.  
  [MsAccess]
  - Fixed Identity type.

======================================================================================================================
Update 2008-10-21
=================
- Sql Model (OleDb) updated.  
  [MsAccess]
  - Add new class MsAccessSqlModel.MsAccessDDLFormatter for handle DDL generate script.
  - Implement method CreateDDLFormatter.
  - Implement method GenerateViewScript (incompleted) in it's DDLFormatter.
  - Implement method GenerateTableScript in it's DDLFormatter.
  - Implement method GenerateTableColumnScript in it's DDLFormatter.
  - Implement method GenerateTableConstraintScript in it's DDLFormatter.

======================================================================================================================
Update 2008-07-07
=================
- DataAccess library new Features add
  - Implement Task for Ms Access maintance routine.

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
    #region Open MsAccess Editor

    /// <summary>
    /// *.mdb Open File Editor
    /// </summary>
    public class OpenMsAccessFileNameEditor : System.Windows.Forms.Design.FileNameEditor
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
            openFileDialog.Filter = "Ms Access Files (*.mdb, *.accdb)|*.mdb;*.accdb|All Files (*.*)|*.*";
        }

        #endregion
    }

    #endregion

    #region Save MsAccess Editor

    /// <summary>
    /// *.mdb Save File Editor
    /// </summary>
    public class SaveMsAccessFileNameEditor : SaveFileEditor
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
            saveFileDialog.Filter = "Ms Access Files (*.mdb, *.accdb)|*.mdb;*.accdb|All Files (*.*)|*.*";
        }

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region Common classes and Enums

    #region Enums for Ms Access

    #region MsAccess Driver Enum

    /// <summary>
    /// Ms Access Driver.
    /// </summary>
    public enum MsAccessDriver
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

    #region MsAccess Version Enum

    /// <summary>
    /// Specificed MS Access Version Compatible
    /// </summary>
    public enum MsAccessVersion
    {
        /// <summary>
        /// MsAccess 95 Compatible
        /// </summary>
        Access95,
        /// <summary>
        /// MsAccess 97 Compatible
        /// </summary>
        Access97,
        /// <summary>
        /// MsAccess 2K Compatible
        /// </summary>
        Access2K,
        /// <summary>
        /// MsAccess XP Compatible
        /// </summary>
        AccessXP,
        /// <summary>
        /// MsAccess 2003 Compatible
        /// </summary>
        Access2003,
        /// <summary>
        /// MsAccess 2007 Compatible
        /// </summary>
        Access2007,
        /// <summary>
        /// MsAccess 2010 Compatible
        /// </summary>
        Access2010,
        /// <summary>
        /// MsAccess 2013 Compatible
        /// </summary>
        Access2013
    }

    #endregion

    #endregion

    #region Common classes for Serialization connection config

    /// <summary>
    /// The MsAccessOptions class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class MsAccessOptions
    {
        #region Internal Variables

        private string _fileName = string.Empty;
        private MsAccessDriver _driver = MsAccessDriver.Jet;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MsAccessOptions()
            : base()
        {
            this.FileName = string.Empty;
            this.Version = MsAccessVersion.Access2003;
            this.Driver = MsAccessDriver.Jet;
        }

        #endregion

        #region Private Methods

        private void Recheck()
        {
            // Note. May be need to check HKCR\Microsoft.ACE.OLEDB.12.0
            // to checks in ACE is installed.

            if (!string.IsNullOrWhiteSpace(_fileName))
            {
                string ext = string.Empty;
                try { ext = System.IO.Path.GetExtension(_fileName); }
                catch { ext = string.Empty; }

                if (!string.IsNullOrWhiteSpace(ext) &&
                    ext.Contains("accdb"))
                {
                    // is 2007 or above version
                    if (_driver == MsAccessDriver.Jet)
                        this.Driver = MsAccessDriver.ACE;
                }
                else
                {
                    // is mdb both driver should supports
                }
            }
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
            return string.Format(@"{0}, Ver. = {1}, Driver = {2}",
                this.FileName, this.Version, this.Driver);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the ms access file name.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets the ms access file name.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Editor(typeof(OpenMsAccessFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PropertyOrder(1)]
        [XmlAttribute]
        public string FileName
        {
            get
            {
                if (null == _fileName) _fileName = string.Empty;
                return _fileName;
            }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    if (null == _fileName) _fileName = string.Empty;
                    Recheck();
                }
            }
        }
        /// <summary>
        /// Gets or sets the ms access version.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets ms access version.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(2)]
        [XmlAttribute]
        public MsAccessVersion Version { get; set; }
        /// <summary>
        /// Gets or sets the ms access driver.
        /// Note. Make sure that with Jet the database file should be mdb only.
        /// </summary>
        [Category("Database")]
        [Description("Gets or sets ms access driver.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [PropertyOrder(3)]
        [XmlAttribute]
        public MsAccessDriver Driver
        {
            get { return _driver; }
            set
            {
                if (_driver != value)
                {
                    _driver = value;
                    Recheck();
                }
            }
        }

        #endregion
    }

    #endregion

    #endregion

    #region MsAccessConfig

    /// <summary>
    /// Microsoft Access Connection Config class.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorterSupportExpandableTypeConverter))]
    public class MsAccessConfig : OleDbConfig
    {
        #region Internal Variables

        private MsAccessOptions _datasource = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MsAccessConfig()
            : base()
        {
            // set default user name, password and authentication mode.
            this.Security.UserName = string.Empty;
            this.Security.Password = string.Empty;
            this.Security.Authentication = AuthenticationMode.Server;
            // set default server and database name.
            this.DataSource.Driver = MsAccessDriver.Jet;
            // set default server date format.
            this.Optional.ServerDateFormat = string.Empty;
            this.Optional.EnableMARS = false;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~MsAccessConfig()
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
            #region Sample of Connection String

            //	OleDbConnection
            //	===============			
            //	Standard:
            //		Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\folder;Extended Properties=dBASE IV;User ID=Admin;Password=;
            //	
            //	OdbcConnection
            //	==============
            //	Standard:
            //		"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=c:\txtFilesFolder\;Extensions=asc,csv,tab,txt;" 			

            #endregion

            string result = string.Empty;

            // Clear and Setup parameters
            this.ConnectionStrings.Clear();

            if (this.DataSource.Driver == MsAccessDriver.ACE)
                this.ConnectionStrings["Provider"] = "Microsoft.ACE.OLEDB.12.0"; // ACE
            else this.ConnectionStrings["Provider"] = "Microsoft.Jet.OLEDB.4.0"; // Jet

            this.ConnectionStrings["Data Source"] = this.DataSource.FileName;
            
            if (!string.IsNullOrWhiteSpace(this.Security.Password))
                this.ConnectionStrings["Jet OLEDB:Database Password"] = this.Security.Password;

            if (this.DataSource.Driver == MsAccessDriver.Jet)
            {
                switch (this.DataSource.Version)
                {
                    case MsAccessVersion.Access95:
                        this.ConnectionStrings["Jet OLEDB:Engine Type"] = "3";
                        break;
                    case MsAccessVersion.Access97:
                        this.ConnectionStrings["Jet OLEDB:Engine Type"] = "4";
                        break;
                    default:
                        this.ConnectionStrings["Jet OLEDB:Engine Type"] = "5";
                        break;
                }
            }
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
            MsAccessConnectionFactory result = new MsAccessConnectionFactory();
            result.SetConfig(this);
            return result;
        }

        #endregion

        #endregion

        #region Public Properties

        #region Data Source

        /// <summary>
        /// Gets or sets Ms Access Database Connection Options.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets Ms Access Database Connection Options.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [XmlElement]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PropertyOrder(ConfigOrders.DataSource)]
        public MsAccessOptions DataSource
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
            get { return "MS Access (OLEDB)"; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create new NDbConfig Instance.
        /// </summary>
        /// <returns>Returns NDbConfig Instance.</returns>
        public static new NDbConfig Create() { return new MsAccessConfig(); }

        #endregion

        #endregion
    }

    #endregion
}
