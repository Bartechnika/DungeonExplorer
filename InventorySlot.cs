using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
    /// Class <c>InventorySlot</c> reprents a single storage location accessible to the player,
    /// such as a pocket or rucksack pocket.
    /// </summary>
    /// <remarks>
    /// Default parameter for <c>ItemStack</c> is <c>Empty : Item<</c> when no <c>Item</c> proivded.
    /// </remarks>
    public class InventorySlot
    {
        public ItemStack ItemStack;
        public bool IsEmpty;
        public InventorySlot(ItemStack itemStack = null)
        {
            if (itemStack == null)
            {
                IsEmpty = true;
                ItemStack = new ItemStack(new Empty(), 0);
            }
            else
            {
                ItemStack = itemStack;
            }
        }

        public string GetDescription()
        {
            return ItemStack.GetDescription();
        }

        public override string ToString()
        {
            return ItemStack.ToString();
        }
    }
}
