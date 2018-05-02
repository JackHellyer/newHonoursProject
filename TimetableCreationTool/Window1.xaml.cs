using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //private Model1Container dbContext;
        private timetableCreationEntities3 dbcontext;
        
        string userMyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string timetableName;
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";
        //private System.Windows.Data.CollectionViewSource roomsViewSource;

        public Window1(string tName)
        {
            InitializeComponent();
            timetableName = tName;
            this.Title = timetableName;
            bindComboBox(chooseCourse);
            createExampleCSVFile("timetable.txt", "courseId, moduleId, lecturerId, roomId, day, time");
            this.dbcontext = new timetableCreationEntities3();
           
        }
        
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int test = dataGrid.CurrentCell.Column.DisplayIndex;
            string test2 = dataGrid.SelectedIndex.ToString();

            MessageBox.Show(test + ",     " + test2);

        }
        
        public void menuSave_Click(object sender, RoutedEventArgs e)
        {
            saveDbToCSVFile("roomCode,capacity, lab", "rooms.txt", "dbo.Room");
            saveDbToCSVFile("lecturerCode,lecturerName,lecturerDept", "lecturers.txt", "dbo.Lecturer");
            saveDbToCSVFile("courseCode,courseName,noOfStudents", "courses.txt", "dbo.Course");
            saveDbToCSVFile("moduleCode,moduleName", "modules.txt", "dbo.Module");
            saveDbToCSVFile("courseId,moduleId", "coursemodules.txt", "dbo.Course_Module");
            saveDbToCSVFile("courseId, moduleId, lecturerId, roomId, day, time", "timetable.txt", "dbo.Timetable");

        }

       
        public void menuExit_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();

        }

        public void menuViewRooms_Click(object sender, RoutedEventArgs e)
        {
            viewRooms vr = new viewRooms();
            vr.Owner = this;
            vr.ShowDialog();
            //Window_Loaded(sender, e);
        }

        public void menuViewLecturers_Click(object sender, RoutedEventArgs e)
        {
            viewLecturers vl = new viewLecturers();
            vl.Owner = this;
            vl.ShowDialog();
        }

        public void menuViewCourses_Click(object sender, RoutedEventArgs e)
        {
            viewCourses vc = new viewCourses();
            vc.Owner = this;
            vc.ShowDialog();

        }

        public void menuViewModules_Click(object sender, RoutedEventArgs e)
        {
            viewModules vm = new viewModules();
            vm.Owner = this;
            vm.ShowDialog();
        }

        private void menuInsertRoomCSV_Click(object Sender, RoutedEventArgs e)
        {
            
            insertRoomCsv irc = new insertRoomCsv(timetableName);
            irc.Owner = this;
            string fileName = "rooms.txt";
            string tableColumns = "roomCode,capacity,lab";
            createExampleCSVFile(fileName, tableColumns);
            irc.ShowDialog();

            

        }

        private void menuInsertRoomForm_Click(object sender, RoutedEventArgs e)
        {
            menuInsertRoom_Form addRoom = new menuInsertRoom_Form();
            addRoom.Owner = this;
            addRoom.ShowDialog();
            
    
        }

        private void menuInsertModuleForm_Click(object sender, RoutedEventArgs e)
        {
            menuInsertModule_Form addModule = new menuInsertModule_Form();
            addModule.Owner = this;
            addModule.ShowDialog();



        }

        private void menuInsertCourseForm_Click(object sender, RoutedEventArgs e)
        {
            menuInsertCourse_Form addCourse = new menuInsertCourse_Form();
            addCourse.Owner = this;
            addCourse.ShowDialog();

            bindComboBox(chooseCourse);

        }

        private void menuInsertLecturerForm_Click(object sender, RoutedEventArgs e)
        {
            menuInsertLecturer_Form addLecturer = new menuInsertLecturer_Form();
            addLecturer.Owner = this;
            addLecturer.ShowDialog();



        }



        private void menuLecturersCSV_Click(object sender, RoutedEventArgs e)
        {
            insertLecturerCSV ilc = new insertLecturerCSV(timetableName);
            ilc.Owner = this;
            string fileName = "lecturers.txt";
            string tableColumns = "lecturerCode,lecturerName,lecturerDept";
            createExampleCSVFile(fileName, tableColumns);
            ilc.ShowDialog();
        }

        public void menuCoursesCSV_Click(object sender, RoutedEventArgs e)
        {
            insertCourseCSV icc = new insertCourseCSV(timetableName);
            icc.Owner = this;
            string fileName = "courses.txt";
            string tableColumns = "courseCode,courseName,noOfStudents";
            createExampleCSVFile(fileName, tableColumns);
            icc.ShowDialog();

            bindComboBox(chooseCourse);

        }

        public void menuModulesCSV_Click(object sender, RoutedEventArgs e)
        {
            insertModuleCSV imc = new insertModuleCSV(timetableName);
            imc.Owner = this;
            string fileName = "modules.txt";
            string tableColumns = "moduleCode,moduleName";
            createExampleCSVFile(fileName, tableColumns);
            imc.ShowDialog();
        }

        public void saveDbToCSVFile(string columns, string fName, string tableName)
        {
            SqlConnection dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            
            //string command = "SELECT" + 
            SqlCommand selectRooms = new SqlCommand("SELECT " + columns + " FROM " + tableName + ";", dbConnection);
            SqlDataReader sdr = selectRooms.ExecuteReader();

            string fileName = userMyDocumentsPath + "/Timetable App/" + timetableName + "/" + fName;
            StreamWriter sw = new StreamWriter(fileName);
            object[] output = new object[sdr.FieldCount];

            for (int i = 0; i < sdr.FieldCount; i++)
                output[i] = sdr.GetName(i);

            sw.WriteLine(string.Join(",", output));

            while (sdr.Read())
            {
                sdr.GetValues(output);
                sw.WriteLine(string.Join(",", output));
            }

            sw.Close();
            sdr.Close();
            dbConnection.Close();
        }

        /*public void truncateAllTables()
        {
            string queryString = "TRUNCATE TABLE dbo.Course_Module; DELETE FROM dbo.Room DBCC CHECKIDENT ('timetableCreation.dbo.Room', RESEED, 0);  DELETE FROM dbo.Lecturer DBCC CHECKIDENT ('timetableCreation.dbo.Lecturer', RESEED, 0); DELETE FROM dbo.Course DBCC CHECKIDENT ('timetableCreation.dbo.Course', RESEED, 0); DELETE FROM dbo.Module DBCC CHECKIDENT ('timetableCreation.dbo.Module', RESEED, 0);";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, dbConnection);
                dbConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }*/
        DataTable dt = new DataTable();
        public void createDatatable(object sender, RoutedEventArgs e)
        {
            
            dt.Columns.Add("DayName");
            dt.Columns.Add("08:00");
            dt.Columns.Add("09:00");
            dt.Columns.Add("10:00");
            dt.Columns.Add("11:00");
            dt.Columns.Add("12:00");
            dt.Columns.Add("13:00");
            dt.Columns.Add("14:00");
            dt.Columns.Add("15:00");
            dt.Columns.Add("16:00");
            dt.Columns.Add("17:00");

            DataRow dr = dt.NewRow();
            dr["DayName"] = "Monday";
            dt.Rows.Add(dr);

            DataRow dr2 = dt.NewRow();
            dr2["DayName"] = "Tuesday";
            dt.Rows.Add(dr2);

            DataRow dr3 = dt.NewRow();
            dr3["DayName"] = "Wednesday";
            dt.Rows.Add(dr3);

            
            DataRow dr4 = dt.NewRow();
            dr4["DayName"] = "Thursday";
            dt.Rows.Add(dr4);

            DataRow dr5 = dt.NewRow();
            dr5["DayName"] = "Friday";
            dt.Rows.Add(dr5);

            dataGrid.ItemsSource = dt.DefaultView;
        }

        public void bindComboBox(ComboBox comboBoxName)
        {
            string query = "select courseId, courseName from dbo.Course ORDER BY courseCode";
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            sda.Fill(ds, "dbo.Course");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["courseName"].ToString();
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["courseId"].ToString();
        }

       

        public void createExampleCSVFile(string fileName, string tableColumns)
        {
            string pathToCreateCSVFile = userMyDocumentsPath + "/Timetable App/" + timetableName +"/" + fileName;


            if (!File.Exists(pathToCreateCSVFile))
            {
                try
                {
                    using (FileStream fs = File.Create(pathToCreateCSVFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(tableColumns);
                        fs.Write(info, 0, info.Length);

                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                }
            }
           

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save before exiting", "Exit", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                
            }
            else
            {
                saveDbToCSVFile("roomCode,capacity, lab", "rooms.txt", "dbo.Room");
                saveDbToCSVFile("lecturerCode,lecturerName,lecturerDept", "lecturers.txt", "dbo.Lecturer");
                saveDbToCSVFile("courseCode,courseName,noOfStudents", "courses.txt", "dbo.Course");
                saveDbToCSVFile("moduleCode,moduleName", "modules.txt", "dbo.Module");
                saveDbToCSVFile("courseId,moduleId", "coursemodules.txt", "dbo.Course_Module");
                saveDbToCSVFile("courseId, moduleId, lecturerId, roomId, day, time", "timetable.txt", "dbo.Timetable");
                saveDbToCSVFile("lecturerId, moduleId", "lecturermodules.txt", "dbo.Lecturer_Module");
            }
        }

        /*private int insertTodataGrid(string day, string time)
        {
            string query = "SELECT tId FROM Timetable WHERE courseId = @courseId AND day = @day AND time = @time;";
            int tId;
            using(var conn = new SqlConnection(dbConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@courseId", chooseCourse.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@day", day);
                cmd.Parameters.AddWithValue("@time", time);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        tId = reader.GetInt32(reader.GetOrdinal("tId"));
                        return tId;
                    }
                    else
                    {
                        tId = 0;
                        return tId;
                    }
                }
            }
        }*/

        private void chooseCourse_DropDownClosed(object sender, EventArgs e)
        {
            
            if(chooseCourse.SelectedItem != null)
            {
                dt.Clear();


                DataRow dr1 = dt.NewRow();
                dr1["DayName"] = "Monday";
                dt.Rows.Add(dr1);

                DataRow dr2 = dt.NewRow();
                dr2["DayName"] = "Tuesday";
                dt.Rows.Add(dr2);

                DataRow dr3 = dt.NewRow();
                dr3["DayName"] = "Wednesday";
                dt.Rows.Add(dr3);


                DataRow dr4 = dt.NewRow();
                dr4["DayName"] = "Thursday";
                dt.Rows.Add(dr4);

                DataRow dr5 = dt.NewRow();
                dr5["DayName"] = "Friday";
                dt.Rows.Add(dr5);

                //dataGrid.ItemsSource = dt.DefaultView;

               // MessageBox.Show(chooseCourse.SelectedValue.ToString());
                string fileName = "coursemodules.txt";
                string tableColumns = "courseId,moduleId";
                createExampleCSVFile(fileName, tableColumns);

                addModulesStudied ams = new addModulesStudied(chooseCourse.Text, chooseCourse.SelectedValue.ToString());
                ams.Owner = this;
                ams.ShowDialog();

                refreshDatagrid();
                
            }
            


        }

        private void refreshDatagrid()
        {
            string day;
            string time;



            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if ((dr[dc].ToString() != "Monday") && (dr[dc].ToString() != "Tuesday") && (dr[dc].ToString() != "Wednesday") && (dr[dc].ToString() != "Thursday") && (dr[dc].ToString() != "Friday"))
                    {
                        dr[dc] = "";
                    }
                    
                        day = dr[0].ToString();
                        time = dc.ColumnName.ToString();
                        //MessageBox.Show(day + " time: " + time);

                        string query = "SELECT m.moduleName, r.roomCode, l.lecturerName FROM Timetable t, Module m, Room r, Lecturer l WHERE t.moduleId = m.moduleId AND t.roomId = r.roomId AND t.lecturerId = l.lecturerId AND courseId = @courseId AND day = @day AND time = @time;";
                        string mName;
                        string rCode;
                        string lName;
                        //string mName;
                        //string rCode;
                        using (var conn = new SqlConnection(dbConnectionString))
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@courseId", chooseCourse.SelectedValue.ToString());
                            cmd.Parameters.AddWithValue("@day", day);
                            cmd.Parameters.AddWithValue("@time", time);
                            conn.Open();
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    mName = reader.GetString(reader.GetOrdinal("moduleName"));
                                    rCode = reader.GetString(reader.GetOrdinal("roomCode"));
                                    lName = reader.GetString(reader.GetOrdinal("lecturerName"));
                                    //line below inserts into cell
                                    dr[dc] = " Module: " + mName + "\n Room:    " + rCode + "\n Lecturer: " + lName;


                                }



                            }
                        }
                    
                    
                 
                       


                    }
                
                
                   
            }
        }
        private void dataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(dataGrid.SelectedIndex.ToString());
            
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {


            if (dataGrid.Items.IndexOf(dataGrid.CurrentItem) < 5)
            {
                if (chooseCourse.SelectedItem != null)
                {
                    if (dataGrid.CurrentCell.Column != null)
                    {
                        int columnIndex = dataGrid.CurrentCell.Column.DisplayIndex;

                        int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                        DataRowView v = (DataRowView)dataGrid.Items[rowIndex];
                        string day = (string)v[0];
                        string timeString = (string)dataGrid.Columns[columnIndex].Header;


                        string cName = chooseCourse.Text;
                        //MessageBox.Show((string)v.ToString());
                        //MessageBox.Show(day + " and    " + timeString);
                        string cId = chooseCourse.SelectedValue.ToString();


                        string day2;
                        string time;
                        foreach (DataRow dr in dt.Rows)
                        {
                            foreach (DataColumn dc in dt.Columns)
                            {
                                day2 = dr[0].ToString();
                                time = dc.ColumnName.ToString();
                                if ((day2 == day) && (time == timeString) && (dr[dc].ToString() == ""))
                                {
                                    insertTimetable it = new insertTimetable(day, timeString, cId, cName);
                                    it.Owner = this;
                                    it.ShowDialog();


                                    //MessageBox.Show(day + " time: " + time);
                                    //dataGrid.CurrentCell.Column = null;
                                    //dataGrid.ItemsSource = dt.DefaultView;
                                    refreshDatagrid();


                                }
                                else if((day2 == day) && (time == timeString) && (dr[dc].ToString() != "") && (dr[dc].ToString() != "Monday") && (dr[dc].ToString() != "Tuesday") && (dr[dc].ToString() != "Wednesday") && (dr[dc].ToString() != "Thursday") && (dr[dc].ToString() != "Friday"))
                                {
                                    editTimetable editTtable = new editTimetable(day, timeString, cId, cName);
                                    editTtable.Owner = this;
                                    editTtable.ShowDialog();
                                    /*if(DialogResult == true)
                                    {
                                        dr[dc] = "";
                                    }*/
                                    //chooseCourse.SelectedIndex = 2;
                                    refreshDatagrid();

                                }

                                

                            }
                        }
                    }




                }
                else
                {
                    MessageBox.Show("Must choose course");
                }




            }
        }

        private void assignLecturerButton_Click(object sender, RoutedEventArgs e)
        {
            assignLecturer_Module alm = new assignLecturer_Module();

            alm.ShowDialog();
            createExampleCSVFile("lecturermodules.txt", "lecturerId, moduleId");
        }

       
    }
}
