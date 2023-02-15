using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using Utility.Core.Logging;
using Utility.Toolkit.Diagnostic;

namespace Utility.Toolkit.Data
{
    [Serializable]
    public class DataParameter
    {
        #region Fields

        public SqlDbType ParamType { get; set; }
        public OracleDbType OracleParamType { get; set; }
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
        public ParameterDirection ParamDirection { get; set; }
        public int Size { get; set; }

        #endregion

        #region Ctor

        public DataParameter() { }

        public DataParameter(SqlDbType type, string name, DBNull value, ParameterDirection direction)
        {
            try
            {
                ParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }
        }

        public DataParameter(SqlDbType type, string name, string value, ParameterDirection direction) : this(type, name, value, direction, false)
        {
        }

        public DataParameter(SqlDbType type, string name, string value, ParameterDirection direction, bool allowEmptyStrings)
        {
            try
            {
                ParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;

                // No empty strings allowed
                if (!allowEmptyStrings && string.IsNullOrEmpty(value))
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }
        }

        public DataParameter(SqlDbType type, string name, object value, ParameterDirection direction)
        {
            try
            {
                ParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;

                if (value == null)
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }
        }

        public DataParameter(SqlDbType type, string name, object value, ParameterDirection direction, int size)
        {
            try
            {
                ParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;
                Size = size;

                if (value == null)
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }
        }

        public DataParameter(SqlDbType type, string name, string value, ParameterDirection direction, int size, bool allowEmptyStrings)
        {
            try
            {
                ParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;
                Size = size;

                // No empty strings allowed
                if (!allowEmptyStrings)
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }
        }

        #endregion

        #region Oracle Ctor

        public DataParameter(OracleDbType type, string name, DBNull value, ParameterDirection direction)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                OracleParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }

            FileTrace.WriteMemberExit();
        }

        public DataParameter(OracleDbType type, string name, string value, ParameterDirection direction)
            : this(type, name, value, direction, false)
        {
        }

        public DataParameter(OracleDbType type, string name, string value, ParameterDirection direction, bool allowEmptyStrings)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                OracleParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;

                // No empty strings allowed
                if (!allowEmptyStrings && string.IsNullOrEmpty(value))
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }

            FileTrace.WriteMemberExit();
        }

        public DataParameter(OracleDbType type, string name, object value, ParameterDirection direction)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                OracleParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;

                if (value == null)
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }

            FileTrace.WriteMemberExit();
        }

        public DataParameter(OracleDbType type, string name, object value, ParameterDirection direction, int size)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                OracleParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;
                Size = size;

                if (value == null)
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }

            FileTrace.WriteMemberExit();
        }

        public DataParameter(OracleDbType type, string name, string value, ParameterDirection direction, int size, bool allowEmptyStrings)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                OracleParamType = type;
                ParamName = name;
                ParamValue = value;
                ParamDirection = direction;
                Size = size;

                // No empty strings allowed
                if (!allowEmptyStrings)
                    ParamValue = DBNull.Value;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
                throw;
            }

            FileTrace.WriteMemberExit();
        }

        #endregion
    }
}
