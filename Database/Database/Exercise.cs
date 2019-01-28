using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //-----------------------Paramaters instellen in Exercise object-------------------------//
    //---------------------------------------------------------------------------------------//

    public class Exercise
    {
        public string Name { get; set; }
        public string Type { get { return "Exercise"; } }
        public string Workout { get; set; }
        public string Difficulty { get; set; }
        public string Feeling { get; set; }
        public string Duration { get; set; }
        public string Repetitions { get; set; }
        public string RepetitionsColor { get; set; }
        public string Date { get; set; }
    }
}
