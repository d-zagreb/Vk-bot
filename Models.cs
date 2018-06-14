using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Models
    {
        public string Url { get;  set; }
        public string Name { get; set; }

        public List<Varieties> Varieties { get; set; }

        public Models()
        {
            Varieties = new List<Varieties>();
        }
    }
}
