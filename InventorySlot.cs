using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class InventorySlot
    {
        public ItemStack itemStack;
        public InventorySlot()
        {
            itemStack = new ItemStack();
        }

        public override string ToString()
        {
            return itemStack.ToString();
        }
    }
}
