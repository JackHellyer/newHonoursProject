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
        // takes in day and string from the datagrid, course id and name from the course combobox on the main window
        public insertTimetable(string day, string time, string courseId, string courseName)
        {
            InitializeComponent();

            dayTextBox.Text = day;
            timeTextBox.Text = time;
            cId = courseId;
            courseNameTextBox.Text = courseName;
            bindComboBox(moduleCombobox);
            bindComboBox1(roomCombobox);
            

        }

        // bind combobox to module table data
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

        // bind combo box to room table data
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
                
                DataSet ds = new DataSet();
                sda.Fill(ds, "dbo.Room");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["roomCode"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["roomId"].ToString();

            conn.Close();

        }

        // bind combo box to lecturer table data
        public void bindComboBox2(ComboBox comboBoxName)
        {
            if(moduleCombobox.SelectedItem != null)
            {
                int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                
                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select lm.lecturerId, lm.moduleId, l.lecturerName from dbo.Lecturer l, dbo.Lecturer_Module lm WHERE lm.lecturerId = l.lecturerId AND lm.moduleId = @moduleId ORDER BY l.lecturerName;", conn);
                sda.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@moduleId",
                    Value = moduleId

                });
                
                DataSet ds = new DataSet();
                sda.Fill(ds, "dbo.Lecturer");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["lecturerName"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["lecturerId"].ToString();

                conn.Close();
            }


        }

        // save button event handler 
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // if the user has selected a module room and lecturer
            if (moduleCombobox.SelectedItem != null && roomCombobox.SelectedItem != null && lecturercomboBox.SelectedItem != null)
            {
                int courseId = int.Parse(cId);
                int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                int roomId = int.Parse(roomCombobox.SelectedValue.ToString());
                int lecturerId = int.Parse(lecturercomboBox.SelectedValue.ToString());

          


                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();

                // check if the room is being used by by the timetable at selected daya and time
                SqlCommand checkRoom = new SqlCommand("SELECT COUNT(*) FROM dbo.timetable WHERE roomId= @roomId AND day = @day AND time = @time;", conn);
                checkRoom.Parameters.AddWithValue("@roomId", roomId);
                checkRoom.Parameters.AddWithValue("@day", dayTextBox.Text);
                checkRoom.Parameters.AddWithValue("@time", timeTextBox.Text);

                int roomBeingUsed = (int)checkRoom.ExecuteScalar();

                // check if the lecturer is busy at the selected time and day
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
                    // insert timeslot to the timetable table
                    SqlCommand command = new SqlCommand("INSERT dbo.Timetable(courseId, moduleId, lecturerId, roomId, day, time) SELECT @courseId, @moduleId, @lecturerId, @roomId, @day, @time WHERE NOT EXISTS(SELECT courseId, moduleId, lecturerId, roomId, day, time FROM dbo.Timetable WHERE(roomId = @roomId AND day = @day AND time = @time));", conn);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@day", dayTextBox.Text);
                    command.Parameters.AddWithValue("@time", timeTextBox.Text);
                    command.Parameters.AddWithValue("@lecturerId", lecturerId);
                    int numOfRowsEffected = command.ExecuteNonQuery();

                    
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

        // event handler for the cancel button 
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // event handler for the lecturer combo box opening
        private void lecturercomboBox_DropDownOpened(object sender, EventArgs e)
        {
            // if module combo box is not null, bind the lecturer combobox to the table data
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

       
        // event handler for a seection changed module box
        private void moduleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lecturercomboBox.SelectedItem != null)
            {
                lecturercomboBox.SelectedIndex = -1;
            }
        }

        //event handler for the module combobox being opened
        private void moduleCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (moduleCombobox.Items.Count == 0)
            {
                MessageBox.Show("No modules studied by selected course");
            }
        }

        //event handler for the module combobox being opened
        private void roomCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (roomCombobox.Items.Count == 0)
            {
                MessageBox.Show("No Rooms that are big enough");
            }
        }
    }
}
