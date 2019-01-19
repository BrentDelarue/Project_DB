using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Water
    {
        public string Naam { get; set; }
        public string Type { get { return "Water"; } }
        public string WaterDoel { get; set; }
        public string WaterGedronken { get; set; }
        public string Datum { get; set; }
    }
}
