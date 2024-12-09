using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace SensorSystem
{    
    #region Menükezelése:

    public delegate void ButtonPressedEventHandler(object sender, EventArgs e);

    public class SelectedMenu
    {
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
        }

        public void SelectedMenuKiir(List<Sensor> lista)
        {
            meresek = lista;
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
            List<Sensor> sensors = new List<Sensor> 
            { 
                new Brake("B1", "Féknyomás_mérő", Brake.MeasureBrakePressure(), "50-200", "Bar", "Brake System"), 
                new Brake("B2", "Hőmérséklet_mérő", Brake.MeasureBrakeTemperature(), "100-500", "°C", "Brake System") 
            }; 
            foreach (var sensor in sensors) 
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
            return sensors;
        }
        
        private List<Sensor> GenerateTyreSensorData()
        {
            List<Sensor> sensors = new List<Sensor>
            {
                new Tyre("T1", "Hőmérséklet_mérő", Tyre.MeasureTemperature(), "50-120", "°C", "Front Left"),
                new Tyre("T2", "Keréknyomás_mérő", Tyre.MeasurePressure(), "20-35", "PSI", "Front Right"),
                new Tyre("T3", "Kopás_érzékelő", Tyre.MeasureWear(), "0-100", "%", "Rear Left")
            };
            foreach (var sensor in sensors)
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
            return sensors;
        }

        public virtual void HandleSelectedMenuInput(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ConsoleKey.Enter:      //menü választás
                    Console.Clear();
                    SelectedMenuKiir(meresek);
                    break;
                case ConsoleKey.Escape:     //program leállítása
                    Console.Clear();
                    MainMenu menu = new MainMenu(meresek);
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

        public MainMenu()
        {            
            selectedMenus = new SelectedMenu[6];
            selectedMenus[0] = new SelectedMenu("Motor Szenzor Értékek","Itt tudod a legenerált a motor szenzorok értékét megnézni!", "",0);
            selectedMenus[1] = new SelectedMenu("Fék Szenzor Értékek", "Itt tudod a legenerált a fék szenzorok értékét megnézni!", "", 1);
            selectedMenus[2] = new SelectedMenu("Kerék Szenzor Értékek", "Itt tudod a legenerált a kerék szenzorok értékét megnézni!", "", 2);
            selectedMenus[3] = new SelectedMenu("JSON fájlba kiíratás", "Itt tudod kiíratni az eddigi mérés adatait .json fájlba!", "", 3);
            selectedMenus[4] = new SelectedMenu("Adatbázisba helyezés", "Itt tudod a legenerált méréseket adatbázisba helyezni!", "", 4);
            selectedMenus[5] = new SelectedMenu("LINQ lh7ekérdezések", "Itt tudod megnézni az előre megírt LINQ-s lekérdezéseket!", "", 5);
            currentSelectedMenuIndex = 0;
            inSelectedMenu = false;
        }

        public MainMenu(List<Sensor> lista)
        {
            selectedMenus = new SelectedMenu[6];
            selectedMenus[0] = new SelectedMenu("Motor Szenzor Értékek", "Itt tudod a legenerált a motor szenzorok értékét megnézni!", "", 0);
            selectedMenus[1] = new SelectedMenu("Fék Szenzor Értékek", "Itt tudod a legenerált a fék szenzorok értékét megnézni!", "", 1);
            selectedMenus[2] = new SelectedMenu("Kerék Szenzor Értékek", "Itt tudod a legenerált a kerék szenzorok értékét megnézni!", "", 2);
            selectedMenus[3] = new SelectedMenu("JSON fájlba kiíratás", "Itt tudod kiíratni az eddigi mérés adatait .json fájlba!", "", 3);
            selectedMenus[4] = new SelectedMenu("Adatbázisba helyezés", "Itt tudod a legenerált méréseket adatbázisba helyezni!", "", 4);
            selectedMenus[5] = new SelectedMenu("LINQ lekérdezések", "Itt tudod megnézni az előre megírt LINQ-s lekérdezéseket!", "", 5);
            currentSelectedMenuIndex = 0;
            inSelectedMenu = false;
            meresek.AddRange(lista);
        }

        public void MainMenuRunning()
        {
            ButtonPressed += this.OnButtonPressed;

            DisplayMenu();

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
            Console.WriteLine(meresek.Count());
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
        
        protected virtual void OnButtonPressed(object sender, EventArgs e)
        {
            if(inSelectedMenu)
            {
                selectedMenus[currentSelectedMenuIndex].HandleSelectedMenuInput(sender, e);
            }
            else
            {
                switch (sender)
                {
                    case ConsoleKey.Enter:      //menü választás
                        Console.Clear();
                        inSelectedMenu = true;                        
                        selectedMenus[currentSelectedMenuIndex].SelectedMenuKiir(meresek);                       
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
            return rng.Next(100, 500); // °C
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
