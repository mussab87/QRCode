using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using Utility.Toolkit.Diagnostic;
using Utility.Core.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Utility.Toolkit.Data
{
    public class DataManager
    {
        private const int SQL_DEADLOCK_MAX_RETRY_COUNT = 5;
        private const int SQL_DEADLOCK_ERROR_CODE = 1205;
        private const int SQL_TIMEOUT_ERROR_CODE = -2;
        private const int SQL_CONN_BROKEN = -1;
        private const int SQL_NO_MEMORY = 701;
        private const int SQL_NO_LOCKS_AVAILABLE = 1204;
        private const int SQL_LOCK_REQ_TIMEOUT = 1222;
        private const int SQL_TIMEOUT_WAITING_FOR_MEM_RESOURCE = 8645;
        private const int SQL_LOW_MEM_CONDITION = 8651;
        private const int SQL_WORKBREAKER_TIMEOUT = 30053;

        #region SQL Server Delegates

        public delegate T ForwardOnlySProcResultsEvent<T>(SqlDataReader r, DataParameter[] p);
        public ForwardOnlySProcResultsEvent<IList> ForwardOnlySProcResults = null;

        public delegate void ForwardOnlyQueryResultsEvent(SqlDataReader r);
        public ForwardOnlyQueryResultsEvent ForwardOnlyQueryResults = null;

        #endregion

        #region Oracle Delegates

        public delegate T OracleForwardOnlySProcResultsEvent<T>(OracleDataReader r, DataParameter[] p);
        public OracleForwardOnlySProcResultsEvent<IList> OracleForwardOnlySProcResults = null;

        public delegate void OracleForwardOnlyQueryResultsEvent(OracleDataReader r);
        public OracleForwardOnlyQueryResultsEvent OracleForwardOnlyQueryResults = null;

        #endregion

        #region Public Methods

        public int ExecuteStoredProcedureNonQuery(string connection, string procName, int timeout)
        {
            int rowsUpdated = 0;
            try
            {
                DataParameter[] dp = new DataParameter[] { };
                rowsUpdated = ExecuteStoredProcedureNonQuery(connection, procName, ref dp, timeout);
            }
            catch (Exception)
            {
                throw;
            }
            return rowsUpdated;
        }

        public int ExecuteStoredProcedureNonQuery(string connection, string procName)
        {
            int rowsUpdated = 0;
            try
            {
                DataParameter[] dp = new DataParameter[] { };
                rowsUpdated = ExecuteStoredProcedureNonQuery(connection, procName, ref dp, 30);
            }
            catch (Exception)
            {
                throw;
            }
            return rowsUpdated;
        }

        public int ExecuteStoredProcedureNonQuery(string connection, string procName, ref DataParameter[] procParameters)
        {
            int rowsUpdated = 0;
            try
            {
                rowsUpdated = ExecuteStoredProcedureNonQuery(connection, procName, ref procParameters, 30);
            }
            catch (Exception)
            {
                throw;
            }
            return rowsUpdated;
        }

        public int ExecuteStoredProcedureNonQuery(string connection, string procName, ref DataParameter[] procParameters, int commandTimeout)
        {
            int rowsUpdated = 0;
            try
            {
                rowsUpdated = ExecuteStoredProcedureNonQuery(connection, procName, ref procParameters, commandTimeout, false);
            }
            catch (Exception)
            {

                throw;
            }
            return rowsUpdated;
        }

        public int ExecuteStoredProcedureNonQuery(string connection, string procName, ref DataParameter[] procParameters, int commandTimeout, bool throwError, int retryCount = 3)
        {
            int rowsUpdated = 0;

            SqlConnection c = null;
            try
            {
                ArrayList outParams = new ArrayList();

                int i = 0;
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand(procName);
                if (procParameters != null && procParameters.Length > 0)
                {
                    // loop through the parameter arrays
                    foreach (DataParameter param in procParameters)
                    {
                        SqlParameter p = new SqlParameter();
                        p.ParameterName = param.ParamName;
                        p.SqlDbType = param.ParamType;

                        if (param.ParamValue != null)
                        {
                            p.Value = param.ParamValue;
                            p.SqlValue = param.ParamValue;
                        }

                        if (param.Size > -1)
                        {
                            p.Size = param.Size;
                        }

                        p.SqlDbType = param.ParamType;
                        p.Direction = param.ParamDirection;
                        command.Parameters.Add(p);

                        if (param.ParamDirection == ParameterDirection.Output || param.ParamDirection == ParameterDirection.InputOutput)
                        {
                            outParams.Add(i);
                        }
                        i++;
                    }
                }

                command.CommandType = CommandType.StoredProcedure;
                if (commandTimeout != 30)
                {
                    Trace.WriteLine("Setting commandTimeout to: {0}", commandTimeout);
                    command.CommandTimeout = commandTimeout;
                }

                OpenConnection(ref c);

                command.Connection = c;
                rowsUpdated = command.ExecuteNonQuery();

                // Set any output variables
                if (outParams.Count > 0)
                {
                    foreach (int x in outParams)
                    {
                        procParameters[x].ParamValue = command.Parameters[procParameters[x].ParamName].Value;
                    }
                }
                CloseConnection(ref c);
            }
            catch (SqlException sqlEx)
            {
                if (retryCount == SQL_DEADLOCK_MAX_RETRY_COUNT)
                {
                    // Give up and throw the error back up.
                    throw;
                }

                switch (sqlEx.Number)
                {
                    case SQL_DEADLOCK_ERROR_CODE: 
                    case SQL_TIMEOUT_ERROR_CODE:
                    case SQL_CONN_BROKEN:
                    case SQL_NO_MEMORY:
                    case SQL_NO_LOCKS_AVAILABLE:
                    case SQL_LOCK_REQ_TIMEOUT:
                    case SQL_TIMEOUT_WAITING_FOR_MEM_RESOURCE:
                    case SQL_LOW_MEM_CONDITION:
                    case SQL_WORKBREAKER_TIMEOUT:
                        // Try again.
                        break;
                    default:
                        // All other errors, throw back to the caller.
                        throw;
                }

                System.Threading.Thread.Sleep(3000);
                return ExecuteStoredProcedureNonQuery(connection, procName, ref procParameters, commandTimeout, throwError, ++retryCount);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
            return rowsUpdated;
        }

        public DataSet ExecuteStoredProcedureCachedReturn(string connection, string procName, int commandTimeout)
        {
            DataSet d = null;
            try
            {
                DataParameter[] dp = new DataParameter[0] { };
                d = ExecuteStoredProcedureCachedReturn(connection, procName, ref dp, commandTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public DataSet ExecuteStoredProcedureCachedReturn(string connection, string procName)
        {
            DataSet d = null;
            try
            {
                DataParameter[] dp = new DataParameter[0] { };
                d = ExecuteStoredProcedureCachedReturn(connection, procName, ref dp);
            }
            catch (Exception)
            {

                throw;
            }
            return d;
        }

        public DataSet ExecuteStoredProcedureCachedReturn(string connection, string procName, ref DataParameter[] procParameters)
        {
            DataSet dataSet = null;
            SqlConnection c = null;
            try
            {
                Trace.WriteLine("SP Used : " + procName);
                dataSet = ExecuteStoredProcedureCachedReturn(connection, procName, ref procParameters, 30);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
            return dataSet;
        }

        public DataSet ExecuteStoredProcedureCachedReturn(string connection, string procName, ref DataParameter[] procParameters, int commandTimeout)
        {
            DataSet dataSet = null;
            SqlConnection c = null;
            try
            {
                ArrayList outParams = new ArrayList();

                int i = 0;
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand(procName);

                dataSet = new DataSet();
                if (procParameters != null && procParameters.Length > 0)
                {
                    // loop through the parameter arrays
                    foreach (DataParameter param in procParameters)
                    {
                        SqlParameter p = new SqlParameter(param.ParamName, param.ParamValue);

                        p.SqlDbType = param.ParamType;
                        p.Direction = param.ParamDirection;
                        command.Parameters.Add(p);

                        if (param.ParamDirection == ParameterDirection.Output || param.ParamDirection == ParameterDirection.InputOutput)
                        {
                            outParams.Add(i);
                        }
                        i++;
                    }
                }

                command.CommandType = CommandType.StoredProcedure;
                if (commandTimeout != 30)
                {
                    Trace.WriteLine("Setting commandTimeout to: {0}", commandTimeout);
                    command.CommandTimeout = commandTimeout;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                OpenConnection(ref c);
                command.Connection = c;

                dataAdapter.Fill(dataSet);

                // Set any output variables
                if (outParams.Count > 0)
                {
                    foreach (int x in outParams)
                    {
                        procParameters[x].ParamValue = command.Parameters[procParameters[x].ParamName].Value;

                    }
                }

                CloseConnection(ref c);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
            return dataSet;
        }

        public DataSet ExecuteStoredProcedureCachedReturn(string connection, string procName, ref DataParameter[] procParameters, int commandTimeout, bool throwError)
        {
            DataSet dataSet = null;
            SqlConnection c = null;
            try
            {
                ArrayList outParams = new ArrayList();

                int i = 0;
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand(procName);

                dataSet = new DataSet();

                if (procParameters != null && procParameters.Length > 0)
                {
                    // loop through the parameter arrays
                    foreach (DataParameter param in procParameters)
                    {
                        SqlParameter p = new SqlParameter(param.ParamName, param.ParamValue);

                        p.SqlDbType = param.ParamType;
                        p.Direction = param.ParamDirection;
                        command.Parameters.Add(p);

                        if (param.ParamDirection == ParameterDirection.Output || param.ParamDirection == ParameterDirection.InputOutput)
                        {
                            outParams.Add(i);
                        }
                        i++;
                    }
                }

                command.CommandType = CommandType.StoredProcedure;
                if (commandTimeout != 30)
                {
                    Trace.WriteLine("Setting commandTimeout to: {0}", commandTimeout);
                    command.CommandTimeout = commandTimeout;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                OpenConnection(ref c);
                command.Connection = c;

                dataAdapter.Fill(dataSet);

                // Set any output variables
                if (outParams.Count > 0)
                {
                    foreach (int x in outParams)
                    {
                        procParameters[x].ParamValue = command.Parameters[procParameters[x].ParamName].Value;

                    }
                }
                CloseConnection(ref c);
            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }


            return dataSet;
        }

        public IList ExecuteStoredProcedureForwardOnlyReturn(string connection, string procName)
        {
            return ExecuteStoredProcedureForwardOnlyReturn(connection, procName, 200);
        }

        public IList ExecuteStoredProcedureForwardOnlyReturn(string connection, string procName, int dbTimeout)
        {
            IList list = null;
            try
            {
                DataParameter[] dp = new DataParameter[0] { };
                list = ExecuteStoredProcedureForwardOnlyReturn(connection, procName, ref dp, dbTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        public IList ExecuteStoredProcedureForwardOnlyReturn(string connection, string procName, ref DataParameter[] procParameters)
        {
            return ExecuteStoredProcedureForwardOnlyReturn(connection, procName, ref procParameters, 200);
        }

        public IList ExecuteStoredProcedureForwardOnlyReturn(string connection, string procName, ref DataParameter[] procParameters, int dbTimeout)
        {
            SqlConnection c = null;

            IList list = null;
            SqlDataReader reader = null;
            try
            {
                if (ForwardOnlySProcResults == null)
                    throw new Exception("Cannot proceed with sp execution as forward delegate not set. Ensure this is set prior to calling this operation.");

                ArrayList outParams = new ArrayList();

                int i = 0;
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand(procName, c);
                command.Connection = c;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = dbTimeout;

                if (procParameters != null && procParameters.Length > 0)
                {
                    // loop through the parameter arrays
                    foreach (DataParameter param in procParameters)
                    {
                        SqlParameter p = new SqlParameter(param.ParamName, param.ParamValue);

                        p.SqlDbType = param.ParamType;
                        p.Direction = param.ParamDirection;
                        command.Parameters.Add(p);

                        if (param.ParamDirection == ParameterDirection.Output || param.ParamDirection == ParameterDirection.InputOutput)
                        {
                            outParams.Add(i);
                        }
                        i++;
                    }
                }

                OpenConnection(ref c);

                reader = command.ExecuteReader();

                // Set any output variables
                if (outParams.Count > 0)
                {
                    foreach (int x in outParams)
                    {
                        procParameters[x].ParamValue = command.Parameters[procParameters[x].ParamName].Value;
                    }
                }

                // Forward and populate the list
                list = ForwardOnlySProcResults(reader, procParameters);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                CloseConnection(ref c);
            }
            return list;
        }

        public IList ExecuteQueryForwardOnlyReturn(string connection, string query, int dbTimeout)
        {
            IList list = null;

            SqlConnection c = null;
            SqlDataReader reader = null;
            try
            {
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand(query, c);
                command.Connection = c;
                command.CommandTimeout = dbTimeout;
                command.CommandType = CommandType.Text;

                OpenConnection(ref c);

                reader = command.ExecuteReader();

                // Invoke the delegate so that the caller can process the results
                // Forward and populate the list
                list = ForwardOnlySProcResults(reader, null);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                CloseConnection(ref c);
            }
            return list;
        }

        public DataSet ExecuteQuery(string connection, String strQuery)
        {
            SqlConnection c = null;
            DataSet d = null;

            try
            {
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand();
                command.Connection = c;
                command.CommandType = CommandType.Text;
                command.CommandText = strQuery;

                OpenConnection(ref c);

                Trace.WriteLine("About to execute query : ");
                Trace.WriteLine(strQuery);
                SqlDataReader reader = command.ExecuteReader();

                DataTable table = new DataTable();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i));
                }
                table.BeginLoadData();
                object[] values = new object[reader.FieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();

                d = new DataSet();
                d.Tables.Add(table);
                reader.Close();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
            return d;
        }

        public DataTable ExecuteQueryDataTable(string connection, String strQuery)
        {
            SqlConnection c = null;
            DataTable t = null;
            try
            {
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand();
                command.Connection = c;
                command.CommandType = CommandType.Text;
                command.CommandText = strQuery;

                OpenConnection(ref c);

                Trace.WriteLine("About to execute query : ");
                Trace.WriteLine(strQuery);
                SqlDataReader reader = command.ExecuteReader();

                t = new DataTable();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    t.Columns.Add(reader.GetName(i));
                }

                t.BeginLoadData();
                object[] values = new object[reader.FieldCount];

                while (reader.Read())
                {
                    reader.GetValues(values);
                    t.LoadDataRow(values, true);
                }
                t.EndLoadData();
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
            return t;
        }

        public void ExecuteNonQuery(string connection, String query)
        {
            SqlConnection c = null;
            try
            {
                c = new SqlConnection(connection);

                SqlCommand command = new SqlCommand();
                command.Connection = c;
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                OpenConnection(ref c);

                Trace.WriteLine("About to execute query : ");
                Trace.WriteLine(query);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
        }

        public void BulkInsert(string connection, DataTable dataTable)
        {
            SqlConnection c = null;
            try
            {
                c = new SqlConnection(connection);
                OpenConnection(ref c);
                using (SqlBulkCopy s = new SqlBulkCopy(c))
                {
                    s.DestinationTableName = dataTable.TableName;

                    foreach (var column in dataTable.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());

                    s.WriteToServer(dataTable);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
        }

        #endregion

        #region Oracle Public Methods

        public IList ExecuteOracleStoredProcedureForwardOnlyReturn(string connection, string procName, ref DataParameter[] procParameters, int dbTimeout)
        {
            OracleConnection c = null;

            IList list = null;
            OracleDataReader reader = null;
            try
            {
                if (OracleForwardOnlySProcResults == null)
                    throw new Exception("Cannot proceed with sp execution as forward delegate not set. Ensure this is set prior to calling this operation.");

                ArrayList outParams = new ArrayList();

                int i = 0;
                c = new OracleConnection(connection);

                OracleCommand command = new OracleCommand(procName, c);
                command.Connection = c;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = dbTimeout;

                // need to specify the bind by name flag Oracle binds parameters by order as default
                command.BindByName = true;

                if (procParameters != null && procParameters.Length > 0)
                {
                    // loop through the parameter arrays
                    foreach (DataParameter param in procParameters)
                    {
                        OracleParameter p = new OracleParameter(param.ParamName, param.ParamValue);

                        p.OracleDbType = param.OracleParamType;
                        p.Direction = param.ParamDirection;
                        command.Parameters.Add(p);

                        if (param.ParamDirection == ParameterDirection.Output || param.ParamDirection == ParameterDirection.InputOutput)
                        {
                            outParams.Add(i);
                        }
                        i++;
                    }
                }

                OpenConnection(ref c);

                reader = command.ExecuteReader();

                // Set any output variables
                if (outParams.Count > 0)
                {
                    foreach (int x in outParams)
                    {
                        procParameters[x].ParamValue = command.Parameters[procParameters[x].ParamName].Value;
                    }
                }

                // Forward and populate the list
                list = OracleForwardOnlySProcResults(reader, procParameters);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                CloseConnection(ref c);
            }
            return list;
        }

        public int ExecuteOracleStoredProcedureNonQuery(string connection, string procName, ref DataParameter[] procParameters, int commandTimeout)
        {
            int rowsUpdated = 0;
            try
            {
                rowsUpdated = ExecuteOracleStoredProcedureNonQuery(connection, procName, ref procParameters, commandTimeout, false);
            }
            catch (Exception)
            {

                throw;
            }
            return rowsUpdated;
        }

        public int ExecuteOracleStoredProcedureNonQuery(string connection, string procName, ref DataParameter[] procParameters, int commandTimeout, bool throwError)
        {
            int rowsUpdated = 0;

            OracleConnection c = null;
            try
            {
                ArrayList outParams = new ArrayList();

                int i = 0;
                c = new OracleConnection(connection);

                OracleCommand command = new OracleCommand(procName);
                if (procParameters != null && procParameters.Length > 0)
                {
                    // loop through the parameter arrays
                    foreach (DataParameter param in procParameters)
                    {
                        OracleParameter p = new OracleParameter();
                        p.ParameterName = param.ParamName;
                        p.OracleDbType = param.OracleParamType;

                        if (param.ParamValue != null)
                        {
                            p.Value = param.ParamValue;
                            // no equivalent to SqlValue property in OracleParameter class
                            //p.SqlValue = param.ParamValue;
                        }

                        if (param.Size > -1)
                        {
                            p.Size = param.Size;
                        }

                        p.OracleDbType = param.OracleParamType;
                        p.Direction = param.ParamDirection;
                        command.Parameters.Add(p);

                        if (param.ParamDirection == ParameterDirection.Output || param.ParamDirection == ParameterDirection.InputOutput)
                        {
                            outParams.Add(i);
                        }
                        i++;
                    }
                }

                command.CommandType = CommandType.StoredProcedure;

                // need to specify the bind by name flag Oracle binds parameters by order as default               
                command.BindByName = true;

                if (commandTimeout != 30)
                {
                    Trace.WriteLine("Setting commandTimeout to: {0}", commandTimeout);
                    command.CommandTimeout = commandTimeout;
                }

                OpenConnection(ref c);

                command.Connection = c;

                rowsUpdated = command.ExecuteNonQuery();

                // Set any output variables
                if (outParams.Count > 0)
                {
                    foreach (int x in outParams)
                    {
                        procParameters[x].ParamValue = command.Parameters[procParameters[x].ParamName].Value;
                    }
                }
                CloseConnection(ref c);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection(ref c);
            }
            return rowsUpdated;
        }

        #endregion

        #region Private Methods

        private void OpenConnection(ref SqlConnection c)
        {
            try
            {
                switch (c.State)
                {
                    case ConnectionState.Closed:
                        c.Open();
                        break;

                    case ConnectionState.Broken:
                        CloseConnection(ref c);
                        c.Open();
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CloseConnection(ref SqlConnection c)
        {
            try
            {
                if (c == null)
                    return;

                if (c.State != ConnectionState.Closed)
                    c.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Oracle Private Methods

        private void OpenConnection(ref OracleConnection c)
        {
            try
            {
                switch (c.State)
                {
                    case ConnectionState.Closed:
                        c.Open();
                        break;

                    case ConnectionState.Broken:
                        CloseConnection(ref c);
                        c.Open();
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CloseConnection(ref OracleConnection c)
        {
            try
            {
                if (c == null)
                    return;

                if (c.State != ConnectionState.Closed)
                    c.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Datatable Methods
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #endregion
    }
}