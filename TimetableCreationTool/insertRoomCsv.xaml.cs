using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;

namespace TimetableCreationTool
{
    /// <summary>
    /// Interaction logic for insertRoomCsv.xaml
    /// </summary>
    public partial class insertRoomCsv : Window
    {
        string timetableName;
        public insertRoomCsv(string tName)
        {
            InitializeComponent();
            timetableName = tName;

        }
        // comments the same as insertCourseCSV window but for the lecturer
        // ran out of time to finish comments
        public string userMyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public void openExternalCSVFile_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + "rooms.txt");
        }

        public void uploadCsvData_Click(object sender, RoutedEventArgs e)
        {
            DataTable csvData = getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + "rooms.txt");
            InsertDataTableToSQL(csvData);
            selectIntoDistinct();
            truncateTempAfterCSVInsert();
            
            this.Close();
        }

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
                        // making empty value as null
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
                MessageBox.Show("Issues with the rooms csv file, check the formating");

            }
            return csvData;
        }


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
                            sbc.DestinationTableName = "dbo.roomTemp";

                            foreach (var column in csvFileData.Columns)

                                sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                            sbc.WriteToServer(csvFileData);
                            dbConnection.Close();

                            return true;


                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Rooms not loaded correctly, check CSV file to make sure formatting is correct");
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

        public void selectIntoDistinct()
        {
            string queryString = "INSERT dbo.Room(roomCode,capacity,lab) SELECT roomCode,capacity,lab FROM dbo.roomTemp rt WHERE not exists(SELECT * FROM dbo.Room r WHERE rt.roomCode = r.roomCode);";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, dbConnection);
                dbConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }

        public void truncateTempAfterCSVInsert()
        {
            string queryString = "TRUNCATE TABLE dbo.roomTemp;";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, dbConnection);
                dbConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }


    }
}
