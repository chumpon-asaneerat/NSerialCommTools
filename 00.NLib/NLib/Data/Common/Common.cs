//#define ENABLE_ORM

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-07
=================
- Data Access Framework - Common classes updated.
  - Changed all log code used MethodBase.
======================================================================================================================
Update 2014-11-12
=================
- Data Access Framework - Common classes
  - Ported DataReaderAdaptor class.
  - Ported ConnectionRef class.
  - Ported ConnectionPools class.
  - Ported DataAccessException class.
  - Ported Related Delegate and EventArgs classes.
======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Linq;
using System.Text;

#endregion

namespace NLib.Data.Design
{
    #region Sql Scripts

    /// <summary>
    /// The Editor for Open *.sql File.
    /// </summary>
    public class SqlScriptFileNameEditor : System.Windows.Forms.Design.FileNameEditor
    {
        #region Override For Dialog

        /// <summary>
        /// Initialize Dialog before process Opening
        /// </summary>
        /// <param name="openFileDialog">Open Dialog Instance</param>
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.Filter = "Sql Script Files (*.sql, *.txt)|*.sql;*.txt|All Files (*.*)|*.*";
        }

        #endregion
    }

    #endregion
}

namespace NLib.Data
{
    #region Data Reader Adapter

    /// <summary>
    /// Use to Convert DataReader to DataTable
    /// Used by Connection Manager derived class i.e. SqlServer Manager
    /// </summary>
    [ToolboxItem(false)]
    public class DataReaderAdapter : DbDataAdapter
    {
        #region Public Method

        /// <summary>
        /// Fill DataTable with DataReader
        /// </summary>
        /// <param name="dataTable">Target DataTable</param>
        /// <param name="dataReader">Source DataReader</param>
        /// <returns>No of Row that Fill in DataTable</returns>
        public int FillFromReader(DataTable dataTable, IDataReader dataReader)
        {
            if (null == dataTable || null == dataReader)
                return 0;

            int fieldCount = dataReader.FieldCount;

            dataTable.Columns.Clear();
            dataTable.Rows.Clear();
            if (fieldCount <= 0)
            {
                //dataTable.Columns.Add("Column", typeof(string));
            }
            else
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    dataTable.Columns.Add(dataReader.GetName(i), dataReader.GetFieldType(i));
                }
            }

