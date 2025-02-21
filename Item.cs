using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class Item
    {
        public string name {  get; set; }
        public int id {  get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
