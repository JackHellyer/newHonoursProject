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
    /// Interaction logic for viewLecturers.xaml
    /// </summary>
    public partial class viewLecturers : Window
    {
        public viewLecturers()
        {
            InitializeComponent();
        }
        private timetableCreationEntities3 dbcontext;
        private System.Windows.Data.CollectionViewSource lecturersViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dbcontext = new timetableCreationEntities3();
            this.lecturersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("lecturersViewSource")));

            var query = from Lecturer in this.dbcontext.Lecturers
                        orderby Lecturer.lecturerName
                        select Lecturer;
            this.lecturersViewSource.Source = query.ToList();
        }
    }
}
