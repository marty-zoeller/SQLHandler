using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using DataProvider;
using System.Configuration;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string pword = ConfigurationManager.AppSettings["SQL_PASSWORD"];
            Console.WriteLine(pword.Length);
            SQLHandler data = new SQLHandler();
            //ParameterSQL
            string conn = data.GetConnectionString("ZPC_App_Dev", "Dev");
            Console.WriteLine(conn);
            //DataSet result = data.ExecuteQuery("CFE_Kiosk", "Dev", "StatesGET", null);
            DataSet result = data.ExecuteQuery("StatesGET", null);
            if (result.Tables.Count > 0)
            {
                foreach(DataRow row in result.Tables[0].Rows)
                {
                    Debug.WriteLine(row["StateName"].ToString());
                }
            }
        }
    }
}
