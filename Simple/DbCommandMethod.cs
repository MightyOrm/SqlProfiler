using System.Data;
using System.Data.Common;

namespace SqlProfiler.Simple
{
    /// <summary>
    /// The <see cref="DbCommand"/> methods which are intercepted by <see cref="SimpleCommandProfiler"/>.
    /// </summary>
    public enum DbCommandMethod
    {
        /// <summary>
        /// <see cref="DbCommand.ExecuteDbDataReader(CommandBehavior)"/>
        /// </summary>
        ExecuteDbDataReader,

        /// <summary>
        /// <see cref="DbCommand.ExecuteNonQuery"/>
        /// </summary>
        ExecuteNonQuery,

        /// <summary>
        /// <see cref="DbCommand.ExecuteScalar"/>
        /// </summary>
        ExecuteScalar
    }
}
