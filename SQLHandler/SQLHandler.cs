using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq;
using Zoeller;

namespace DatabaseHelper
{
    /// <summary>
    /// Used for retrieving data from a database
    /// </summary>
    public class SQLHandler : ISQLHandler
    {
        #region "Private Variables"

        private int? _procedureReturn = null;
        private List<ParameterSQL> _parameters = new List<ParameterSQL>();

        #endregion "Private Variables"


        #region "Private Properties"

        /// <summary>
        /// The value returned from the last Stored Procedure call
        /// </summary>
        public int? ProcedureReturn { get { return this._procedureReturn; } }

        /// <summary>
        /// The updateed parameters after the Stored Procedure call
        /// </summary>
        public List<ParameterSQL> Parameters { get { return this._parameters; } }
        #region "Private Properties"


        #region "Public Methods"

        #endregion "Database Stored Connection Information"

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="databaseName">The name of the database where the stored procedure resides</param>
        /// <param name="databaseLevel">The level of data (Dev, Test, UET, Live)</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns>(DataSet)</returns>
        public DataSet ExecuteQuery(string databaseName, string databaseLevel, string storedProcedureName, List<ParameterSQL> parameters)
        {
            if (databaseName.IsNullOrEmpty())
            {
                throw new Exception("Invalid database name");
            }
            if (databaseLevel.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid database level");
            }

            string connectionString = this.GetConnectionString(databaseName, databaseLevel);

            if(connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not locate " + databaseName + " for level " + databaseLevel);
            }

            using (DataSet returnValue = new DataSet())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        this.LoadParameters(command, storedProcedureName, parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(returnValue);
                        }
                        this.ReadOutputParameters(command, parameters);
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="databaseName">The name of the database where the stored procedure resides</param>
        /// <param name="databaseLevel">The level of data (Dev, Test, UET, Live)</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns>(DataSet)</returns>
        public void ExecuteNonQuery(string databaseName, string databaseLevel, string storedProcedureName, List<ParameterSQL> parameters)
        {
            if (databaseName.IsNullOrEmpty())
            {
                throw new Exception("Invalid database name");
            }
            if (databaseLevel.IsNullOrEmpty())
            {
                throw new Exception("Invalid database level");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }

            string connectionString = this.GetConnectionString(databaseName, databaseLevel);

            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not locate " + databaseName + " for level " + databaseLevel);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    this.LoadParameters(command, storedProcedureName, parameters);

                    command.ExecuteNonQuery();
                    this.ReadOutputParameters(command, parameters);
                }
            }
        }

