﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SensorSystem
{    
    #region Menükezelése:

    public delegate void ButtonPressedEventHandler(object sender, EventArgs e);

    public class SelectedMenu
    {
        public event ButtonPressedEventHandler ButtonPressed;
        private List<Sensor> meresek = new List<Sensor>();
        public string menuDisplayName { get; set; }
        public string menuDescription { get; set; }
        public string menuText { get; set; }
        public int menuIndex { get; set; }

        public SelectedMenu(string menudisplayname, string menudescription,string menutext,int menuindex) // Konstruktor kódja
        {
            this.menuDisplayName = menudisplayname;
            this.menuDescription = menudescription;
            this.menuText = menutext;            
            this.menuIndex = menuindex;
            ButtonPressed += this.OnButtonPressed;

        }

        public void selectedMenuRunning(List<Sensor> lista)
        {
            ButtonPressed += this.OnButtonPressed;
            meresek = lista;
          
            SelectedMenuKiir();
           

            bool running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    ButtonPressed.Invoke(key, EventArgs.Empty);
                }
                Thread.Sleep(100);
            }
        }

        public void SelectedMenuKiir()
        {                       
            Console.WriteLine($"Kiválasztott menü: {this.menuDisplayName}");            
            switch (menuIndex)
            {
                case 0:
                    meresek.AddRange(GenerateEngineSensorData());
                    break;
                case 1:
                    meresek.AddRange(GenerateBrakeSensorData());
                    break;
                case 2:
                    meresek.AddRange(GenerateTyreSensorData());
                    break;
                case 3:
                    saveToJSON(meresek);
                    break;
                case 4:
                    saveToXMl(meresek);
                    break;
                case 5:
                    lekerdezesekLINQ(meresek);
                    break;
                case 6:
                    mentesAdatbazisba(meresek);
                    break;
                default:
                    break;
            }
        }

        private List<Sensor> GenerateEngineSensorData() 
        {
            List<Sensor> sensors = new List<Sensor>
            {
                    new Engine("E1", "RPM_mérő", Engine.MeasureRPM(), "0-12000", "RPM", "Engine Block"),
                    new Engine("E2", "Hőmérséklet_mérő", Engine.MeasureTemperature(), "60-110", "°C", "Engine Block"),
                    new Engine("E3", "Olajnyomás_mérő", Engine.MeasureOilPressure(), "50-200", "Bar", "Engine Block")
            };
            foreach (var sensor in sensors) 
            { 
                if (sensor is Engine engine) 
                {
                    switch (engine.szenzorTípus)
                    {
                        case "RPM_mérő":
                            Console.WriteLine($"RPM: {engine.szenzorErtek} {engine.mertekEgyseg}"); 
                            break;
                        case "Hőmérséklet_mérő":
                            Console.WriteLine($"Temperature: {engine.szenzorErtek} {engine.mertekEgyseg}");
                            break;
                        case "Olajnyomás_mérő":
                            Console.WriteLine($"Oil Pressure: {engine.szenzorErtek} {engine.mertekEgyseg}");
                            break;
                        default:
                            Console.WriteLine("Nincs ilyen szenzor Tipus");
                            break;
                    }                                   
                } 
            } 
            return sensors;
        }
        
        private List<Sensor> GenerateBrakeSensorData() 
        { 
            List<Sensor> fek = new List<Sensor> 
            { 
                new Brake("B1", "Féknyomás_mérő", Brake.MeasureBrakePressure(), "50-200", "Bar", "Brake System"), 
                new Brake("B2", "Hőmérséklet_mérő", Brake.MeasureBrakeTemperature(), "100-500", "°C", "Brake System") 
            }; 
            foreach (var sensor in fek) 
            { 
                if (sensor is Brake brake) 
                {
                    switch (brake.szenzorTípus)
                    {
                        case "Féknyomás_mérő":
                            Console.WriteLine($"Brake Pressure: {brake.szenzorErtek} {brake.mertekEgyseg}");
                            break;
                        case "Hőmérséklet_mérő":
                            Console.WriteLine($"Brake Temperature: {brake.szenzorErtek} {brake.mertekEgyseg}");
                            break;                        
                        default:
                            Console.WriteLine("Nincs ilyen szenzor Tipus");
                            break;
                    }
                } 
            }
            return fek;
        }
        
        private List<Sensor> GenerateTyreSensorData()
        {
            List<Sensor> kerekek = new List<Sensor>
            {
                new Tyre("T1", "Hőmérséklet_mérő", Tyre.MeasureTemperature(), "50-120", "°C", "Front Left"),
                new Tyre("T2", "Keréknyomás_mérő", Tyre.MeasurePressure(), "20-35", "PSI", "Front Right"),
                new Tyre("T3", "Kopás_érzékelő", Tyre.MeasureWear(), "0-100", "%", "Rear Left")
            };
            foreach (var sensor in kerekek)
            {
                if (sensor is Tyre tyre)
                {
                    switch (tyre.szenzorTípus)
                    {
                        case "Hőmérséklet_mérő":
                            Console.WriteLine($"Tyre Temperature: {tyre.szenzorErtek} {tyre.mertekEgyseg}");
                            break;
                        case "Keréknyomás_mérő":
                            Console.WriteLine($"Tyre Pressure: {tyre.szenzorErtek} {tyre.mertekEgyseg}");
                            break;
                        case "Kopás_érzékelő":
                            Console.WriteLine($"Tyre Wear: {tyre.szenzorErtek} {tyre.mertekEgyseg}");
                            break;
                        default:
                            Console.WriteLine("Nincs ilyen szenzor Tipus");
                            break;
                    }
                }
            }
            return kerekek;
        }
        
        public void saveToXMl(List<Sensor> lista)
        {
            try
            {
                XmlTextWriter writer;
                string filePath = "meresek.xml";

                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);

                    XmlNode root = doc.DocumentElement;
                    if (root == null)
                    {
                        throw new InvalidOperationException("Az XML fájl gyökéreleme hiányzik.");
                    }

                    foreach (var item in lista)
                    {
                        XmlElement szenzorElem = doc.CreateElement(item.szenzorTípus);
                        szenzorElem.SetAttribute("azon", item.szenzorAzon.ToString());

                        XmlElement ertekElem = doc.CreateElement("ertek");
                        ertekElem.InnerText = item.szenzorErtek.ToString();
                        szenzorElem.AppendChild(ertekElem);

                        XmlElement tartomanyElem = doc.CreateElement("tartomany");
                        tartomanyElem.InnerText = item.szenzorErtekTartomany.ToString();
                        szenzorElem.AppendChild(tartomanyElem);

                        XmlElement mertekElem = doc.CreateElement("mertekegyseg");
                        mertekElem.InnerText = item.mertekEgyseg;
                        szenzorElem.AppendChild(mertekElem);

                        XmlElement helyElem = doc.CreateElement("hely");
                        helyElem.InnerText = item.szenzorHely;
                        szenzorElem.AppendChild(helyElem);

                        root.AppendChild(szenzorElem);
                    }

                    doc.Save(filePath);
                }
                else
                {
                    // Ha a fájl nem létezik, új fájlt hozunk létre
                    writer = new XmlTextWriter(filePath, Encoding.UTF8)
                    {
                        Formatting = System.Xml.Formatting.Indented
                    };
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("Szenzorok");

                    foreach (var item in lista)
                    {
                        writer.WriteStartElement($"{item.szenzorTípus}");
                        writer.WriteAttributeString("azon", $"{item.szenzorAzon}");
                        writer.WriteElementString("ertek", $"{item.szenzorErtek}");
                        writer.WriteElementString("tartomany", $"{item.szenzorErtekTartomany}");
                        writer.WriteElementString("mertekegyseg", $"{item.mertekEgyseg}");
                        writer.WriteElementString("hely", $"{item.szenzorHely}");
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt az XML fájl írása során: {ex.Message}");
            }
        }

        public void saveToJSON(List<Sensor> lista)
        {
            try
            {
                StreamWriter sw = new StreamWriter("meresek.json");
                string json = JsonConvert.SerializeObject(lista, Newtonsoft.Json.Formatting.Indented);
                sw.WriteLine(json);
                sw.Flush();
                sw.Close();
                Console.WriteLine("Sikeresen kiíratás történt!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt az Json fájl írása során: {ex.Message}");
            }
        }

        public void lekerdezesekLINQ(List<Sensor> lista)
        {
            try
            {
                Console.WriteLine("Az összes méréseket kiírja.");
                var osszes = lista.Select(x => x).ToList();
                foreach (var i in osszes)
                {
                    Console.Write(i.ToString() + "\n");
                }
                Console.WriteLine();

                Console.WriteLine("Azok a szenzorokat listázza ki amik hőmérsékletett mérnek.");
                var temperatureSensors = from szenzor in lista
                             where szenzor.mertekEgyseg == "°C"
                             select szenzor;
                foreach (var i in temperatureSensors)
                {
                    Console.Write(i.ToString() + "\n");
                }
                Console.WriteLine();

                Console.WriteLine("Azok a szenzorokat listázza ki ahol a gumikopás nagyobb, mint 60%");
                var gumikopas = from szenzor in lista
                                where szenzor.mertekEgyseg == "%" 
                                && Convert.ToInt32(szenzor.szenzorErtek) > 60
                                select szenzor;
                foreach (var i in gumikopas)
                {
                    Console.Write(i.ToString() + "\n");
                }
                Console.WriteLine();

                Console.WriteLine("Kijelzi a fék átlaghőmérsékletét mérések szerinti.");
                var átlag =     
                    (from szenzor in lista
                    where szenzor.szenzorTípus == "Hőmérséklet_mérő"
                    && szenzor.szenzorHely == "Brake System"
                    select Convert.ToDouble(szenzor.szenzorErtek)).Average();
                
                Console.WriteLine($"{átlag:F00} °C az átlag hőmérséklet");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a lekérdezések során: {ex.Message}");
            }
        }

        public void mentesAdatbazisba(List<Sensor> lista)
        {
            List<Sensor> adatb = new List<Sensor>();
            try
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\vorak\source\repos\MigelHJ\F1Sensors\F1Sensors\Meresek.mdf;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "select * from Szenzorok";
                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader["szenzorAzon"].ToString().Contains('E'))
                            {
                                adatb.Add(new Engine(reader["szenzorAzon"].ToString(), reader["szenzorTípus"].ToString(), Convert.ToInt32(reader["szenzorErtek"]), reader["szenzorErtekTartomany"].ToString(), reader["mertekEgyseg"].ToString(), reader["szenzorHely"].ToString()));
                            }
                            else if (reader["szenzorAzon"].ToString().Contains('B'))
                            {
                                adatb.Add(new Brake(reader["szenzorAzon"].ToString(), reader["szenzorTípus"].ToString(), Convert.ToInt32(reader["szenzorErtek"]), reader["szenzorErtekTartomany"].ToString(), reader["mertekEgyseg"].ToString(), reader["szenzorHely"].ToString()));
                            }
                            else if (reader["szenzorAzon"].ToString().Contains('T'))
                            {
                                adatb.Add(new Tyre(reader["szenzorAzon"].ToString(), reader["szenzorTípus"].ToString(), Convert.ToInt32(reader["szenzorErtek"]), reader["szenzorErtekTartomany"].ToString(), reader["mertekEgyseg"].ToString(), reader["szenzorHely"].ToString()));
                            }
                        }
                    }
                }

                List<Sensor> kilista = new List<Sensor>();
                foreach (var x in lista)
                {
                    bool bennevan = false;
                    foreach (var y in adatb)
                    {
                        if (x.ToString() == y.ToString())
                        {
                            bennevan= true;
                            break;
                        }
                    }
                    if (!bennevan)
                    {
                        kilista.Add(x);
                    }
                }

                string query2 = "INSERT INTO Szenzorok (szenzorAzon, szenzorTípus, szenzorErtek, szenzorErtekTartomany, mertekEgyseg, szenzorHely) VALUES (@szenzorAzon, @szenzorTípus, @szenzorErtek, @szenzorErtekTartomany, @mertekEgyseg, @szenzorHely)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var i in kilista)
                    {
                        using (SqlCommand command2 = new SqlCommand(query2, connection))
                        {
                            command2.Parameters.AddWithValue("@szenzorAzon", i.szenzorAzon);
                            command2.Parameters.AddWithValue("@szenzorTípus", i.szenzorTípus);
                            command2.Parameters.AddWithValue("@szenzorErtek", i.szenzorErtek);
                            command2.Parameters.AddWithValue("@szenzorErtekTartomany", i.szenzorErtekTartomany);
                            command2.Parameters.AddWithValue("@mertekEgyseg", i.mertekEgyseg);
                            command2.Parameters.AddWithValue("@szenzorHely", i.szenzorHely);
                            command2.ExecuteNonQuery();
                        }
                    }
                }
                Console.WriteLine("Adatbázisba való kiírás megtörtént! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a lekérdezések során: {ex.Message}");
            }
        }

        public virtual void OnButtonPressed(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ConsoleKey.Enter:      //menü választás
                    Console.Clear();
                    selectedMenuRunning(meresek);
                    break;
                case ConsoleKey.Escape:     //program leállítása
                    Console.Clear();
                    MainMenu menu = new MainMenu(meresek);
                    ButtonPressed -= this.OnButtonPressed;
                    menu.MainMenuRunning();
                    break;            
            }
            
        }
    }

    public class MainMenu
    {
        public List<Sensor> meresek = new List<Sensor>();
        public event ButtonPressedEventHandler ButtonPressed; 
        public SelectedMenu[] selectedMenus;

        private int currentSelectedMenuIndex;
        private bool inSelectedMenu;


        public MainMenu(List<Sensor> lista)
        {
            selectedMenus = new SelectedMenu[7];
            selectedMenus[0] = new SelectedMenu("Motor Szenzor Értékek", "Itt tudod a legenerált a motor szenzorok értékét megnézni!", "", 0);
            selectedMenus[1] = new SelectedMenu("Fék Szenzor Értékek", "Itt tudod a legenerált a fék szenzorok értékét megnézni!", "", 1);
            selectedMenus[2] = new SelectedMenu("Kerék Szenzor Értékek", "Itt tudod a legenerált a kerék szenzorok értékét megnézni!", "", 2);
            selectedMenus[3] = new SelectedMenu("JSON fájlba kiíratás", "Itt tudod kiíratni az eddigi mérés adatait .json fájlba!", "", 3);
            selectedMenus[4] = new SelectedMenu("Adatbázisba helyezés", "Itt tudod a legenerált méréseket adatbázisba helyezni!(XML)", "", 4);
            selectedMenus[5] = new SelectedMenu("LINQ lekérdezések", "Itt tudod megnézni az előre megírt LINQ-s lekérdezéseket!", "", 5);
            selectedMenus[6] = new SelectedMenu("Adatbázisba mentés", "Itt tudod elmenteni adatbázisba az eddigi méréseket!", "", 6);
            currentSelectedMenuIndex = 0;
            inSelectedMenu = false;
            if (lista.Count()>0)
            {
                meresek.AddRange(lista);
            }
            else
            {
                meresek = lista;
            }
        }

        public void MainMenuRunning()
        {
            ButtonPressed += this.OnButtonPressed;

            if (!inSelectedMenu)
            {
                DisplayMenu();
            }

            bool running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    ButtonPressed.Invoke(key, EventArgs.Empty);
                }
                Thread.Sleep(100);
            }
        }
        
        private void DisplayMenu()
        {
            Console.Clear();
            foreach (var menu in selectedMenus)
            {
                if (menu.menuIndex == currentSelectedMenuIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(menu.menuDisplayName + "\t <----");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\t{menu.menuDescription}");

                }
                else
                {
                    Console.WriteLine(menu.menuDisplayName);
                }
            }
            Console.WriteLine($"\nA lista elemeinek a száma: {meresek.Count()}");
        }

        private void FellepesMenu()
        {
            if (currentSelectedMenuIndex == 0)
            {
                currentSelectedMenuIndex = selectedMenus.Length - 1;
            }
            else currentSelectedMenuIndex--;

            Console.Clear();
            MainMenuRunning();
        }

        private void LelepesMenu()
        {
            if (currentSelectedMenuIndex == selectedMenus.Length-1)
            {
                currentSelectedMenuIndex = 0;
            }
            else currentSelectedMenuIndex++;

            Console.Clear();
            MainMenuRunning();
        }
       
        private void HibasGomb()
        {
            Console.Clear();
            Console.WriteLine("Hibás gombot nyomot, a rendszer nem érzékelte!");
            Thread.Sleep(1000);
        }
        
        public void OnButtonPressed(object sender, EventArgs e)
        {
           switch (sender)
           {
                case ConsoleKey.Enter:      //menü választás
                    Console.Clear();
                    ButtonPressed -= this.OnButtonPressed;
                    selectedMenus[currentSelectedMenuIndex].selectedMenuRunning(meresek);
                    break;
                case ConsoleKey.Escape:     //program leállítása
                    Console.Clear();
                    Console.WriteLine("Program befejezve.");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;
                case ConsoleKey.UpArrow:    //fellépés egy menüvel
                    FellepesMenu();
                    break;
                case ConsoleKey.W:
                    FellepesMenu();
                    break;
                case ConsoleKey.DownArrow:  //lelépés egy menüvel
                    LelepesMenu();
                    break;
                case ConsoleKey.S:
                    LelepesMenu();
                    break;
                default:                    //Hibás gombnyomás
                    HibasGomb();
                    MainMenuRunning();
                    break;
           }
                       
        }
    }

    #endregion

    #region Szenzorok:
    
    public abstract class Sensor
    {
        public string szenzorAzon { get; set; }
        public string szenzorTípus { get; set; } //hőmérséklet, fordulat, nyomás....
        public int szenzorErtek { get; set; } //
        public string szenzorErtekTartomany { get; set; } //
        public string mertekEgyseg { get; set; } // C°, km/h, rpm...
        public string szenzorHely { get; set; }    
        
        public Sensor(string szA, string szTip, int szErt, string szErtTar, string mE, string szH)
        {
            szenzorAzon = szA;
            szenzorTípus = szTip;
            szenzorErtek = szErt;
            szenzorErtekTartomany = szErtTar;
            mertekEgyseg = mE;
            szenzorHely = szH;
        }

        public override string ToString()
        {
            return $"{szenzorAzon}\t{szenzorTípus}\t{szenzorErtek} {mertekEgyseg}\t{szenzorErtekTartomany}\t{szenzorHely}";
        }
    }

    public class Engine : Sensor    // RPM, Hűtőfolyadék hőmérséklet érzékelő, Olajnyomás érzékelő
    {
        public Engine(string szA,string szTip,int szErt,string szErtTar,string mE,string szH):base(szA, szTip, szErt, szErtTar, mE, szH)
        {
            szenzorAzon = szA;
            szenzorTípus = szTip;
            szenzorErtek = szErt;
            szenzorErtekTartomany = szErtTar;
            mertekEgyseg = mE;
            szenzorHely = szH;
        }
    
        public static int MeasureRPM()
        {            
            Random rng = new Random();
            return 9500 + rng.Next(50, 2115);
        }

        public static int MeasureTemperature()
        {
            Random rng = new Random();
            return rng.Next(60, 110);
        }

        public static int MeasureOilPressure()
        {
            Random rng = new Random();
            return rng.Next(50, 200); // Barban a nyomás
        }

    }

    public class Brake : Sensor // Féknyomás, fékhőmérséklet
    {
        public Brake(string szA, string szTip, int szErt, string szErtTar, string mE, string szH) : base(szA, szTip, szErt, szErtTar, mE, szH)
        {
            szenzorAzon = szA;
            szenzorTípus = szTip;
            szenzorErtek = szErt;
            szenzorErtekTartomany = szErtTar;
            mertekEgyseg = mE;
            szenzorHely = szH;
        }

        public static int MeasureBrakePressure()
        {
            Random rng = new Random();
            return rng.Next(50, 200); // Barban a nyomás
        }

        public static int MeasureBrakeTemperature()
        {
            Random rng = new Random();
            return rng.Next(100, 1000); // °C
        }
    }

    public class Tyre : Sensor // Gumihőmérséklet, Keréknyomás-érzékelő, Kopásérzékelő
    {
        public Tyre(string szA, string szTip, int szErt, string szErtTar, string mE, string szH) : base(szA, szTip, szErt, szErtTar, mE, szH)
        {
                szenzorAzon = szA;
                szenzorTípus = szTip;
                szenzorErtek = szErt;
                szenzorErtekTartomany = szErtTar;
                mertekEgyseg = mE;
                szenzorHely = szH;
        }

        public static int MeasureTemperature()
        {
            Random rng = new Random();
            return rng.Next(50, 120); //  °C                
        }

        public static int MeasurePressure()
        {            
            Random rng = new Random();
            return rng.Next(20, 35); // PSI-ben             
        }

        public static int MeasureWear()
        {
            Random rng = new Random();
            return rng.Next(0, 100); // Kopás %                
        }
    }
    #endregion
}
