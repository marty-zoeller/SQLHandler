using System.Data;

namespace DataProvider
{
    /// <summary>
    /// Parameters that are to be passed to a stored procedure.
    /// </summary>
    public class ParameterSQL
    {
        /// <summary>
        /// REQUIRED - Get or Set - The name of the parameter. You must include the "@"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// REQUIRED - Get or Set - The value to pass to the SQL stored procedure (may be null)
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// OPTIONAL - Get or Set - The direction of the parameter (System.Data.ParameterDirection).  Is it Input, Output, Input and Output or a Return Value.
        ///                         This will be set for you.
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// OPTIONAL - Get or Set - The data type of the parameter (System.Data.SqlDbType)
        ///                         This will be set for you.
        /// </summary>
        public SqlDbType DataType { get; set; }

        /// <summary>
        /// OPTIONAL - Get or Set - The size or length of the parameter
        ///                         This will be set for you.
        /// </summary>
        public int Size { get; set; }
    }
}
