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
    /// Interaction logic for addModulesStudied.xaml
    /// </summary>
    public partial class addModulesStudied : Window
    {
        string cName;
        string cId;
        // sql connection string
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";
        public addModulesStudied(string courseName, string courseId)
        {
            InitializeComponent();
            // takes in course name and id from the combobox on window1
            cName = courseName;
            cId = courseId;
            // set heading
            modulesHeading.Content = "Current Models Studied By " + cName;
            
            // bind combobox to moduleid and module name
            bindComboBox(comboBox);

        }
        private timetableCreationEntities3 dbcontext;
        private System.Windows.Data.CollectionViewSource moduleViewSource;

        // on window load refresh the modules listview
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            onRefresh();
            
        }

        // method to refresh the listview
        public void onRefresh()
        {
            this.dbcontext = new timetableCreationEntities3();
            this.moduleViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("moduleViewSource")));
            int intCId = int.Parse(cId);

            // link query to get the modules for the selected course
            var query = from Module in this.dbcontext.Modules
                        where Module.Courses.Any(c => c.courseId == intCId)
                        orderby Module.moduleName
                        select Module;
            this.moduleViewSource.Source = query.ToList();
        }

        // bind combobox
        public void bindComboBox(ComboBox comboBoxName)
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

        
        // on presseing the add module button
        private void addModules_Click(object sender, RoutedEventArgs e)
        {
            // if the user has selected a module
            if(comboBox.SelectedItem != null)
            {
                int courseId = int.Parse(cId);
                int moduleId = int.Parse(comboBox.SelectedValue.ToString());
                // insert into Course_module table if they don't exist
                string query2 = "INSERT dbo.Course_Module (courseId, moduleId) SELECT " + courseId + "," +  moduleId + " WHERE NOT EXISTS( SELECT courseId, moduleId FROM dbo.Course_Module WHERE courseId = " + courseId + " AND moduleId = " + moduleId + ");";

                SqlConnection conn = new SqlConnection(dbConnectionString);
                conn.Open();
                SqlCommand command = new SqlCommand(query2, conn);
                command.ExecuteNonQuery();
                conn.Close();
                
                //refresh the listview to show the newly added module
                onRefresh();
            }
            else
            {
                MessageBox.Show("No module selected.");
            }
            
        }

        // on pressing delete button
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {

            object selected = this.moduleListView.SelectedItem;
            // cast listview object as a Module
            Module module = (Module)selected;
            if(selected == null)
            {
                MessageBox.Show("No module selected");
                return;
               
            }
            int mId = module.moduleId;

            SqlConnection conn = new SqlConnection(dbConnectionString);
            conn.Open();

            // check if module is currently being used by the course
            SqlCommand checkModulebeingused = new SqlCommand("SELECT COUNT(*) FROM dbo.timetable WHERE courseId = @courseId AND moduleId = @moduleId;", conn);
            checkModulebeingused.Parameters.AddWithValue("@courseId", cId);
            checkModulebeingused.Parameters.AddWithValue("@moduleId", mId);

            int moduleBeingUsed = (int)checkModulebeingused.ExecuteScalar();

            if(moduleBeingUsed > 0)
            {
                MessageBox.Show("can't delete module as it's being used by current course");
            }
            else
            {
                // delete selected module from selected course
                string query = "DELETE FROM Course_Module WHERE courseId = " + cId + "AND moduleId = " + mId + ";";

                SqlCommand command = new SqlCommand(query, conn);
                command.ExecuteNonQuery();
                

                onRefresh();
              
            }

            conn.Close();
        }
        //continue button 
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //close dialog window
            this.Close();
        }
    }
}
