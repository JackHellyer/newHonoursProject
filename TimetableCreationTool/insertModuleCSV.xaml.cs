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
    /// Interaction logic for insertModuleCSV.xaml
    /// </summary>
    public partial class insertModuleCSV : Window
    {
        string timetableName;
        public insertModuleCSV(string tName)
        {
            InitializeComponent();
            timetableName = tName;
        }

        public string userMyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public void openExternalCSVFile_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + "modules.txt");
        }

        public void uploadCsvData_Click(object sender, RoutedEventArgs e)
        {
            DataTable csvData = getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + "modules.txt");
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
                MessageBox.Show(ex.Message);

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
                        //MessageBox.Show("connection success");
                        using (SqlBulkCopy sbc = new SqlBulkCopy(dbConnection))
                        {
                            // change this method later to have a string parameter which will hold the destination table
                            sbc.DestinationTableName = "moduleTemp";

                            foreach (var column in csvFileData.Columns)

                                sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                            sbc.WriteToServer(csvFileData);
                            dbConnection.Close();
                            return true;



                        }
                    }
                    catch
                    {
                        MessageBox.Show("Modules not loaded correctly, check CSV file to make sure formatting is correct");
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
            string queryString = "INSERT dbo.Module(moduleCode,moduleName) SELECT moduleCode,moduleName FROM dbo.moduleTemp mt WHERE not exists(SELECT * FROM dbo.Module m WHERE mt.moduleCode = m.moduleCode);";
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
            string queryString = "TRUNCATE TABLE dbo.moduleTemp;";
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
