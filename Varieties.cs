using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Varieties
    {
        public string Text { get; set; }
        public List<Characteristics> MainCharacteristics { get; set; }
        public Varieties()
        {
            MainCharacteristics = new List<Characteristics>();
        }
    }
}
