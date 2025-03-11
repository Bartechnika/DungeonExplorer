using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{   
    /// <summary>
    /// Class <c>ItemStack</c> is a container class for an Item type along with the amount stored.
    /// </summary>
    public class ItemStack
    {   
        public Item Item;
        public int Amount;

        public ItemStack(Item item, int amount)
        {
            Item = item;
            Amount= amount;
        }

        public override string ToString()
        {
            return $"{Item.ToString()} : {Amount}";
        }
    }
}
