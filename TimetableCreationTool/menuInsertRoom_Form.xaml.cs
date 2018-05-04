using System;
using System.Collections.Generic;
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
    /// Interaction logic for menuInsertRoom_Form.xaml
    /// </summary>
    /// // same as menu insert course comments
        // didn't have time to add comments
    public partial class menuInsertRoom_Form : Window
    {
        private string dbConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;  Initial Catalog = timetableCreation; Integrated Security = True; Connect Timeout = 30";

        public menuInsertRoom_Form()
        {
            InitializeComponent();
            roomIsLabCombobox.Items.Add("true");
            roomIsLabCombobox.Items.Add("false");
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            
            
            string roomCode = roomCodeTextbox.Text;
            string roomCapString = roomCapacityTextbox.Text;
            

            if(roomCode != null && roomCapString != null && roomIsLabCombobox.SelectedItem != null)
            {
                bool isLab = bool.Parse(roomIsLabCombobox.Text);
                int roomCap;
                if(roomCode.Length < 11 && int.TryParse(roomCapString, out roomCap))
                {
                    SqlConnection conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.roomTemp(roomCode, capacity, lab) values(@roomCode, @capacity, @islab);", conn);
                    command.Parameters.AddWithValue("@roomCode", roomCode);
                    command.Parameters.AddWithValue("@capacity", roomCap);
                    command.Parameters.AddWithValue("@islab", isLab);

                    command.ExecuteNonQuery();

                    int number = selectIntoDistinct("INSERT dbo.Room(roomCode, capacity, lab) SELECT roomCode, capacity, lab FROM dbo.roomTemp rt WHERE not exists(SELECT * FROM dbo.Room r WHERE rt.roomCode = r.roomCode); ");

                    if (number == 1)
                    {
                        this.Close();
                        truncateTempAfterInsert("TRUNCATE TABLE dbo.roomTemp;");
                    }
                    else
                    {

                        MessageBox.Show("Room already exists");
                    }


                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Room Capacity must be a number and Room code can't be longer than 10 characters");

                }
            }
            else
            {
                MessageBox.Show("fields can't be empty");
            }
                


           
        }

        public int selectIntoDistinct(string query)
        {
            //string queryString = "INSERT dbo.Room(roomCode,capacity,lab) SELECT roomCode,capacity,lab FROM dbo.roomTemp rt WHERE not exists(SELECT * FROM dbo.Room r WHERE rt.roomCode = r.roomCode);";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(query, dbConnection);
                dbConnection.Open();
                int numberOfChanges = command.ExecuteNonQuery();
                dbConnection.Close();
                return numberOfChanges;
                //return numOfRows;

            }
        }

        public void truncateTempAfterInsert(string query)
        {
            //string queryString = "TRUNCATE TABLE dbo.roomTemp;";
            using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
            {

                SqlCommand command = new SqlCommand(query, dbConnection);
                dbConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dbConnection.Close();


            }
        }



    }
}
