using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonExplorer
{   
    /// <summary>
    /// Class <c>ItemStack</c> is a container class for an Item type along with the amount stored.
    /// </summary>
    public class ItemStack
    {   
        public Item Item { get; set; }
        public int Amount { get; set; }
        public ItemStack(Item item, int amount)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item), "The item cannot be null.");
            Amount = amount;
        }

        public string GetDescription()
        {
            return Item.GetDescription();
        }

        public override string ToString()
        {
            return $"{Item.ToString()} : {Amount}";
        }
    }
}
