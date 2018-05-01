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

            var query = from Module in this.dbcontext.Modules
                        orderby Module.moduleCode
                        select Module;
            this.moduleViewSource.Source = query.ToList();
        }

        
    }
}
