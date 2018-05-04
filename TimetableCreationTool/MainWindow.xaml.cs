﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace TimetableCreationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        


        public MainWindow()
        {
            InitializeComponent();
        }
        public string userMyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public void newTimetable_Click(object sender, RoutedEventArgs e)
        {
            newTimetableDialog nt = new newTimetableDialog();
            nt.Show();
            this.Close();
            
           
        }
        
        
        public void loadTimetable_Click(object sender, RoutedEventArgs e)
        {
            // clear all database table incase the program crashes
            truncateAllTables();
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                fbd.SelectedPath = userMyDocumentsPath + @"\" + @"Timetable App";
                fbd.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    int index = fbd.SelectedPath.LastIndexOf(@"\");
                    string tName = fbd.SelectedPath.Substring(index + 1);
                    //MessageBox.Show(tName);
                    insertRoomCsv irc = new insertRoomCsv(tName);
                    insertLecturerCSV ilc = new insertLecturerCSV(tName);
                    insertCourseCSV icc = new insertCourseCSV(tName);
                    insertModuleCSV imc = new insertModuleCSV(tName);
                    bool ifValid = ifVaildLoadFile(fbd.SelectedPath);
                    if(ifValid)
                    {
                        bool roomSuccess = false;
                        bool lecturerSuccess = false;
                        bool courseSuccess = false;
                        bool moduleSuccess = false;

                        if(File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "rooms.txt"))
                        {
                            DataTable roomCSV = irc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "rooms.txt");
                            roomSuccess = irc.InsertDataTableToSQL(roomCSV);

                            if(roomSuccess)
                            {
                                //irc.InsertDataTableToSQL(roomCSV);
                                irc.selectIntoDistinct();
                                irc.truncateTempAfterCSVInsert();
                            }
                            
                        }
                        if (File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "lecturers.txt"))
                        {

                            DataTable lecturerCSV = irc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "lecturers.txt");
                            lecturerSuccess = ilc.InsertDataTableToSQL(lecturerCSV);
                            
                            if(lecturerSuccess)
                            {
                                ilc.selectIntoDistinct();
                                ilc.truncateTempAfterCSVInsert();
                            }
                            
                        }

                        if (File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "courses.txt"))
                        {
                            DataTable courseCSV = icc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "courses.txt");
                            courseSuccess = icc.InsertDataTableToSQL(courseCSV);

                            if(courseSuccess)
                            {
                                icc.selectIntoDistinct();
                                icc.truncateTempAfterCSVInsert();
                            }
                            
                        }

                        if (File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "modules.txt"))
                        {
                            DataTable modulesCSV = imc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "modules.txt");
                            moduleSuccess = imc.InsertDataTableToSQL(modulesCSV);

                            if(moduleSuccess)
                            {
                                imc.selectIntoDistinct();
                                imc.truncateTempAfterCSVInsert();
                            }
                            


                        }

                           
                        if(File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "coursemodules.txt"))
                        {
                            DataTable courseModuleCSV = imc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "coursemodules.txt");
                            InsertDataTableToSQL(courseModuleCSV);
                        }

                        if (File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "timetable.txt"))
                        {
                            DataTable timetableCSV = imc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "timetable.txt");
                            InsertDataTableToSQLTimetable(timetableCSV);
                        }

                        if (File.Exists(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "lecturermodules.txt"))
                        {
                            DataTable lecturerModuleCSV = imc.getDataTableCSVFile(userMyDocumentsPath + "/Timetable App/" + tName + "/" + "lecturermodules.txt");
                            InsertDataTableToSQLlecturerModules(lecturerModuleCSV);
                        }

                        /* if(roomSuccess && moduleSuccess && courseSuccess && lecturerSuccess)
                        {*/
                            Window1 win1 = new Window1(tName);
                            win1.Show();
                            irc.Close();
                            ilc.Close();
                            icc.Close();
                            imc.Close();
                            this.Close();
                        //}

                        
                    }
                    else
                    {
                        MessageBox.Show("Timetable must be loaded from the Timetable App folder");
                    }
                }
                    


            }
                

        }
        // validate taht the file is inside of the Timetable app folder
        private bool ifVaildLoadFile(string t)
        {
             bool ifValid = false;
             var dir = Directory.GetDirectories(userMyDocumentsPath + @"\Timetable App\");
             foreach (var i in dir)
             { 
                //MessageBox.Show("i:" + i);
                if (t == i)
                {
                    
                    
                    ifValid = true;
                    break;
                    
                
                }
                else
                {
                    ifValid = false;
                    
                    
                }
       
             }
             return ifValid;
        }

        public void InsertDataTableToSQL(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {


                dbConnection.Open();
                if (dbConnection.State == ConnectionState.Open)
                {

                    //MessageBox.Show("connection success");
                    using (SqlBulkCopy sbc = new SqlBulkCopy(dbConnection))
                    {
                        // change this method later to have a string parameter which will hold the destination table
                        sbc.DestinationTableName = "Course_Module";

                        foreach (var column in csvFileData.Columns)

                            sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                        sbc.WriteToServer(csvFileData);
                        dbConnection.Close();



                    }
                }
                else
                {
                    MessageBox.Show("connection failed");
                }





            }
        }

        public void InsertDataTableToSQLTimetable(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {


                dbConnection.Open();
                if (dbConnection.State == ConnectionState.Open)
                {
                    
                        //MessageBox.Show("connection success");
                        using (SqlBulkCopy sbc = new SqlBulkCopy(dbConnection))
                        {
                            // change this method later to have a string parameter which will hold the destination table
                            sbc.DestinationTableName = "Timetable";

                            foreach (var column in csvFileData.Columns)

                                sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                            sbc.WriteToServer(csvFileData);
                            dbConnection.Close();



                        }
                    


                   
                }
                else
                {
                    MessageBox.Show("connection failed");
                }





            }
        }

        public void InsertDataTableToSQLlecturerModules(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {


                dbConnection.Open();
                if (dbConnection.State == ConnectionState.Open)
                {

                    //MessageBox.Show("connection success");
                    using (SqlBulkCopy sbc = new SqlBulkCopy(dbConnection))
                    {
                        // change this method later to have a string parameter which will hold the destination table
                        sbc.DestinationTableName = "Lecturer_Module";

                        foreach (var column in csvFileData.Columns)

                            sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                        sbc.WriteToServer(csvFileData);
                        dbConnection.Close();



                    }
                }
                else
                {
                    MessageBox.Show("connection failed");
                }





            }
        }

        public void truncateAllTables()
        {
            string queryString = "TRUNCATE TABLE dbo.Course_Module; TRUNCATE TABLE dbo.Lecturer_Module; TRUNCATE TABLE dbo.Timetable; DELETE FROM dbo.Room DBCC CHECKIDENT ('timetableCreation.dbo.Room', RESEED, 0); DELETE FROM dbo.Lecturer DBCC CHECKIDENT ('timetableCreation.dbo.Lecturer', RESEED, 0); DELETE FROM dbo.Course DBCC CHECKIDENT ('timetableCreation.dbo.Course', RESEED, 0); DELETE FROM dbo.Module DBCC CHECKIDENT ('timetableCreation.dbo.Module', RESEED, 0); TRUNCATE TABLE dbo.lecturerTemp;";
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
