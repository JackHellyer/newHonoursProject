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
    /// Interaction logic for viewRooms.xaml
    /// </summary>
    public partial class viewRooms : Window
    {
        public viewRooms()
        {
            InitializeComponent();
        }
        private timetableCreationEntities3 dbcontext;
        private System.Windows.Data.CollectionViewSource roomsViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dbcontext = new timetableCreationEntities3();
            this.roomsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("roomsViewSource")));

            refreshListView();
        }

        private void refreshListView()
        {
            var query = from Room in this.dbcontext.Rooms
                        orderby Room.roomCode
                        select Room;
            this.roomsViewSource.Source = query.ToList();
        }
        private void OnDelete(object sender, RoutedEventArgs e)
        {
            
            object selected = this.roomsListView.SelectedItem;
            
            if (selected == null)
            {
                MessageBox.Show("No Room Selected");
                return;
            }
            else
            {
                try
                {
                    this.dbcontext.Rooms.Remove((Room)selected);
                    this.dbcontext.SaveChanges();
                    refreshListView();
                }
                catch(Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    MessageBox.Show("Can't delete room as it's being used by a timetable");
                }
                
            }

        }

        

        private void roomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //object selectedRoom = roomsListView.SelectedItem;

            //Room room = (Room)selectedRoom;
        }
    }
}
