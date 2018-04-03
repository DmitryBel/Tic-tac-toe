using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Models
{
    public class Turn
    {
        public string player { get; set; }
        public int position { get; set; }
        public string mark { get; set; }
    }
}