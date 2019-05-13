using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler
{
    /// <summary>
    /// Generic <see cref="DbDataReader"/> wrapper - prototype only, not currently implemented or used
    /// </summary>
	public class ReaderWrapper : DbDataReader
	{
        /// <summary>
        /// Position based indexer
        /// </summary>
        /// <param name="ordinal">Index</param>
        /// <returns></returns>
		public override object this[int ordinal] => throw new NotImplementedException();

        /// <summary>
        /// Name based indexer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public override object this[string name] => throw new NotImplementedException();

        /// <summary>
        /// Wrapped <see cref="DbDataReader.Depth"/> property
        /// </summary>
		public override int Depth => throw new NotImplementedException();

        /// <summary>
        /// Wrapped <see cref="DbDataReader.FieldCount"/> property
        /// </summary>
		public override int FieldCount => throw new NotImplementedException();

        /// <summary>
        /// Wrapped <see cref="DbDataReader.HasRows"/> property
        /// </summary>
		public override bool HasRows => throw new NotImplementedException();

        /// <summary>
        /// Wrapped <see cref="DbDataReader.IsClosed"/> property
        /// </summary>
		public override bool IsClosed => throw new NotImplementedException();

        /// <summary>
        /// Wrapped <see cref="DbDataReader.RecordsAffected"/> property
        /// </summary>
		public override int RecordsAffected => throw new NotImplementedException();

#if NETFRAMEWORK
        /// <summary>
        /// Wrapped <see cref="DbDataReader.Close"/> method
        /// </summary>
		public override void Close()
		{
			throw new NotImplementedException();
		}
#endif

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetBoolean(int)"/> method
        /// </summary>
		public override bool GetBoolean(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetByte(int)"/> method
        /// </summary>
		public override byte GetByte(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetBytes(int, long, byte[], int, int)"/> method
        /// </summary>
		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetChar(int)"/> method
        /// </summary>
		public override char GetChar(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetChars(int, long, char[], int, int)"/> method
        /// </summary>
		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetDataTypeName(int)"/> method
        /// </summary>
		public override string GetDataTypeName(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetDateTime(int)"/> method
        /// </summary>
		public override DateTime GetDateTime(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetDecimal(int)"/> method
        /// </summary>
		public override decimal GetDecimal(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetDouble(int)"/> method
        /// </summary>
		public override double GetDouble(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetEnumerator"/> method
        /// </summary>
		public override IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetFieldType(int)"/> method
        /// </summary>
		public override Type GetFieldType(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetFloat(int)"/> method
        /// </summary>
		public override float GetFloat(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetGuid(int)"/> method
        /// </summary>
		public override Guid GetGuid(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetInt16(int)"/> method
        /// </summary>
		public override short GetInt16(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetInt32(int)"/> method
        /// </summary>
		public override int GetInt32(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetInt64(int)"/> method
        /// </summary>
		public override long GetInt64(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetName(int)"/> method
        /// </summary>
		public override string GetName(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetOrdinal(string)"/> method
        /// </summary>
		public override int GetOrdinal(string name)
		{
			throw new NotImplementedException();
		}

#if NETFRAMEWORK
        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetSchemaTable"/> method
        /// </summary>
		public override DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}
#endif

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetString(int)"/> method
        /// </summary>
		public override string GetString(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetValue(int)"/> method
        /// </summary>
		public override object GetValue(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.GetValues(object[])"/> method
        /// </summary>
		public override int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.IsDBNull(int)"/> method
        /// </summary>
		public override bool IsDBNull(int ordinal)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.NextResult()"/> method
        /// </summary>
		public override bool NextResult()
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Wrapped <see cref="DbDataReader.Read()"/> method
        /// </summary>
		public override bool Read()
		{
			throw new NotImplementedException();
		}
	}
}