using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Oefening
    {
        public string Naam { get; set; }
        public string Type { get { return "Oefening"; } }
        public string Workout { get; set; }
        public string Moeilijkheidsgraad { get; set; }
        public string Kcal { get; set; }
        public string MaxHeart { get; set; }
        public string AverageHeart { get; set; }
        public string Gevoel { get; set; }
        public string Duur { get; set; }
        public string Datum { get { return DateTime.Now.ToString("MM/dd/yyyy HH:mm"); } }
    }
}
