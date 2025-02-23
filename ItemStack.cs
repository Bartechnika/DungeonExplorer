using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class ItemStack
    {
        public Item item;
        public int amount;

        public ItemStack()
        {
            item = new Item("Sword", 5);
            amount = 0;
        }

        public override string ToString()
        {
            return $"{item.ToString()} : {amount}";
        }
    }
}
