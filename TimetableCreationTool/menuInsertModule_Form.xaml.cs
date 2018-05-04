using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimetableCreationTool
{
    /// <summary>
    /// Interaction logic for menuInsertModule_Form.xaml
    /// </summary>
    /// 
    // same as menu insert course comments
    // didn't have time to add comments
    public partial class menuInsertModule_Form : Window
    {
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public menuInsertModule_Form()
        {
            InitializeComponent();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            string moduleCode = moduleCodeTextbox.Text;
            string moduleName = moduleNameTextbox.Text;

            if (moduleCode != "" && moduleName != "" )
            {
                
                if (moduleCode.Length < 16 && moduleName.Length < 76)
                {
                    SqlConnection conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.moduleTemp(moduleCode, moduleName) values(@moduleCode, @moduleName);", conn);
                    command.Parameters.AddWithValue("@moduleCode", moduleCode);
                    command.Parameters.AddWithValue("@moduleName", moduleName);
                    

                    command.ExecuteNonQuery();

                    int number = selectIntoDistinct("INSERT dbo.Module(moduleCode, moduleName) SELECT moduleCode, moduleName FROM dbo.moduleTemp mt WHERE not exists(SELECT * FROM dbo.Module m WHERE mt.moduleCode = m.moduleCode);");

                    if (number == 1)
                    {
                        this.Close();
                        truncateTempAfterInsert("TRUNCATE TABLE dbo.moduleTemp;");
                    }
                    else
                    {

                        MessageBox.Show("Module already exists");
                    }


                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Module code can't be greater than 15 characters and moduleName can't be greater than 75 characters");

                }
            }
            else
            {
                MessageBox.Show("fields can't be empty");
            }




        }

        public int selectIntoDistinct(string query)
        {
            //string queryString = "INSERT dbo.Room(roomCode,capacity,lab) SELECT roomCode,capacity,lab FROM dbo.roomTemp rt WHERE not exists(SELECT * FROM dbo.Room r WHERE rt.roomCode = r.roomCode);";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                int numberOfChanges = command.ExecuteNonQuery();
                //MessageBox.Show(numberOfChanges.ToString());

                //SqlDataReader reader = command.ExecuteReader();               
                /*
                int numOfRows = command.ExecuteNonQuery();
                MessageBox.Show(numOfRows.ToString());*/
                dbConnection.Close();
                return numberOfChanges;
                //return numOfRows;

            }
        }

        public void truncateTempAfterInsert(string query)
        {
            //string queryString = "TRUNCATE TABLE dbo.roomTemp;";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(query, dbConnection);
                dbConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }
    
    }
}
