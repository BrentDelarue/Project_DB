using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class User
    {
        public string Type { get { return "User"; } }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ApiName { get; set; }
        public string Password { get; set; }
        public string Age { get; set; }
        public string Length { get; set; }
        public string Weight { get; set; }
    }
}
