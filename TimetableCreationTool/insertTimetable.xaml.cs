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
    /// Interaction logic for insertTimetable.xaml
    /// </summary>
    public partial class insertTimetable : Window
    {
        
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";
        string cId;
        public insertTimetable(string day, string time, string courseId, string courseName)
        {
            InitializeComponent();

            dayTextBox.Text = day;
            timeTextBox.Text = time;
            cId = courseId;
            courseNameTextBox.Text = courseName;
            bindComboBox(moduleCombobox);
            bindComboBox1(roomCombobox);
            //bindComboBox2(lecturercomboBox);

        }

        public void bindComboBox(ComboBox comboBoxName)
        {
            string query = "select cm.moduleId, cm.courseId, m.moduleCode, m.moduleName from dbo.Module m, dbo.Course_Module cm WHERE m.moduleId = cm.moduleId AND cm.courseId = " + cId + " ORDER BY m.moduleName";
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            sda.Fill(ds, "dbo.Module");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["moduleName"].ToString();
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["moduleId"].ToString();
        }

        public void bindComboBox1(ComboBox comboBoxName)
        {


            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter("select r.roomId, r.roomCode from dbo.Room r, dbo.Course c WHERE c.courseId = @cId AND c.noOfStudents <= r.capacity ORDER BY r.roomCode;", conn);
            sda.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@cId",
                    Value = cId
                    
            });
                //sda.SelectCommand.Parameters.AddWithValue("@day", dayTextBox.Text);
                DataSet ds = new DataSet();
                sda.Fill(ds, "dbo.Room");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["roomCode"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["roomId"].ToString();

            conn.Close();


            // string query = "select r.roomId, r.roomCode from dbo.Room r, dbo.Course c, dbo.Timetable t WHERE c.courseId = " + cId + " AND c.noOfStudents <= r.capacity AND t.day <> " + dayTextBox.Text  + " ORDER BY roomCode;";

            
            
        }

        
        public void bindComboBox2(ComboBox comboBoxName)
        {
            if(moduleCombobox.SelectedItem != null)
            {
                int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                //MessageBox.Show(moduleId.ToString());
                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select lm.lecturerId, lm.moduleId, l.lecturerName from dbo.Lecturer l, dbo.Lecturer_Module lm WHERE lm.lecturerId = l.lecturerId AND lm.moduleId = @moduleId ORDER BY l.lecturerName;", conn);
                sda.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@moduleId",
                    Value = moduleId

                });
                //sda.SelectCommand.Parameters.AddWithValue("@day", dayTextBox.Text);
                DataSet ds = new DataSet();
                sda.Fill(ds, "dbo.Lecturer");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["lecturerName"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["lecturerId"].ToString();

                conn.Close();
            }

            


            // string query = "select r.roomId, r.roomCode from dbo.Room r, dbo.Course c, dbo.Timetable t WHERE c.courseId = " + cId + " AND c.noOfStudents <= r.capacity AND t.day <> " + dayTextBox.Text  + " ORDER BY roomCode;";



        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (moduleCombobox.SelectedItem != null && roomCombobox.SelectedItem != null && lecturercomboBox.SelectedItem != null)
            {
                int courseId = int.Parse(cId);
                int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                int roomId = int.Parse(roomCombobox.SelectedValue.ToString());
                int lecturerId = int.Parse(lecturercomboBox.SelectedValue.ToString());

                //string query = "INSERT INTO Course_Module (courseId, moduleId) VALUES(" + courseId + "," + moduleId + ");";
                //string query2 = "INSERT dbo.Course_Module (courseId, moduleId) SELECT " + courseId + "," + moduleId + " WHERE NOT EXISTS( SELECT courseId, moduleId FROM dbo.Course_Module WHERE courseId = " + courseId + " AND moduleId = " + moduleId + ");";

                //"INSERT INTO dbo.Timetable (courseId, moduleId, roomId, day, time) VALUES(@courseId, @moduleId, @roomId, @day, @time);"
                //INSERT dbo.Timetable(courseId, moduleId, roomId, day, time) SELECT @courseId, @moduleId, @roomId, @day, @time WHERE NOT EXISTS(SELECT courseId, moduleId, roomId, day, time FROM dbo.Timetable WHERE(courseId = @courseId AND moduleId = @moduleId) AND(roomId = @roomId AND day = @day AND time = @time));


                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();

                SqlCommand checkRoom = new SqlCommand("SELECT COUNT(*) FROM dbo.timetable WHERE roomId= @roomId AND day = @day AND time = @time;", conn);
                checkRoom.Parameters.AddWithValue("@roomId", roomId);
                checkRoom.Parameters.AddWithValue("@day", dayTextBox.Text);
                checkRoom.Parameters.AddWithValue("@time", timeTextBox.Text);

                int roomBeingUsed = (int)checkRoom.ExecuteScalar();

                SqlCommand checkLecturer = new SqlCommand("SELECT COUNT(*) FROM dbo.Timetable WHERE lecturerId = @lecturerId and day = @day AND time = @time;", conn);
                checkLecturer.Parameters.AddWithValue("@lecturerId", lecturerId);
                checkLecturer.Parameters.AddWithValue("@day", dayTextBox.Text);
                checkLecturer.Parameters.AddWithValue("@time", timeTextBox.Text);

                int lecturerBusy = (int)checkLecturer.ExecuteScalar();

                if (roomBeingUsed > 0)
                {
                    MessageBox.Show("Room being used at selected time");
                }
                else if (lecturerBusy > 0)
                {
                    MessageBox.Show("Lecturer busy");
                }
                else
                {
                    SqlCommand command = new SqlCommand("INSERT dbo.Timetable(courseId, moduleId, lecturerId, roomId, day, time) SELECT @courseId, @moduleId, @lecturerId, @roomId, @day, @time WHERE NOT EXISTS(SELECT courseId, moduleId, lecturerId, roomId, day, time FROM dbo.Timetable WHERE(roomId = @roomId AND day = @day AND time = @time));", conn);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@day", dayTextBox.Text);
                    command.Parameters.AddWithValue("@time", timeTextBox.Text);
                    command.Parameters.AddWithValue("@lecturerId", lecturerId);
                    int numOfRowsEffected = command.ExecuteNonQuery();

                    //command.ExecuteNonQuery();
                    //MessageBox.Show(numOfRowsEffected.ToString());
                    conn.Close();
                    if (numOfRowsEffected == 0)
                    {
                        MessageBox.Show("Selected Room is in use at specified time and day");
                    }
                    else
                    {
                        MessageBox.Show("Timeslot insert");
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("feilds can't be empty");
            }



        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lecturercomboBox_DropDownOpened(object sender, EventArgs e)
        {
            if(moduleCombobox.SelectedItem != null)
            {
                bindComboBox2(lecturercomboBox);
                if (lecturercomboBox.Items.Count == 0)
                {
                    MessageBox.Show("No lecturers that can teach the selected module");
                }
            }
            else
            {
                MessageBox.Show("Choose Module First");
            }

            
        }

       

        private void moduleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lecturercomboBox.SelectedItem != null)
            {
                lecturercomboBox.SelectedIndex = -1;
            }
        }

        private void moduleCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (moduleCombobox.Items.Count == 0)
            {
                MessageBox.Show("No modules studied by selected course");
            }
        }

        private void roomCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (roomCombobox.Items.Count == 0)
            {
                MessageBox.Show("No Rooms that are big enough");
            }
        }
    }
}
