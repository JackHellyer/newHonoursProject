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
    /// Interaction logic for menuInsertCourse_Form.xaml
    /// </summary>
    public partial class menuInsertCourse_Form : Window
    {
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public menuInsertCourse_Form()
        {
            InitializeComponent();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            string courseCode = courseCodeTextbox.Text;
            string courseName = courseNameTextbox.Text;
            string noOfStudentsString = noOfStudentsTextbox.Text;

            if (courseCode != "" && courseName != "" && noOfStudentsString != "")
            {
                int noOfStudents;
                if (courseCode.Length < 16 && courseName.Length < 76 && int.TryParse(noOfStudentsString, out noOfStudents))
                {
                    SqlConnection conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.courseTemp(courseCode, courseName, noOfStudents) values(@courseCode, @courseName, @noOfStudents);", conn);
                    command.Parameters.AddWithValue("@courseCode", courseCode);
                    command.Parameters.AddWithValue("@courseName", courseName);
                    command.Parameters.AddWithValue("@noOfStudents", noOfStudents);


                    command.ExecuteNonQuery();

                    int number = selectIntoDistinct("INSERT dbo.Course(courseCode,courseName,noOfStudents) SELECT courseCode,courseName,noOfStudents FROM dbo.courseTemp ct WHERE not exists(SELECT * FROM dbo.Course c WHERE ct.courseCode = c.courseCode);");

                    if (number == 1)
                    {
                        this.Close();
                        truncateTempAfterInsert("TRUNCATE TABLE dbo.courseTemp;");
                    }
                    else
                    {

                        MessageBox.Show("Course already exists");
                    }


                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Course code can't be greater than 15 characters and course name can't be greater than 75 characters and number of students must be a number");

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

        private void courseCodeTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

