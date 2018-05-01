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
    /// Interaction logic for newTimetableDialog.xaml
    /// </summary>
    public partial class newTimetableDialog : Window
    {
        public newTimetableDialog()
        {
            InitializeComponent();
        }

        public void createTimetable_Click(object sender, RoutedEventArgs e)
        {
            string timetableName = timetableNameTextBox.Text;
            
            bool exists = System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Timetable App/" + timetableNameTextBox.Text);
            if(!exists)
            {
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Timetable App/" + timetableNameTextBox.Text);
                Window1 win1 = new Window1(timetableName);
                win1.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Timetable Name already Exists, chose another");
                this.Close();

            }
            
           
           

        }
    }
}
