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
            //dayTextBox.Text = d;
            //timeTextBox.Text = t;
            cId = courseId;
            courseNameTextBox.Text = cName;
            bindComboBox(moduleCombobox);
            bindComboBox1(roomCombobox);
            

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dayCombobox.Items.Add("Monday");
            dayCombobox.Items.Add("Tuesday");
            dayCombobox.Items.Add("Wednesday");
            dayCombobox.Items.Add("Thursday");
            dayCombobox.Items.Add("Friday");

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
            //string mName;
            //string rCode;
            using (var conn = new SqlConnection(dbConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@courseId", cId);
                cmd.Parameters.AddWithValue("@day", d);
                cmd.Parameters.AddWithValue("@time", t);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mName = reader.GetString(reader.GetOrdinal("moduleName"));
                        rCode = reader.GetString(reader.GetOrdinal("roomCode"));
                        lName = reader.GetString(reader.GetOrdinal("lecturerName"));
                        moduleId = reader.GetInt32(reader.GetOrdinal("moduleId"));
                        tId = reader.GetInt32(reader.GetOrdinal("tId"));

                        bindComboBox2(lecturercomboBox, moduleId);
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

        private void setIndexCombobox(string comboboxData, ComboBox combobox)
        {
            
            int index = -1;
            int count = combobox.Items.Count;
            for (int i = 0; i <= (count - 1); i++)
            {
                combobox.SelectedIndex = i;
                //MessageBox.Show(moduleCombobox.SelectedItem.ToString());
                if (combobox.Text == comboboxData)
                {
                    index = i;
                    //MessageBox.Show(moduleCombobox.Text);

                }

            }
            combobox.SelectedIndex = index;
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


        public void bindComboBox2(ComboBox comboBoxName, int mId)
        {
            /*if (moduleCombobox.SelectedItem != null)
            {*/
                //int moduleId = int.Parse(moduleCombobox.SelectedValue.ToString());
                //MessageBox.Show(moduleId.ToString());
                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select lm.lecturerId, lm.moduleId, l.lecturerName from dbo.Lecturer l, dbo.Lecturer_Module lm WHERE lm.lecturerId = l.lecturerId AND lm.moduleId = @moduleId ORDER BY l.lecturerName;", conn);
                sda.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@moduleId",
                    Value = mId

                });
                //sda.SelectCommand.Parameters.AddWithValue("@day", dayTextBox.Text);
                DataSet ds = new DataSet();
                sda.Fill(ds, "dbo.Lecturer");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["lecturerName"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["lecturerId"].ToString();

                conn.Close();
            //}
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
                //"Insert dbo.Timetable(courseId, moduleId, lecturerId, roomId, day, time) SELECT @courseId, @moduleId, @lecturerId, @roomId, @day, @time WHERE NOT EXISTS(SELECT courseId, moduleId, lecturerId, roomId, day, time FROM dbo.Timetable WHERE(roomId = @roomId AND day = @day AND time = @time));
                SqlCommand command = new SqlCommand("UPDATE dbo.Timetable SET courseId = @courseId, moduleId = @moduleId, roomId = @roomId, lecturerId = @lecturerId, day = @day, time = @time WHERE tId = @tId", conn);
                command.Parameters.AddWithValue("@courseId", courseId);
                command.Parameters.AddWithValue("@moduleId", moduleId);
                command.Parameters.AddWithValue("@roomId", roomId);
                command.Parameters.AddWithValue("@day", dayCombobox.Text);
                command.Parameters.AddWithValue("@time", timeCombobox.Text);
                command.Parameters.AddWithValue("@lecturerId", lecturerId);
                command.Parameters.AddWithValue("@tId", tId);
                int numOfRowsEffected = command.ExecuteNonQuery();
                //MessageBox.Show(tId.ToString());

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

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lecturercomboBox_DropDownOpened(object sender, EventArgs e)
        {
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



        private void moduleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lecturercomboBox.SelectedItem != null)
            {
                lecturercomboBox.SelectedIndex = -1;
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            int courseId = int.Parse(cId);
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlCommand command = new SqlCommand("DELETE FROM dbo.Timetable WHERE tId = @tId", conn);
            
            command.Parameters.AddWithValue("@tId", tId);
            
            int numOfRowsEffected = command.ExecuteNonQuery();

            //MessageBox.Show(numOfRowsEffected.ToString());
            DialogResult = true;
            this.Close();
        }
    }
}
