using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //-------------------------Parameters instellen in Water object--------------------------//
    //---------------------------------------------------------------------------------------//

    public class Water
    {
        public string Name { get; set; }
        public string Type { get { return "Water"; } }
        public int WaterGoal { get; set; }
        public int WaterDrunk { get; set; }
        public DateTime DateTime { get; set; }
        public string Date { get; set; }
    }
}
