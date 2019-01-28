using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //--------------------Paramaters instellen in ProfilePicture object----------------------//
    //---------------------------------------------------------------------------------------//

    public class ProfilePicture
    {
        public string Name { get; set; }
        public byte[] stream { get; set; }
    }
}