        /// <summary>
        /// Builds and returns a database connection string
        /// </summary>
        /// <param name="databaseName">The name of the database to build the connection string to</param>
        /// <param name="databaseLevel">The level of data (Dev, Test, UET, Live)</param>
        /// <returns>(string)</returns>
        public string GetConnectionString(string databaseName, string databaseLevel)
        {
            string returnValue = null;

            using(SqlConnection connection = new SqlConnection("Data Source=sqldev;Database=ZoellerDatabase;User Id=DBService;Password=z0El13RdBdAt@"))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand("dbo.DatabaseConnectionData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@databaseName", databaseName);
                    command.Parameters.AddWithValue("@databaseLevel", databaseLevel);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            string name = reader.GetString(0);
                            string user = reader.GetString(1);
                            string password = reader.GetString(2);
                            string server = reader.GetString(3);
                            string connectString = reader.GetString(4);
                            returnValue = connectString.FormatString(server, name, user, password);
                        }
                    }
                }
            }

            return returnValue;
        }

        #endregion "Database Stored Connection Information"


        #region "Config file Connection Information"

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcedureName, List<ParameterSQL> parameters)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ToString();
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"DatabaseConnectionString\" in the config file");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }
            string server = ConfigurationManager.AppSettings["SQL_SERVER_NAME"];
            if (server.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_SERVER_NAME\" in the config file");
            }
            string database = ConfigurationManager.AppSettings["SQL_DATABASE_NAME"];
            if (database.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_DATABASE_NAME\" in the config file");
            }
            string user = ConfigurationManager.AppSettings["SQL_USER_ID"];
            if (user.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_USER_ID\" in the config file");
            }
            string password = ConfigurationManager.AppSettings["SQL_PASSWORD"];
            if (password.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_PASSWORD\" in the config file");
            }

            Security.Security security = new Security.Security();

            password = DecryptPassword(password);

            connectionString = connectionString.FormatString(server, database, user, password);

            using (DataSet returnValue = new DataSet())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        this.LoadParameters(command, storedProcedureName, parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(returnValue);
                        }
                        this.ReadOutputParameters(command, parameters);
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        public void ExecuteNonQuery(string storedProcedureName, List<ParameterSQL> parameters)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ToString();
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"DatabaseConnectionString\" in the config file");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }
            string server = ConfigurationManager.AppSettings["SQL_SERVER_NAME"];
            if (server.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_SERVER_NAME\" in the config file");
            }
            string database = ConfigurationManager.AppSettings["SQL_DATABASE_NAME"];
            if (database.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_DATABASE_NAME\" in the config file");
            }
            string user = ConfigurationManager.AppSettings["SQL_USER_ID"];
            if (user.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_USER_ID\" in the config file");
            }
            string password = ConfigurationManager.AppSettings["SQL_PASSWORD"];
            if (password.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"SQL_PASSWORD\" in the config file");
            }

            Security.Security security = new Security.Security();

            password = DecryptPassword(password);

            connectionString = connectionString.FormatString(server, database, user, password);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    this.LoadParameters(command, storedProcedureName, parameters);

                    command.ExecuteNonQuery();
                    this.ReadOutputParameters(command, parameters);
                }
            }
        }

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
        public DataSet ExecuteQuery(string storedProcedureName, List<ParameterSQL> parameters, string cfConStr, string cfServer, string cfDatabase, string cfUser, string cfPassword)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[cfConStr].ToString();
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"DatabaseConnectionString\" in the config file");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }
            string server = ConfigurationManager.AppSettings[cfServer];
            if (server.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfServer + "\" in the config file");
            }
            string database = ConfigurationManager.AppSettings[cfDatabase];
            if (database.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfDatabase + "\" in the config file");
            }
            string user = ConfigurationManager.AppSettings[cfUser];
            if (user.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfUser + "\" in the config file");
            }
            string password = ConfigurationManager.AppSettings[cfPassword];
            if (password.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfPassword + "\" in the config file");
            }

            Security.Security security = new Security.Security();

            password = DecryptPassword(password);

            connectionString = connectionString.FormatString(server, database, user, password);

            using (DataSet returnValue = new DataSet())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        this.LoadParameters(command, storedProcedureName, parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(returnValue);
                        }
                        this.ReadOutputParameters(command, parameters);
                    }
                }

                return returnValue;
            }
        }

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
        public void ExecuteNonQuery(string storedProcedureName, List<ParameterSQL> parameters, string cfConStr, string cfServer, string cfDatabase, string cfUser, string cfPassword)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[cfConStr].ToString();
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"DatabaseConnectionString\" in the config file");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }
            string server = ConfigurationManager.AppSettings[cfServer];
            if (server.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfServer + "\" in the config file");
            }
            string database = ConfigurationManager.AppSettings[cfDatabase];
            if (database.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfDatabase + "\" in the config file");
            }
            string user = ConfigurationManager.AppSettings[cfUser];
            if (user.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfUser + "\" in the config file");
            }
            string password = ConfigurationManager.AppSettings[cfPassword];
            if (password.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"" + cfPassword + "\" in the config file");
            }

            Security.Security security = new Security.Security();

            password = DecryptPassword(password);

            connectionString = connectionString.FormatString(server, database, user, password);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    this.LoadParameters(command, storedProcedureName, parameters);

                    command.ExecuteNonQuery();
                    this.ReadOutputParameters(command, parameters);
                }
            }
        }

        #endregion "Config file Connection Information"


        #region "Received Connection String"

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        public void ExecuteNonQuery(string connectionString, string storedProcedureName, List<ParameterSQL> parameters)
        {
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Invalid connection string");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    this.LoadParameters(command, storedProcedureName, parameters);

                    command.ExecuteNonQuery();
                    this.ReadOutputParameters(command, parameters);
                }
            }
        }

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <returns>(DataSet)</returns>
        public DataSet ExecuteQuery(string connectionString, string storedProcedureName, List<ParameterSQL> parameters)
        {
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Invalid connection string");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }

            using (DataSet returnValue = new DataSet())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        this.LoadParameters(command, storedProcedureName, parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(returnValue);
                        }
                        this.ReadOutputParameters(command, parameters);
                    }
                }

                return returnValue;
            }
        }

        #endregion "Received Connection String"


        #region "Received Config File Name For Connection String"

        /// <summary>
        /// Execute a stored procedure that does not return any data
        /// </summary>
        /// <param name="cfConStr">Configuration file element name for the connection string</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        public void ExecuteNonQuery(string cfConStr, List<ParameterSQL> parameters, string storedProcedureName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[cfConStr].ToString();
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"DatabaseConnectionString\" in the config file");
            }
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Invalid connection string");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    this.LoadParameters(command, storedProcedureName, parameters);

                    command.ExecuteNonQuery();
                    this.ReadOutputParameters(command, parameters);
                }
            }
        }

        /// <summary>
        /// Execute a stored procedure and returns all data returned by the procedure in a Dataset
        /// </summary>
        /// <param name="cfConStr">Configuration file element name for the connection string</param>
        /// <param name="parameters">The list of parameters to pass to the stored procedure</param>
        /// <param name="storedProcedureName">The name of the stored porocedure to execute</param>
        /// <returns>(DataSet)</returns>
        public DataSet ExecuteQuery(string cfConStr, List<ParameterSQL> parameters, string storedProcedureName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[cfConStr].ToString();
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Could not find \"DatabaseConnectionString\" in the config file");
            }
            if (connectionString.IsNullOrEmpty())
            {
                throw new Exception("Invalid connection string");
            }
            if (storedProcedureName.IsNullOrEmpty())
            {
                throw new Exception("Invalid stored procedure name");
            }

            using (DataSet returnValue = new DataSet())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        this.LoadParameters(command, storedProcedureName, parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(returnValue);
                        }
                        this.ReadOutputParameters(command, parameters);
                    }
                }

                return returnValue;
            }
        }

        #endregion "Received Config File Name For Connection String"

        #endregion "Public Methods"


        #region "Private Methods"

        private void LoadParameters(SqlCommand command, string storedProcedureName, List<ParameterSQL> parameters)
        {
            ParameterSQL parm;
            SqlParameter tempParm = null;
            List<ParameterSQL> procedureParms = this.StoredProcedureParameters(command, storedProcedureName);

            command.Parameters.Clear();

            foreach (ParameterSQL parameter in parameters)
            {
                parm = (ParameterSQL)procedureParms.FirstOrDefault(p => p.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
                if (parm != null)
                {
                    if (!parameter.Direction.Equals(ParameterDirection.ReturnValue))
                    {
                        tempParm = command.Parameters.Add(parm.Name, parm.DataType, parm.Size);
                        tempParm.Direction = parm.Direction;
                        tempParm.Value = parameter.Value;
                    }
                    else
                    {
                        tempParm = command.Parameters.Add(parm.Name, SqlDbType.Int, 0);
                        tempParm.Direction = ParameterDirection.ReturnValue;
                    }
                }
            }

            // We'll add the return value paraameter for them
            if ((procedureParms.FindIndex(f => f.Name.Equals("@RETURN_VALUE", StringComparison.CurrentCultureIgnoreCase)) != -1) &&
                (parameters.Count == 0 || parameters.FindIndex(p => p.Name.Equals("@RETURN_VALUE", StringComparison.CurrentCultureIgnoreCase)) == -1))
            {
                command.Parameters.Add("@RETURN_VALUE", SqlDbType.BigInt, 0).Direction = ParameterDirection.ReturnValue;
                parameters.Add(new ParameterSQL
                {
                    Name = "@RETURN_VALUE",
                    DataType = SqlDbType.BigInt,
                    Direction = ParameterDirection.ReturnValue,
                    Size = 0
                });
            }
        }

        /// <summary>
        /// Reads the output and return values of the stored procedure that was executed
        /// </summary>
        /// <param name="command">The SqlCommand object used for the database operation</param>
        /// <param name="parameters">The list of parameter names and values receibved from the caller</param>
        /// <returns>(List of ParameterSQL)</returns>
        private List<ParameterSQL> ReadOutputParameters(SqlCommand command, List<ParameterSQL> parameters)
        {
            for (int p = 0; p < parameters.Count; p++)
            {
                switch (parameters[p].Direction)
                {
                    case ParameterDirection.Output:
                    case ParameterDirection.InputOutput:
                        parameters[p].Value = command.Parameters[parameters[p].Name].Value;
                        parameters[p].Size = command.Parameters[parameters[p].Name].Size;
                        break;
                    case ParameterDirection.ReturnValue:
                        parameters[p].Value = command.Parameters[parameters[p].Name].Value;
                        this._procedureReturn = (int?)command.Parameters[parameters[p].Name].Value;
                        break;
                }
            }
            this._parameters = parameters;

            return parameters;
        }

        private List<ParameterSQL> StoredProcedureParameters(SqlCommand command, string procedureName)
        {
            List<ParameterSQL> returnValue = new List<ParameterSQL>();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = procedureName;
            SqlCommandBuilder.DeriveParameters(command);
            foreach (SqlParameter p in command.Parameters)
            {
                returnValue.Add(new ParameterSQL
                {
                    Name = p.ParameterName,
                    Direction = p.Direction,
                    Size = p.Size,
                    DataType = TypeConvertor.ToSqlDbType(p.DbType)
                });
            }

            return returnValue;
        }

        private string DecryptPassword(string password)
        {
            string returnValue = null;
            Security.Security security = new Security.Security();

            if (password.Length < 800)
            {
                returnValue = Encryptor.Decrypt(password);
            }
            else
            {
                returnValue = security.Decrypt(password);
            }

            if (returnValue.IsNullOrEmpty())
            {
                throw new Exception("Invalid password encryption");
            }

            return returnValue;
        }

        #endregion "Private Methods"
    }
}
