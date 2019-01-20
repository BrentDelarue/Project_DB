using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Water
    {
        public string Naam { get; set; }
        public string Type { get { return "Water"; } }
        public int WaterDoel { get; set; }
        public int WaterGedronken { get; set; }
        public DateTime DatumTijd { get; set; }
        public string Datum { get; set; }
    }
}
