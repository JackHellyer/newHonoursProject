using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for insertCourseCSV.xaml
    /// </summary>
    public partial class insertCourseCSV : Window
    {
        string timetableName;
        public insertCourseCSV(string tName)
        {
            InitializeComponent();
            timetableName = tName;
        }

        // users my documents folder path
        public string userMyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // sql connection string
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        // method used to open the course CSV file
        public void openExternalCSVFile_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + "courses.txt");
        }

        // method to upload the data from the csv file to the database
        public void uploadCsvData_Click(object sender, RoutedEventArgs e)
        {
            DataTable csvData = getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + "courses.txt");
            InsertDataTableToSQL(csvData);
            selectIntoDistinct();
            truncateTempAfterCSVInsert();
            this.Close();
        }

        // returns datatable takes in the file path of the csv file
        public DataTable getDataTableCSVFile(string filePath)
        {

            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(filePath))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] columnFields = csvReader.ReadFields();
                    foreach (string column in columnFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }

                        }
                        csvData.Rows.Add(fieldData);

                    }
                    csvReader.Close();
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("not working");
                MessageBox.Show("Check formating of the course CSV file");

            }
            return csvData;
        }

        // insert datatable to the database takes iin datatable, returns bool for use on the main window to check if the insert was successful
        public bool InsertDataTableToSQL(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {


                dbConnection.Open();
                if (dbConnection.State == ConnectionState.Open)
                {
                    try
                    {
                        
                        using (SqlBulkCopy sbc = new SqlBulkCopy(dbConnection))
                        {
                            // change this method later to have a string parameter which will hold the destination table
                            sbc.DestinationTableName = "courseTemp";

                            foreach (var column in csvFileData.Columns)

                                sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                            sbc.WriteToServer(csvFileData);
                            dbConnection.Close();
                            return true;



                        }
                    }
                    catch
                    {
                        MessageBox.Show("Courses not loaded correctly, check CSV file to make sure formatting is correct");
                        return false;
                    }
                    
                }
                else
                {
                    MessageBox.Show("connection failed");
                    return false;
                }





            }
        }

        // copy distinct rows from the courseTemp table to the real course table
        public void selectIntoDistinct()
        {
            string queryString = "INSERT dbo.Course(courseCode,courseName,noOfStudents) SELECT courseCode,courseName,noOfStudents FROM dbo.courseTemp ct WHERE not exists(SELECT * FROM dbo.Course c WHERE ct.courseCode = c.courseCode);";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, dbConnection);
                dbConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }

        // truncate
        public void truncateTempAfterCSVInsert()
        {
            string queryString = "TRUNCATE TABLE dbo.courseTemp;";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, dbConnection);
                dbConnection.Open();
                // delete the sql datareader later
                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }
    }
}
