using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class InventoryManager
    {
        public InventorySlot[,] inventory = new InventorySlot[3, 9];

        public InventoryManager()
        {   
            for (int i = 0; i < inventory.GetLength(0); i++) 
            {   
                for(int j = 0; j < inventory.GetLength(1); j++)
                {
                    inventory[i,j] = new InventorySlot();
                }
            }
        }

        public override string ToString()
        {
            string contents = "";
            foreach(var slot in inventory)
            {
                contents += " " + slot.ToString();
            }
            return contents;
        }
    }
}