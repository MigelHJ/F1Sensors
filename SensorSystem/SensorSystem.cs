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
    }

    public class Engine : Sensor    // RPM, Hűtőfolyadék hőmérséklet érzékelő, Olajnyomás érzékelő
    {

    }
    public class Brake : Sensor     // Féknyomás, fékhőmérséklet, fékerő
    {

    }

    public class Tyre : Sensor // Gumihőmérséklet, Keréknyomás-érzékelő, Kopásérzékelő
    {

    }
}
