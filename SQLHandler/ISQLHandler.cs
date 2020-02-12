using System.Collections.Generic;
using System.Data;

namespace DataProvider
{
    /// <summary>
    /// Interface for SQLHandler class
    /// </summary>
    public interface ISQLHandler
    {
        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="databaseName">The name of the database where the stored procedure resides</param>
        /// <param name="databaseLevel">The level of data (Dev, Test, UET, Live)</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns>(DataSet)</returns>
        DataSet ExecuteQuery(string databaseName, string databaseLevel, string storedProcedureName, List<ParameterSQL> parameters);

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="databaseName">The name of the database where the stored procedure resides</param>
        /// <param name="databaseLevel">The level of data (Dev, Test, UET, Live)</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns>(DataSet)</returns>
        void ExecuteNonQuery(string databaseName, string databaseLevel, string storedProcedureName, List<ParameterSQL> parameters);

        /// <summary>
        /// Builds and returns a database connection string
        /// </summary>
        /// <param name="databaseName">The name of the database to build the connection string to</param>
        /// <param name="databaseLevel">The level of data (Dev, Test, UET, Live)</param>
        /// <returns>(string)</returns>
        string GetConnectionString(string databaseName, string databaseLevel);

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns></returns>
        DataSet ExecuteQuery(string storedProcedureName, List<ParameterSQL> parameters);

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        void ExecuteNonQuery(string storedProcedureName, List<ParameterSQL> parameters);

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <param name="cfConStr">Configuration file element name for the connection string</param>
        /// <param name="cfServer">Configuration file element name for the database server name</param>
        /// <param name="cfDatabase">Configuration file element name for the database name</param>
        /// <param name="cfUser">Configuration file element name for the database user name</param>
        /// <param name="cfPassword">Configuration file element name for the database user's password</param>
        /// <returns>(DataSet)</returns>
        DataSet ExecuteQuery(string storedProcedureName, List<ParameterSQL> parameters, string cfConStr, string cfServer, string cfDatabase, string cfUser, string cfPassword);

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <param name="cfConStr">Configuration file element name for the connection string</param>
        /// <param name="cfServer">Configuration file element name for the database server name</param>
        /// <param name="cfDatabase">Configuration file element name for the database name</param>
        /// <param name="cfUser">Configuration file element name for the database user name</param>
        /// <param name="cfPassword">Configuration file element name for the database user's password</param>
        void ExecuteNonQuery(string storedProcedureName, List<ParameterSQL> parameters, string cfConStr, string cfServer, string cfDatabase, string cfUser, string cfPassword);

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        void ExecuteNonQuery(string connectionString, string storedProcedureName, List<ParameterSQL> parameters);

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns>(DataSet)</returns>
        DataSet ExecuteQuery(string connectionString, string storedProcedureName, List<ParameterSQL> parameters);

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="cfConStr">Configuration file element name for the connection string</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        void ExecuteNonQuery(string cfConStr, List<ParameterSQL> parameters, string storedProcedureName);

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="cfConStr">Configuration file element name for the connection string</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <returns>(DataSet)</returns>
        DataSet ExecuteQuery(string cfConStr, List<ParameterSQL> parameters, string storedProcedureName);
    }
}
