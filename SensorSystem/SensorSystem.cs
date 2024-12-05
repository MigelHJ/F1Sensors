using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorSystem
{
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
}
