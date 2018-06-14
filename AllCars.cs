using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class AllCars
    {
        public string url { get; set; }
        public string name { get; set; }



          public List<Models> models { get; set; }

        public AllCars()
        {
            models = new List<Models>();
        }
    }
}
