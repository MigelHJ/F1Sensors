using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SensorSystem
{
    #region Menükezelése:

    public delegate void ButtonPressedEventHandler(object sender, EventArgs e);

    public class SelectedMenu
    {
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
    }

    public class MainMenu
    {
        public event ButtonPressedEventHandler ButtonPressed; 
        public SelectedMenu[] selectedMenus;

        private int currentSelectedMenuIndex;

        public MainMenu()
        {
            selectedMenus = new SelectedMenu[2];
            selectedMenus[0] = new SelectedMenu("RPM","Itt tudod az RPM-et megváltoztatni!", "RPM = ",0);
            selectedMenus[1] = new SelectedMenu("Olajnyomás", "Itt tudod az RPM-et megváltoztatni!", "RPM = ", 1);
            currentSelectedMenuIndex = 0;
        }

        public void MainMenuRunning()
        {
            ButtonPressed += this.OnButtonPressed;

            Console.Clear();

            foreach (var menu in selectedMenus)
            {
                if (menu.menuIndex == currentSelectedMenuIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(menu.menuDisplayName +"\t <----");
                    Console.ForegroundColor = ConsoleColor.White;

                }
                else
                {                    
                    Console.WriteLine(menu.menuDisplayName);
                }

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
            switch (sender)
            {
                case ConsoleKey.Enter:      //menü választás
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
        public string szenzorErtek { get; set; } //
        public string szenzorErtekTartomany { get; set; } //
        public string mertekEgyseg { get; set; } // C°, km/h, rpm...
        public string szenzorHely { get; set; }    
        
        public Sensor(string szA, string szTip, string szErt, string szErtTar, string mE, string szH)
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
        public Engine(string szA,string szTip,string szErt,string szErtTar,string mE,string szH):base(szA, szTip, szErt, szErtTar, mE, szH)
        {
            szenzorAzon = szA;
            szenzorTípus = szTip;
            szenzorErtek = szErt;
            szenzorErtekTartomany = szErtTar;
            mertekEgyseg = mE;
            szenzorHely = szH;
        }
    
        public int revRPM()
        {
            try
            {
                if (this.szenzorTípus == "RPM_mérő")
                {
                    Random rng = new Random();
                    return 100 + rng.Next(50, 115);
                }
                else
                {
                    throw new Exception("Nem lehetséges a motornak a forgatása");
                }
                
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public int deRevRPM()
        {
            try
            {
                if (this.szenzorTípus == "RPM_mérő")
                {
                    Random rng = new Random();
                    return -(100 + rng.Next(50, 115));
                }
                else
                {
                    throw new Exception("Nem lehetséges a motornak a pihentetése");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    
    
    }


    //public class Brake : Sensor     // Féknyomás, fékhőmérséklet, fékerő
    //{

    //}

    //public class Tyre : Sensor // Gumihőmérséklet, Keréknyomás-érzékelő, Kopásérzékelő
    //{

    //}
    #endregion
}
