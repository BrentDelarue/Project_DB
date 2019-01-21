using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Gebruiker
    {
        public string Type { get { return "Gebruiker"; } }
        public string Email { get; set; }
        public string Naam { get; set; }
        public string Wachtwoord { get; set; }
        public string Leeftijd { get; set; }
        public string Lengte { get; set; }
        public string Gewicht { get; set; }
        public string API { get; set; }
        public string Token { get; set; }
    }
}
