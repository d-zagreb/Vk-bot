using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vk_Bot
{
    public class Counter
    {
        public long IdOfPerson { get; set; }
        public List<string> Storage { get; set; }
        public List<string> NameOfMark { get; set; }
        public Counter()
        {
            Storage = new List<string>();
            NameOfMark = new List<string>();
        }       
    }
}
