using SensorSystem;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Configuration;

namespace F1Sensors
{  
    class Program
    {     
        static void Main(string[] args)
        {
            List<Sensor> meresek = new List<Sensor>();
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
                "AttachDbFilename=C:\\Users\\vorak\\Source\\Repos\\MigelHJ\\F1Sensors\\F1Sensors\\Meresek.mdf;" +
                "Integrated Security=True";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "select * from meresek";
                SqlCommand command = new SqlCommand(query,connection);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader["szenzorAzon"].ToString().Contains('E'))
                        {
                            meresek.Add(new Engine(reader["szenzorAzon"].ToString(), reader["szenzorTípus"].ToString(), Convert.ToInt32(reader["szenzorErtek"]), reader["szenzorErtekTartomany"].ToString(), reader["mertekEgyseg"].ToString(), reader["szenzorHely"].ToString()));
                        }
                        else if (reader["szenzorAzon"].ToString().Contains('B'))
                        {
                            meresek.Add(new Brake(reader["szenzorAzon"].ToString(), reader["szenzorTípus"].ToString(), Convert.ToInt32(reader["szenzorErtek"]), reader["szenzorErtekTartomany"].ToString(), reader["mertekEgyseg"].ToString(), reader["szenzorHely"].ToString()));
                        }
                        else if (reader["szenzorAzon"].ToString().Contains('T'))
                        {
                            meresek.Add(new Tyre(reader["szenzorAzon"].ToString(), reader["szenzorTípus"].ToString(), Convert.ToInt32(reader["szenzorErtek"]), reader["szenzorErtekTartomany"].ToString(), reader["mertekEgyseg"].ToString(), reader["szenzorHely"].ToString()));
                        }
                    }
                }               
            }

            MainMenu mainMenu = new MainMenu(meresek);
            mainMenu.MainMenuRunning();
        }        
    }
}
