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

        // on save event handler
        private void OnSave(object sender, RoutedEventArgs e)
        {
            // assign user inputs to strings
            string courseCode = courseCodeTextbox.Text;
            string courseName = courseNameTextbox.Text;
            string noOfStudentsString = noOfStudentsTextbox.Text;
            // if all inputs are not empty
            if (courseCode != "" && courseName != "" && noOfStudentsString != "")
            {
                int noOfStudents;
                // validation i keeping with sql table structure
                if (courseCode.Length < 16 && courseName.Length < 76 && int.TryParse(noOfStudentsString, out noOfStudents))
                {
                    SqlConnection conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    // insert a course
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.courseTemp(courseCode, courseName, noOfStudents) values(@courseCode, @courseName, @noOfStudents);", conn);
                    command.Parameters.AddWithValue("@courseCode", courseCode);
                    command.Parameters.AddWithValue("@courseName", courseName);
                    command.Parameters.AddWithValue("@noOfStudents", noOfStudents);


                    command.ExecuteNonQuery();
                    // if a row is added 
                    int number = selectIntoDistinct("INSERT dbo.Course(courseCode,courseName,noOfStudents) SELECT courseCode,courseName,noOfStudents FROM dbo.courseTemp ct WHERE not exists(SELECT * FROM dbo.Course c WHERE ct.courseCode = c.courseCode);");

                    if (number == 1)
                    {
                        // delete course temp data
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
           
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                int numberOfChanges = command.ExecuteNonQuery();
               
                dbConnection.Close();
                return numberOfChanges;
                

            }
        }

        public void truncateTempAfterInsert(string query)
        {
            
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

