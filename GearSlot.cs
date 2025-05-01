using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class GearSlot
    {
        public Item item;
        public bool isEmpty = true;
        private string _name;
        public string Name
        {
            get
            {
                if (isEmpty) { return ""; } else { return item.Name; }
            }
            private set => _description = value;
        }
        private string _description;
        public string Description
        {
            get
            {
                if(isEmpty) { return ""; } else { return item.Description; }
            }
            private set => _description = value;
        }
        private float _boost;
        public float Boost
        {
            get
            {
                if (isEmpty) { return 0; } else { return item.Boost; }
            }
            private set {  _boost = value; }
        }
        public string Type;
        public GearSlot(string type)
        {
            Type = type;
        }
        public void Use()
        {
            if (isEmpty) { Console.WriteLine("No item equipped!"); Game.Wait(2);  } else { item.Use(); }
        }
    }
}