            return this.Fill(dataTable, dataReader);
        }
        /// <summary>
        /// Fill DataTable with DataReader Optimize version
        /// </summary>
        /// <param name="dataTable">Target DataTable</param>
        /// <param name="dataReader">Source DataReader</param>
        /// <returns>No of Row that Fill in DataTable</returns>
        public int FillFromReaderOptimize(DataTable dataTable, IDataReader dataReader)
        {
            if (null == dataTable || null == dataReader)
                return 0;

            int fieldCount = dataReader.FieldCount;

            dataTable.Columns.Clear();
            dataTable.Rows.Clear();
            if (fieldCount <= 0)
            {
                //dataTable.Columns.Add("Column", typeof(string));
            }
            else
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    dataTable.Columns.Add(dataReader.GetName(i), dataReader.GetFieldType(i));
                }
            }

            dataTable.BeginLoadData();

            object[] objs = new object[fieldCount];

            // Note that first time after Reader is exeuted the reader will not advance automatically
            // so we need to call next to fetch data before access the datareader values
            while (dataReader.Read())
            {
                dataReader.GetValues(objs);
                dataTable.Rows.Add(objs);
            }

            dataTable.EndLoadData();

            objs = null;

            return dataTable.Rows.Count;
        }

        #endregion

        #region Override

        /// <summary>
        /// See DbDataAdapter Document (this class will return null in all case)
        /// </summary>
        /// <param name="dataRow">target DataRow</param>
        /// <param name="command">command for updating</param>
        /// <param name="statementType">Type of Command Statement</param>
        /// <param name="tableMapping">Table Mapping information object</param>
        /// <returns>RowUpdatingEventArgs object</returns>
        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, 
            IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        {
            return null;
        }
        /// <summary>
        /// See DbDataAdapter Document (this class will return null in all case)
        /// </summary>
        /// <param name="dataRow">target DataRow</param>
        /// <param name="command">command for updating</param>
        /// <param name="statementType">Type of Command Statement</param>
        /// <param name="tableMapping">Table Mapping information object</param>
        /// <returns>RowUpdatingEventArgs object</returns>
        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, 
            IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        {
            return null;
        }
        /// <summary>
        /// See DbDataAdapter Document
        /// </summary>
        /// <param name="value">RowUpdatedEventArgs object</param>
        protected override void OnRowUpdated(RowUpdatedEventArgs value) { }
        /// <summary>
        /// See DbDataAdapter Document
        /// </summary>
        /// <param name="value">RowUpdatingEventArgs object</param>
        protected override void OnRowUpdating(RowUpdatingEventArgs value) { }

        #endregion
    }

    #endregion

    #region Execute Result

    /// <summary>
    /// Execute Result class
    /// </summary>
    public class ExecuteResult<T>
    {
        #region Internal Variable

        private T _result = default(T);
        private Exception _ex = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ExecuteResult()
            : base()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        public ExecuteResult(T result)
            : this()
        {
            _result = result;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ex"></param>
        public ExecuteResult(Exception ex)
            : this()
        {
            _ex = ex;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~ExecuteResult()
        {
            // Free Memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Internal Access

        /// <summary>
        /// Set Result
        /// </summary>
        /// <param name="result"></param>
        internal void SetResult(T result)
        {
            _result = result;
        }
        /// <summary>
        /// Set Exception
        /// </summary>
        /// <param name="ex"></param>
        internal void SetException(Exception ex)
        {
            _ex = ex;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Result
        /// </summary>
        public T Result { get { return _result; } }
        /// <summary>
        /// Get Exception Object
        /// </summary>
        public Exception Exception { get { return _ex; } }
        /// <summary>
        /// Check is exception instance is exits.
        /// </summary>
        public bool HasException { get { return null != _ex; } }

        #endregion

        #region Operator Overloading

        /// <summary>
        /// implicit convert to T
        /// </summary>
        /// <param name="value">The ExecuteResult instance.</param>
        /// <returns>string that represent data in T instance.</returns>
        public static implicit operator T(ExecuteResult<T> value)
        {
            if (null == value)
                return default(T);
            else return value.Result; // Return Result
        }

        #endregion
    }

    #endregion

    #region Stored Procedure Result Class

    /// <summary>
    /// Stored Procedure Result.
    /// </summary>
    public class StoredProcedureResult : IDisposable
    {
        #region Internal Variables

        private Dictionary<string, object> _outParams = new Dictionary<string, object>();
        private DataTable _table = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public StoredProcedureResult() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~StoredProcedureResult()
        {
            Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            if (null != _outParams)
            {
                _outParams.Clear();
            }
            _outParams = null;
            if (null != _table)
            {
                try
                {
                    _table.Dispose();
                }
                catch { }
            }
            _table = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Access Out parameters value.
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, object> OutParameters
        {
            get { return _outParams; }
        }
        /// <summary>
        /// Gets DataTable that contains result.
        /// </summary>
        [Browsable(false)]
        public DataTable Table
        {
            get { return _table; }
            internal set { _table = value; }
        }

        #endregion
    }

    #endregion

    #region Recordset Reference

    /// <summary>
    /// Recordset Reference.
    /// </summary>
    internal class RecordsetReference
    {
        #region Internal Variable

        private WeakReference _commandRef = null;
        private WeakReference _readerRef = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private RecordsetReference()
            : base()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The Command object.</param>
        /// <param name="reader">The Reader object.</param>
        public RecordsetReference(DbCommand command, IDataReader reader)
            : this()
        {
            _commandRef = new WeakReference(command);
            _readerRef = new WeakReference(reader);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~RecordsetReference()
        {
            Free();
            NGC.FreeGC(this);
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Free.
        /// </summary>
        public void Free()
        {
            if (null != _commandRef)
            {
                NGC.FreeGC(_commandRef);
            }
            _commandRef = null;
            if (null != _readerRef)
            {
                NGC.FreeGC(_readerRef);
            }
            _readerRef = null;
        }
        /// <summary>
        /// Is Recordset's connection and reader is still alive.
        /// </summary>
        /// <returns>Returns true if recordset still on operation (alive).</returns>
        public bool IsAlive()
        {
            if (null == _readerRef || null == _commandRef)
            {
                return false; // Some object is already free
            }
            else// if (_readerRef != null && _commandRef != null)
            {
                return (_readerRef.IsAlive && _commandRef.IsAlive);
            }
        }

        #endregion
    }

    #endregion

    #region Recordset

    /// <summary>
    /// Recordset class.
    /// </summary>
    public class Recordset : IDisposable
    {
        #region Internal Variable

        private DbDataReader _reader = null;
        private DbCommand _command = null;
        private DataTable _currenttable = null;
        private bool _tableLock = false;

        private object[] _current = null;
        private DataRow _tempRow = null;

        private bool _canMakTable = true;
        // default both is true
        private bool _bof = true;
        private bool _eof = true;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Recordset() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~Recordset()
        {
            Free();
            // Free Memory
            NGC.FreeGC(this);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Free();
        }

        #endregion

        #region Private Method

        #region DataTable Managements

        private void CreateTable()
        {
            // release reference
            _current = null;
            _tempRow = null;
            if (null == _reader || _reader.IsClosed)
                return; // reader is unaccessable

            if (_currenttable == null)
            {
                _currenttable = new DataTable();
                // Fill column
                int fieldCount = _reader.FieldCount;

                for (int i = 0; i < fieldCount; i++)
                {
                    _currenttable.Columns.Add(_reader.GetName(i), _reader.GetFieldType(i));
                }
            }
            LockTable();
        }

        private void LockTable()
        {
            if (_tableLock) 
                return; // already lock
            if (null == _reader || _reader.IsClosed)
                return; // reader is unaccessable
            if (null != _currenttable)
            {
                _currenttable.BeginLoadData();
                // allocate memory
                _current = new object[_reader.FieldCount];
                // allocate new single row for output.
                _tempRow = _currenttable.NewRow();
                _currenttable.Rows.Add(_tempRow);
            }
            _tableLock = true;
        }

        private void UnlockTable()
        {
            if (!_tableLock) 
                return; // already unlock
            if (null != _currenttable)
            {
                try { _currenttable.EndLoadData(); }
                catch { }
                finally
                {
                    _currenttable = null;
                }
            }
            _tableLock = false;
        }

        private void FreeTable()
        {
            UnlockTable();
            if (null != _currenttable)
            {
                try { _currenttable.Dispose(); }
                catch { }
                finally
                {
                    // Free Memory
                    NGC.FreeGC(_currenttable);
                    _currenttable = null;
                }
            }
            // release reference
            _current = null;
            _tempRow = null;
        }

        #endregion

        #region Free

        private void Free()
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            _canMakTable = false;

            // release reference
            _current = null;
            _tempRow = null;

            #region Free Table

            FreeTable();

            #endregion

            #region Free Reader

            try
            {
                if (null != _reader && !_reader.IsClosed)
                {
                    _reader.Close();
                }
            }
            catch (Exception ex1)
            {
                "Free : detected exception in Close Reader operation.".Info(med);
                ex1.Err(med);
            }

            try
            {
                if (null != _reader)
                {
                    _reader.Dispose();
                    // Free Memory
                    NGC.FreeGC(_reader);
                }
            }
            catch (Exception ex2)
            {
                "Free : detected exception in Dispose Reader operation.".Info(med);
                ex2.Err(med);
            }
            _reader = null;

            #endregion

            #region Free Command

            if (null != _command)
            {
                try { _command.Dispose(); }
                catch (Exception ex)
                {
                    "Free : (dispose command) detected exception.".Info(med);
                    ex.Err(med);
                }
                finally
                {
                    // Free Memory
                    NGC.FreeGC(_command);
                    _command = null;
                }
            }

            #endregion
        }

        #endregion

        #region Process Record

        private void ProcessRecord()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                if (null == _currenttable)
                {
                    CreateTable();
                }
                try
                {
                    _reader.GetValues(_current);
                }
                catch (Exception ex)
                {
                    "ProcessRecord : detected exception in Fill operation.".Info(med);
                    ex.Err(med);
                    Free(); // whenever error occur Free it.
                }
            }
            catch (Exception ex)
            {
                "ProcessRecord : detected exception in initialize row.".Info(med);
                ex.Err(med);
                Free(); // whenever error occur Free it.
            }
        }

        #endregion

        #region Skip

        /// <summary>
        /// Advance to Next position
        /// </summary>
        /// <returns></returns>
        public bool Skip()
        {
            _canMakTable = false; // when call next so mark that cannot used reader to make datatable

            if (null != _reader && !_reader.IsClosed)
            {
                if (_reader.Read())
                {
                    _bof = false;
                    _eof = false;
                }
                else
                {
                    if (_reader.NextResult())
                    {
                        FreeTable();
                        _bof = false;
                        _eof = false;
                    }
                    else
                    {
                        Free();
                        _eof = true;
                    }
                }
            }
            else
            {
                _bof = false;
                _eof = true;
                _current = null;
                _tempRow = null;
            }

            return !_eof;
        }

        #endregion

        #region Next

        /// <summary>
        /// Fetch Next Data Row
        /// </summary>
        /// <returns>ture if advance to next record success.</returns>
        public bool Next()
        {
            _canMakTable = false; // when call next so mark that cannot used reader to make datatable

            if (null != _reader && !_reader.IsClosed)
            {
                // Note that first time after Reader is exeuted the reader will not advance automatically
                // so we need to call next to fetch data before access the datareader values
                if (_reader.Read())
                {
                    // Advance Next Row OK.
                    _bof = false;
                    ProcessRecord();
                }
                else
                {
                    if (_reader.NextResult())
                    {
                        FreeTable();
                        _bof = false;
                        ProcessRecord();
                    }
                    else
                    {
                        // No next result and no record exists. End Here
                        Free();
                        _eof = true;
                    }
                }
            }
            else
            {
                _eof = true;
                _current = null;
                _tempRow = null;
            }

            // check again
            if (null == _current) 
                _eof = true;

            return !_eof;
        }

        #endregion

        #endregion

        #region Internal Method

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="command">Command Object.</param>
        /// <param name="reader">DbDataReader Object.</param>
        internal void Init(DbCommand command, DbDataReader reader)
        {
            Free(); // Free First

            _command = command;
            _reader = reader;
            _bof = true;
            _eof = (null == _reader || _reader.IsClosed);

            _canMakTable = true;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets Reader Instance.
        /// </summary>
        public DbDataReader Reader { get { return _reader; } }
        /// <summary>
        /// Get Command Instance.
        /// </summary>
        public DbCommand Command { get { return _command; } }
        /// <summary>
        /// Is Begin Of Record.
        /// </summary>
        public bool BOF { get { return _bof; } }
        /// <summary>
        /// Is End Of Record.
        /// </summary>
        public bool EOF { get { return _eof; } }
        /// <summary>
        /// Gets Current Record As Object Array.
        /// </summary>
        public object[] Current { get { return _current; } }
        /// <summary>
        /// Gets Current Record as DataRow.
        /// </summary>
        public DataRow CurrentRow
        {
            get
            {
                if (null != _tempRow && null != _current)
                {
                    _tempRow.ItemArray = _current;
                }
                return _tempRow;
            }
        }

        #endregion

        #region To Data Table

        /// <summary>
        /// To Data Table.
        /// </summary>
        /// <returns>DataTable instance that contain all information that read from data reader.</returns>
        public DataTable[] ToDataTable()
        {
            DataTable[] results = null;
            if (null != _reader && _canMakTable)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                List<DataTable> tables = new List<DataTable>();
                DataReaderAdapter adaptor = new DataReaderAdapter();
                DataTable table = null;
                try
                {
                    do
                    {
                        try
                        {
                            table = new DataTable();
                            lock (this)
                            {
                                // Used Default Data Reader Adaptor
                                //adaptor.FillFromReader(table, _reader);

                                // a little bit faster but not much.
                                adaptor.FillFromReaderOptimize(table, _reader);
                            }
                        }
                        catch (Exception ex0)
                        {
                            "Fill From Reader : detected exception in Fill operation.".Info(med);
                            ex0.Err(med);
                            if (null != table)
                            {
                                try { table.Dispose(); }
                                catch { } // Ignore
                            }
                            table = null;
                        }
                        if (null != table) tables.Add(table);
                    }
                    while (_reader.NextResult());
                    // All OK
                    results = tables.ToArray();
                }
                catch (Exception err)
                {
                    "Fill From Reader : detected exception in Read Next operation.".Info(med);
                    err.Err(med);
                    results = null;
                }
                Free();
                _canMakTable = false; // when call next so mark that cannot used reader to make datatable               
                return results;
            }
            else
            {
                _canMakTable = false; // when call next so mark that cannot used reader to make datatable               
                return results;
            }
        }

        #endregion
    }

    #endregion

    #region Transaction Scope

    /// <summary>
    /// TransactionScope Scope class.
    /// </summary>
    public sealed class TransactionScope : IDisposable
    {
        #region Internal Variable

        private NLib.Components.NDbConnection _manager = null;
        private DbTransaction _transaction = null;
        private bool _needRollback = false;
        private Exception _lastException = null;

        private int _operationCount = 0;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        private TransactionScope()
            : base()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manager">The NDbConnection instance.</param>
        public TransactionScope(NLib.Components.NDbConnection manager)
            : this()
        {
            _manager = manager;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TransactionScope()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (null != _transaction)
            {
                if (_operationCount > 0)
                {
                    if (_needRollback)
                        Rollback();
                    else Commit();
                }
            }
            if (null != _transaction) 
                _transaction.Dispose();
            _transaction = null;
            _operationCount = 0;
        }

        #endregion

        #region Transaction operation

        /// <summary>
        /// BeginTransaction.
        /// </summary>
        /// <returns>Returns instance of DbTransaction.</returns>
        public DbTransaction BeginTransaction()
        {
            if (null == _transaction)
            {
                if (null != _manager && null != _manager.DbConnection && _manager.IsConnected)
                {
                    lock (this)
                    {
                        _transaction = _manager.DbConnection.BeginTransaction();
                        _needRollback = false;
                        _operationCount = 0;
                    }
                }
            }
            return _transaction;
        }
        /// <summary>
        /// Commit.
        /// </summary>
        public void Commit()
        {
            if (null == _transaction) 
                return;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
            catch (Exception ex)
            {
                "Detected Error in Commit operation.".Err(med);
                ex.Err(med);
            }
            _transaction = null;
            _needRollback = false;
            _operationCount = 0;
        }
        /// <summary>
        /// Rollback.
        /// </summary>
        public void Rollback()
        {
            if (null == _transaction) 
                return;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
            catch (Exception ex)
            {
                "Detected Error in Rollback operation.".Err(med);
                ex.Err(med);
            }
            _transaction = null;
            _needRollback = false;
            _operationCount = 0;
        }
        /// <summary>
        /// Has Error.
        /// </summary>
        /// <returns>Returns true if has error.</returns>
        public bool HasError()
        {
            return (null != _lastException);
        }
        /// <summary>
        /// Get Last Error.
        /// </summary>
        /// <returns>Returns the last exception.</returns>
        public Exception GetLastError()
        {
            return _lastException;
        }
        /// <summary>
        /// Is oparation need rollback.
        /// </summary>
        public bool NeedRollback { get { return _needRollback; } }
        /// <summary>
        /// Get Operation count that call via Transaction Scope.
        /// </summary>
        public int OperationCount { get { return _operationCount; } }

        #endregion

        #region Query

        /// <summary>
        /// Query.
        /// </summary>
        /// <param name="queryText">The query string.</param>
        /// <returns>Returns ExecuteResult of data table.</returns>
        public ExecuteResult<DataTable> Query(string queryText)
        {
            ExecuteResult<DataTable> execResult = new ExecuteResult<DataTable>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Query(queryText, _transaction);
            if (execResult.Exception != null)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }
        /// <summary>
        /// Query.
        /// </summary>
        /// <param name="command">The DbCommand instance.</param>
        /// <returns>Returns ExecuteResult of data table.</returns>
        public ExecuteResult<DataTable> Query(DbCommand command)
        {
            ExecuteResult<DataTable> execResult = new ExecuteResult<DataTable>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Query(command, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Execute NonQuery

        /// <summary>
        /// Execute NonQuery.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Returns ExecuteResult of int.</returns>
        public ExecuteResult<int> ExecuteNonQuery(string commandText)
        {
            ExecuteResult<int> execResult = new ExecuteResult<int>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteNonQuery(commandText, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }
        /// <summary>
        /// Execute NonQuery.
        /// </summary>
        /// <param name="command">The DbCommand instance.</param>
        /// <returns>Returns ExecuteResult of int.</returns>
        public ExecuteResult<int> ExecuteNonQuery(DbCommand command)
        {
            ExecuteResult<int> execResult = new ExecuteResult<int>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteNonQuery(command, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Execute Scalar

        /// <summary>
        /// Execute Scalar.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Returns ExecuteResult of objcet.</returns>
        public ExecuteResult<object> ExecuteScalar(string commandText)
        {
            ExecuteResult<object> execResult = new ExecuteResult<object>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteScalar(commandText, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }
        /// <summary>
        /// Execute Scalar.
        /// </summary>
        /// <param name="command">The DbCommand instance.</param>
        /// <returns>Returns ExecuteResult of objcet.</returns>
        public ExecuteResult<object> ExecuteScalar(DbCommand command)
        {
            ExecuteResult<object> execResult = new ExecuteResult<object>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteScalar(command, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Execute Reader

        /// <summary>
        /// Execute Reader.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Returns ExecuteResult of DataTable array.</returns>
        public ExecuteResult<DataTable[]> ExecuteReader(string commandText)
        {
            ExecuteResult<DataTable[]> execResult = new ExecuteResult<DataTable[]>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteReader(commandText, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }
        /// <summary>
        /// Execute Reader.
        /// </summary>
        /// <param name="command">The DbCommand instance.</param>
        /// <returns>Returns ExecuteResult of DataTable array.</returns>
        public ExecuteResult<DataTable[]> ExecuteReader(DbCommand command)
        {
            ExecuteResult<DataTable[]> execResult = new ExecuteResult<DataTable[]>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteReader(command, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Execute Procedure

        /// <summary>
        /// Execute Procedure.
        /// </summary>
        /// <param name="procedureName">The stored procedure's name.</param>
        /// <param name="parameterNames">The array of parameter names.</param>
        /// <param name="parameterValues">The array of parameter values.</param>
        /// <returns>Returns ExecuteResult of StoredProcedureResult.</returns>
        public ExecuteResult<StoredProcedureResult> ExecuteProcedure(string procedureName,
            string[] parameterNames, object[] parameterValues)
        {
            ExecuteResult<StoredProcedureResult> execResult = new ExecuteResult<StoredProcedureResult>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteProcedure(procedureName, parameterNames, parameterValues, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }
        /// <summary>
        /// Execute Procedure.
        /// </summary>
        /// <param name="command">The DbCommand instance.</param>
        /// <param name="parameterNames">The array of parameter names.</param>
        /// <param name="parameterValues">The array of parameter values.</param>
        /// <returns>Returns ExecuteResult of StoredProcedureResult.</returns>
        public ExecuteResult<StoredProcedureResult> ExecuteProcedure(DbCommand command,
                    string[] parameterNames, object[] parameterValues)
        {
            ExecuteResult<StoredProcedureResult> execResult = new ExecuteResult<StoredProcedureResult>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.ExecuteProcedure(command, parameterNames, parameterValues, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region ORM Functions
#if ENABLE_ORM
        #region Create

        /// <summary>
        /// Create new object instance.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <returns>Return the new object instance.</returns>
        public T Create<T>()
        {
            return _manager.Create<T>();
        }

        #endregion

        #region Select

        /// <summary>
        /// Select all from storage (database) and stored the retrived value in object list.
        /// </summary>
        /// <typeparam name="T">Target object Type.</typeparam>
        /// <returns>Return Execute Result with List that contains object type T that read from Database.</returns>
        public ExecuteResult<List<T>> Select<T>()
        {
            ExecuteResult<List<T>> execResult = new ExecuteResult<List<T>>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Select<T>(_transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }
        /// <summary>
        /// Select with criteria from storage (database) and stored the retrived value in object list.
        /// </summary>
        /// <typeparam name="T">Target object Type.</typeparam>
        /// <param name="whereClause">The where clause string.</param>
        /// <returns>Return Execute Result with List that contains object type T that read from Database.</returns>
        public ExecuteResult<List<T>> Select<T>(string whereClause)
        {
            ExecuteResult<List<T>> execResult = new ExecuteResult<List<T>>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Select<T>(whereClause, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insert the new record by an object instance value.
        /// </summary>
        /// <typeparam name="T">Target object Type</typeparam>
        /// <param name="value">The instance to insert to database.</param>
        /// <returns>Return Execute Result with true if insert is success.</returns>
        public ExecuteResult<bool> Insert<T>(T value)
        {
            ExecuteResult<bool> execResult = new ExecuteResult<bool>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Insert<T>(value, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update the record that match an object instance value.
        /// </summary>
        /// <typeparam name="T">Target object Type.</typeparam>
        /// <param name="value">The instance to update to database.</param>
        /// <returns>Return Execute Result with true if update is success.</returns>
        public ExecuteResult<bool> Update<T>(T value)
        {
            ExecuteResult<bool> execResult = new ExecuteResult<bool>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Update<T>(value, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete the record that match an object instance value.
        /// </summary>
        /// <typeparam name="T">Target object Type.</typeparam>
        /// <param name="value">The instance to delete from database.</param>
        /// <returns>Return Execute Result with true if delete is success.</returns>
        public ExecuteResult<bool> Delete<T>(T value)
        {
            ExecuteResult<bool> execResult = new ExecuteResult<bool>();
            if (_needRollback)
            {
                // need rollback so no exceute occur here.
                return execResult;
            }
            if (null == _manager)
            {
                return execResult;
            }
            _operationCount++; // Increase opeartion counter
            execResult = _manager.Delete<T>(value, _transaction);
            if (null != execResult.Exception)
            {
                _lastException = execResult.Exception;
                _needRollback = true;
            }

            return execResult;
        }

        #endregion
#endif
        #endregion
    }

    #endregion
}
