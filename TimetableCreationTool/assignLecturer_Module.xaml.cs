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
    /// Interaction logic for assignLecturer_Module.xaml
    /// </summary>
    public partial class assignLecturer_Module : Window
    {
        
        private timetableCreationEntities3 dbcontext;
        private System.Windows.Data.CollectionViewSource moduleViewSource;
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public assignLecturer_Module()
        {
            InitializeComponent();
            bindComboBox(lecturerComboBox);
            bindComboBox1(modulecomboBox);
        }

        public void bindComboBox(ComboBox comboBoxName)
        {
            string query = "select lecturerId, CONCAT(lecturerName, ' (Dept) ', lecturerDept) AS lectInfo from dbo.Lecturer ORDER BY lecturerName";
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            sda.Fill(ds, "dbo.Lecturer");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["lectInfo"].ToString();
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["lecturerId"].ToString();
        }

        public void bindComboBox1(ComboBox comboBoxName)
        {
            string query = "select moduleId, moduleCode, moduleName from dbo.Module ORDER BY moduleName";
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            sda.Fill(ds, "dbo.Module");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["moduleName"].ToString();
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["moduleId"].ToString();
        }

        private void lecturerComboBox_DropDownClosed(object sender, EventArgs e)
        {
            onRefresh();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (modulecomboBox.SelectedItem != null && lecturerComboBox.SelectedItem != null)
            {

                int lecturerId = int.Parse(lecturerComboBox.SelectedValue.ToString());
                int moduleId = int.Parse(modulecomboBox.SelectedValue.ToString());
                //string query = "INSERT INTO Course_Module (courseId, moduleId) VALUES(" + courseId + "," + moduleId +");";
                string query2 = "INSERT dbo.Lecturer_Module (lecturerId, moduleId) SELECT " + lecturerId + "," + moduleId + " WHERE NOT EXISTS( SELECT lecturerId, moduleId FROM dbo.Lecturer_Module WHERE lecturerId = " + lecturerId + " AND moduleId = " + moduleId + ");";




                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();
                SqlCommand command = new SqlCommand(query2, conn);
                command.ExecuteNonQuery();
                conn.Close();

                onRefresh();

            }
            else
            {
                MessageBox.Show("No Lecturer or Module selected.");
            }
        }

        public void onRefresh()
        {
            this.dbcontext = new timetableCreationEntities3();
            this.moduleViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("moduleViewSource")));
            if(lecturerComboBox.SelectedItem != null)
            {
                int lecturerId = int.Parse(lecturerComboBox.SelectedValue.ToString());

                

                var query = from Module in this.dbcontext.Modules
                            where Module.Lecturers.Any(l => l.lecturerId == lecturerId)
                            orderby Module.moduleName
                            select Module;
                this.moduleViewSource.Source = query.ToList();
            }
            
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            object selected = this.moduleListView.SelectedItem;
            // cast listview object as a Module
            Module module = (Module)selected;
            if (selected == null)
            {
                MessageBox.Show("No module selected");
                return;

            }
            int mId = module.moduleId;
            int lecturerId = int.Parse(lecturerComboBox.SelectedValue.ToString());
            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();

            // check if module is currently being used by the course
            SqlCommand checkModulebeingTaught = new SqlCommand("SELECT COUNT(*) FROM dbo.timetable WHERE lecturerId = @lecturerId AND moduleId = @moduleId;", conn);
            checkModulebeingTaught.Parameters.AddWithValue("@lecturerId", lecturerId);
            checkModulebeingTaught.Parameters.AddWithValue("@moduleId", mId);

            int LecturerTeachingModule = (int)checkModulebeingTaught.ExecuteScalar();

            if (LecturerTeachingModule > 0)
            {
                MessageBox.Show("can't delete module as it's being taught by current lecturer");
            }
            else
            {
                // delete selected module from selected course
                string query = "DELETE FROM Lecturer_Module WHERE lecturerId = " + lecturerId + "AND moduleId = " + mId + ";";

                SqlCommand command = new SqlCommand(query, conn);
                command.ExecuteNonQuery();


                onRefresh();

            }

            conn.Close();
        }
    }
}

