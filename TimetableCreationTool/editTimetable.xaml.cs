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
    /// Interaction logic for editTimetable.xaml
    /// </summary>
    public partial class editTimetable : Window
    {
        // db connection string
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";
        string cId;
        string d;
        string t;
        string cName;
        int tId;

        public editTimetable(string day, string time, string courseId, string courseName)
        {
            InitializeComponent();

            d = day;
            t = time;
            cName = courseName;
            cId = courseId;
            courseNameTextBox.Text = cName;
            bindComboBox(moduleCombobox);
            bindComboBox1(roomCombobox);
            

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // add day values so user can't insert invalid day
            dayCombobox.Items.Add("Monday");
            dayCombobox.Items.Add("Tuesday");
            dayCombobox.Items.Add("Wednesday");
            dayCombobox.Items.Add("Thursday");
            dayCombobox.Items.Add("Friday");

            // add time values so user can't insert invalid times
            timeCombobox.Items.Add("08:00");
            timeCombobox.Items.Add("09:00");
            timeCombobox.Items.Add("10:00");
            timeCombobox.Items.Add("11:00");
            timeCombobox.Items.Add("12:00");
            timeCombobox.Items.Add("13:00");
            timeCombobox.Items.Add("14:00");
            timeCombobox.Items.Add("15:00");
            timeCombobox.Items.Add("16:00");
            timeCombobox.Items.Add("17:00");


            string query = "SELECT t.tId, m.moduleId, m.moduleName, r.roomCode, l.lecturerName FROM Timetable t, Module m, Room r, Lecturer l WHERE t.moduleId = m.moduleId AND t.roomId = r.roomId AND t.lecturerId = l.lecturerId AND courseId = @courseId AND day = @day AND time = @time;";
            string mName;
            string rCode;
            string lName;
            int moduleId;
            
            var conn = new SqlConnection(dbConnectionString);
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@courseId", cId);
                cmd.Parameters.AddWithValue("@day", d);
                cmd.Parameters.AddWithValue("@time", t);
                conn.Open();

                // read data from the timetable table 
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // assign a string
                        mName = reader.GetString(reader.GetOrdinal("moduleName"));
                        rCode = reader.GetString(reader.GetOrdinal("roomCode"));
                        lName = reader.GetString(reader.GetOrdinal("lecturerName"));
                        moduleId = reader.GetInt32(reader.GetOrdinal("moduleId"));
                        tId = reader.GetInt32(reader.GetOrdinal("tId"));

                        // bind lecturer box
                        bindComboBox2(lecturercomboBox, moduleId);

                        //set the combo box selected item to the time slots information
                        setIndexCombobox(mName, moduleCombobox);
                        setIndexCombobox(rCode, roomCombobox);
                        setIndexCombobox(lName, lecturercomboBox);
                        setIndexCombobox(d, dayCombobox);
                        setIndexCombobox(t, timeCombobox);



                        conn.Close();
                    }


                }
            }
        }

            
        // method to set the index of the combo box
        // takes in a string and comboxbox
        private void setIndexCombobox(string comboboxData, ComboBox combobox)
        {
            
            int index = -1;
            int count = combobox.Items.Count;
            for (int i = 0; i <= (count - 1); i++)
            {
                combobox.SelectedIndex = i;
                
                //if the combobox text is equal to the string param
                if (combobox.Text == comboboxData)
                {
                    index = i;
                    

                }

            }
            
            combobox.SelectedIndex = index;
        }

        // bind combobox to modules
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

        // bind combo box to applicable rooms
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

        // takes in a variable that will hold the module id
        // bind combobox to lecturer table data
        public void bindComboBox2(ComboBox comboBoxName, int mId)
        {
            
                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select lm.lecturerId, lm.moduleId, l.lecturerName from dbo.Lecturer l, dbo.Lecturer_Module lm WHERE lm.lecturerId = l.lecturerId AND lm.moduleId = @moduleId ORDER BY l.lecturerName;", conn);
                sda.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@moduleId",
                    Value = mId

                });
            
                DataSet ds = new DataSet();
                sda.Fill(ds, "dbo.Lecturer");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["lecturerName"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["lecturerId"].ToString();

                conn.Close();
            
        }

        // save button event handler
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // if all of the combo boxes are not equal to null, day and time combo box should be in this statement as well
            if (moduleCombobox.SelectedItem != null && roomCombobox.SelectedItem != null && lecturercomboBox.SelectedItem != null)
            {
                // gets ints from the combobox
                int courseId = int.Parse(cId);
                int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                int roomId = int.Parse(roomCombobox.SelectedValue.ToString());
                int lecturerId = int.Parse(lecturercomboBox.SelectedValue.ToString());


                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();

                // check if room is being used at selected time and day
                SqlCommand checkRoom = new SqlCommand("SELECT COUNT(*) FROM dbo.timetable WHERE roomId= @roomId AND day = @day AND time = @time;", conn);
                checkRoom.Parameters.AddWithValue("@roomId", roomId);
                checkRoom.Parameters.AddWithValue("@day", dayCombobox.Text);
                checkRoom.Parameters.AddWithValue("@time", timeCombobox.Text);

                int roomBeingUsed = (int)checkRoom.ExecuteScalar();

                // check if lecturer is busy at selected time and day
                SqlCommand checkLecturer = new SqlCommand("SELECT COUNT(*) FROM dbo.Timetable WHERE lecturerId = @lecturerId and day = @day AND time = @time;", conn);
                checkLecturer.Parameters.AddWithValue("@lecturerId", lecturerId);
                checkLecturer.Parameters.AddWithValue("@day", dayCombobox.Text);
                checkLecturer.Parameters.AddWithValue("@time", timeCombobox.Text);

                int lecturerBusy = (int)checkLecturer.ExecuteScalar();

                // if room is busy
                if (roomBeingUsed > 0)
                {
                    MessageBox.Show("Room being used at selected time");
                }
                // if lecturer is busy
                else if (lecturerBusy > 0)
                {
                    MessageBox.Show("Lecturer busy");
                }
                else
                {
                    // update timeslot with new data
                    SqlCommand command = new SqlCommand("UPDATE dbo.Timetable SET courseId = @courseId, moduleId = @moduleId, roomId = @roomId, lecturerId = @lecturerId, day = @day, time = @time WHERE tId = @tId", conn);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.Parameters.AddWithValue("@roomId", roomId);
                    command.Parameters.AddWithValue("@day", dayCombobox.Text);
                    command.Parameters.AddWithValue("@time", timeCombobox.Text);
                    command.Parameters.AddWithValue("@lecturerId", lecturerId);
                    command.Parameters.AddWithValue("@tId", tId);
                    int numOfRowsEffected = command.ExecuteNonQuery();
                    
                    conn.Close();
                    // if the no rows in the table are affected
                    if (numOfRowsEffected == 0)
                    {
                        MessageBox.Show("Selected Room is in use at specified time and day");
                    }
                    else
                    {
                        // inform the user of succesful update
                        MessageBox.Show("Timeslot insert");
                        this.Close();
                    }
                }
               
            }

        }

        // event handler for cancel button, could have just set iscancel to true in xaml
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // event handler for when the lecturer combobox is opened
        private void lecturercomboBox_DropDownOpened(object sender, EventArgs e)
        {
            // if the module combo box is not null bind the lecturer combo box using the selected modules id
            if (moduleCombobox.SelectedItem != null)
            {
                int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                bindComboBox2(lecturercomboBox, moduleId);
            }
            else
            {
                MessageBox.Show("Choose Module First");
            }
        }


        // event handler for when the module combobox selection is changed
        private void moduleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // reset lecturer combobox, stops invalid data being entered
            if (lecturercomboBox.SelectedItem != null)
            {
                lecturercomboBox.SelectedIndex = -1;
            }
        }

        // event handler for the delete button
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // deletes the timeslot from the timetable table using the tID
            int courseId = int.Parse(cId);
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlCommand command = new SqlCommand("DELETE FROM dbo.Timetable WHERE tId = @tId", conn);
            
            command.Parameters.AddWithValue("@tId", tId);
            
            int numOfRowsEffected = command.ExecuteNonQuery();

            
            DialogResult = true;
            this.Close();
        }

        // event handler for when the module combobox is opened
        private void moduleCombobox_DropDownOpened(object sender, EventArgs e)
        {
            // if the only available module is the one the timeslot is using notify the user
            if(moduleCombobox.Items.Count == 1)
            {
                MessageBox.Show("No other modules studied by course");
            }
        }

        // same as above module but for rooms
        private void roomCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (roomCombobox.Items.Count == 1)
            {
                MessageBox.Show("No other rooms big enough");
            }
        }

        // same as above module but for lecturers
        private void lecturercomboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            if (lecturercomboBox.Items.Count == 1)
            {
                MessageBox.Show("No other lecturers can teach the module");
            }
        }
    }
}
