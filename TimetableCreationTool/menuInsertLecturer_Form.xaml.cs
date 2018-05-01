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
    /// Interaction logic for menuInsertLecturer_Form.xaml
    /// </summary>
    public partial class menuInsertLecturer_Form : Window
    {
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public menuInsertLecturer_Form()
        {
            InitializeComponent();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            string lecturerCode = lecturerCodeTextbox.Text;
            string lecturerName = lecturerNameTextbox.Text;
            string lecturerDept = lecturerDeptTextbox.Text;

            if (lecturerCode != "" && lecturerName != "" && lecturerDept != "")
            {

                if (lecturerCode.Length < 16 && lecturerName.Length < 76 && lecturerDept.Length < 76)
                {
                    SqlConnection conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.lecturerTemp(lecturerCode, lecturerName, lecturerDept) VALUES(@lecturerCode, @lecturerName, @lecturerDept);", conn);
                    command.Parameters.AddWithValue("@lecturerCode", lecturerCode);
                    command.Parameters.AddWithValue("@lecturerName", lecturerName);
                    command.Parameters.AddWithValue("@lecturerDept", lecturerDept);


                    command.ExecuteNonQuery();

                    int number = selectIntoDistinct("INSERT dbo.Lecturer(lecturerCode,lecturerName,lecturerDept) SELECT lecturerCode,lecturerName,lecturerDept FROM dbo.lecturerTemp lt WHERE not exists(SELECT * FROM dbo.Lecturer l WHERE lt.lecturerCode = l.lecturerCode);");

                    if (number == 1)
                    {
                        this.Close();
                        truncateTempAfterInsert("TRUNCATE TABLE dbo.lecturerTemp;");
                    }
                    else
                    {

                        MessageBox.Show("Lecturer Code already exists");
                    }


                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Lecturer code can't be greater than 15 characters and lecturer name and department can't be greater than 75 characters");

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
