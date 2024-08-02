using System;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Internal;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using MySql.Data.MySqlClient;
using Mysqlx.Session;
using MySqlX.XDevAPI.Common;


namespace caculator
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        MySqlConnection con = new MySqlConnection(
            "server=localhost;userid=root;password= ;database=caculator;"          //connect to database
            );

        private string sql;
        public Window1()
        {
            InitializeComponent();
            //dataShow.MouseDoubleClick += DataShow_MouseDoubleClick;
        }

        private void show_Click(object sender, RoutedEventArgs e)
        {
            DisplayData();
           
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            

            if (dataGridView1.SelectedItems.Count > 0)
            {
                
                DataRowView selectedRow = dataGridView1.SelectedItems[0] as DataRowView;    //get row

                string expression2 = selectedRow["expression"].ToString();
                try
                {
                     //adding MySQL connection
                     string connectionString = "server=localhost;userid=root;password= ;database=caculator;";
                     using (MySqlConnection con = new MySqlConnection(connectionString))
                     {
                         con.Open();
                         using (MySqlCommand cmd1 = new MySqlCommand("DELETE FROM data_new WHERE expression = @expression", con))
                         {
                               
                             cmd1.Parameters.AddWithValue("@expression", expression2);
                             cmd1.ExecuteNonQuery();                //delete command
                         }
                         con.Close();
                     }

                     System.Windows.MessageBox.Show("Successful delete");      

                        //reload data
                     DisplayData();
                     ClearData();
                       
                    }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + ex.Message);
                }
                   
            }
            else
            {
                 System.Windows.MessageBox.Show("Invalid ID format.");
            }
         
            
        }
        private void DisplayData()     //show data
        {
           
            try
            {
                string connectionString = "server=localhost;userid=root;password=;database=caculator;";
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    //System.Windows.MessageBox.Show("Yes connect!!");
                    sql = "SELECT expression, postoder, preoder, `decimal`, `binary` FROM data_new;";
                    MySqlCommand sc = new MySqlCommand(sql, con);    //sql語句
                    MySqlDataAdapter sda = new MySqlDataAdapter(sc);     //數據適配器
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables.Count > 0)
                    {
                        dataGridView1.ItemsSource = ds.Tables[0].DefaultView;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("No data returned.");
                    }

                    con.Close();
                    con.Dispose();
                }
                    
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error");
            }
            finally
            {
                con.Close();
            }

        }
        private void ClearData()
        {
            dataGridView1.ItemsSource = null;
        }
        
    }
}
