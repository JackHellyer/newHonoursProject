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
    /// Interaction logic for viewModules.xaml
    /// </summary>
    /// did'n't have time to add comments
    /// this pages comments are the same as view course
    public partial class viewModules : Window
    {
        public viewModules()
        {
            InitializeComponent();
        }

        private timetableCreationEntities3 dbcontext;
        private System.Windows.Data.CollectionViewSource moduleViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dbcontext = new timetableCreationEntities3();
            this.moduleViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("moduleViewSource")));
            refreshListView();
            
        }

        private void refreshListView()
        {
            var query = from Module in this.dbcontext.Modules
                        orderby Module.moduleCode
                        select Module;
            this.moduleViewSource.Source = query.ToList();
        }
        private void OnDelete(object sender, RoutedEventArgs e)
        {

            object selected = this.moduleListView.SelectedItem;

            if (selected == null)
            {
                MessageBox.Show("No Module Selected");
                return;
            }
            else
            {
                try
                {
                    this.dbcontext.Modules.Remove((Module)selected);
                    this.dbcontext.SaveChanges();
                    refreshListView();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    MessageBox.Show("Can't delete module as it's being used by a timetable");
                }

            }

        }


    }
}
