using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TimetableCreationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";
        
        public void onExit(object sender, ExitEventArgs e)
        {
            
            
            truncateAllTables();
        }
        // delete data from on the table if the user exits the program by clicking the red cross
        public void truncateAllTables()
        {
            string queryString = "TRUNCATE TABLE dbo.Course_Module; TRUNCATE TABLE dbo.Lecturer_Module; TRUNCATE TABLE dbo.Timetable; DELETE FROM dbo.Room DBCC CHECKIDENT ('timetableCreation.dbo.Room', RESEED, 0); DELETE FROM dbo.Lecturer DBCC CHECKIDENT ('timetableCreation.dbo.Lecturer', RESEED, 0); DELETE FROM dbo.Course DBCC CHECKIDENT ('timetableCreation.dbo.Course', RESEED, 0); DELETE FROM dbo.Module DBCC CHECKIDENT ('timetableCreation.dbo.Module', RESEED, 0);";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, dbConnection);
                dbConnection.Open();

               
                    command.ExecuteNonQuery();

                dbConnection.Close();


            }
        }
    }
}
