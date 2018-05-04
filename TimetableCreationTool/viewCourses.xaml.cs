using System;
using System.Collections.Generic;
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
    /// Interaction logic for viewCourses.xaml
    /// </summary>
    public partial class viewCourses : Window
    {
        public viewCourses()
        {
            InitializeComponent();
        }

        private timetableCreationEntities3 dbcontext;
        private System.Windows.Data.CollectionViewSource coursesViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dbcontext = new timetableCreationEntities3();
            // course lsit view source
            this.coursesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("coursesViewSource")));

            // refresh listview
            refreshListView();
            

            
        }
        // method to refresh listview
        private void refreshListView()
        {
            //linq query to return all of the courses
            var query = from Course in this.dbcontext.Courses
                        orderby Course.courseCode
                        select Course;
            this.coursesViewSource.Source = query.ToList();
        }
        // on delete get 
        private void OnDelete(object sender, RoutedEventArgs e)
        {
            // list view selected item
            object selected = this.coursesListView.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("No Course Selected");
                return;
            }
            else
            {
                try
                {
                    // cast selected item object to Course and then remove
                    this.dbcontext.Courses.Remove((Course)selected);
                    this.dbcontext.SaveChanges();
                    refreshListView();
                }
                catch (Exception ex)
                {
                    //validation this would ideally be improved
                    //MessageBox.Show(ex.ToString());
                    MessageBox.Show("Can't delete course as it's being used by a timetable");
                }

            }

        }
    }
}
