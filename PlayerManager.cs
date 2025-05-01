using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonExplorer
{
    public class PlayerManager
    {   
        public Player player;
        public readonly Inventory inventory;

        public PlayerManager()
        {
            player = new Player();
            inventory = new Inventory();
        }
        /*
        public void ChooseItems()
        {
            UI.GetPockets(inventory.Pockets);

            string[] ops = new string[Inventory.maxPocketSlots];

            for (int i = 0; i < Inventory.maxPocketSlots; i++)
            {
                ops[i] = ((char)(i + 65)).ToString();
            }
            char sel1;
            char sel2;

            bool cont = true;
            Item nextItem = null;
            for (int i = 0; i < Inventory.maxHotbarSlots; i++)
            {
                while (cont)
                {
                    Console.WriteLine("\nSelect the next item to view in detail (A, B...)\n");
                    sel1 = Game.ValidateInputSelection("\n-> ", ops)[0];
                    nextItem = (inventory.Pockets[(int)(sel1) - 97].ItemStack.Item);
                    inventory.InspectItem(nextItem);

                    Console.WriteLine("Type Y to confirm your choice, or N to go back");
                    sel2 = Game.ValidateInputSelection("\n-> ")[0];

                    if (sel2 == 'y') { break; }

                    UI.ClearConsole();
                    UI.GetLocation(GameMap.CurRoom.CurLoc);
                    UI.GetPockets(inventory.Pockets);
                }

                Console.WriteLine($"\nYou have selected {nextItem.Name} as your {i} item.");
                inventory.StoreHotbar(nextItem);
            }
        }*/

        public void PickupItem(string store, Item item, int amount)
        {
            inventory.StorePocket(item, amount);
        }

        public Item GetItem(string id)
        {
            //Debug.Assert(String.IsNullOrEmpty(id), "The id cannot be null"); // Check that an item id is provided
            Item thisItem;
            try
            {
                thisItem = GameMap.Items[id];
            }
            catch
            {
                throw new KeyNotFoundException($"No item was found with id: {id}.");
            }

            return GameMap.Items[id];
        }

        public void UseToy()
        {
            
            if (DateTime.Now.Subtract(inventory.comfortToy.item.lastUsed).TotalSeconds < 60)
            {
                Console.WriteLine("Your toy is looking a little worn...");
            }
            else
            {
                inventory.comfortToy.Use();
                player.Love.Level(inventory.comfortToy.item.Boost);
            }
            Game.Wait(2);
        }


        public void EatSnack()
        {
            inventory.snack.Use();
            player.Heal(inventory.snack.Boost);
        }

        public void CheckPockets()
        {
            inventory.CheckPockets();
        }

        public void AssignGear()
        {
            inventory.AssignGear();
        }

        public float CalculateXP(Monster monster)
        {
            return monster.Energy.MaxValue + monster.Damage.Value; // max value of 200
        }

        public void UpdateStats()
        {
            player.Resilience.Value = player.Resilience.BaseValue + inventory.comfortToy.Boost;
            player.Imagination.Value = player.Imagination.BaseValue + inventory.book.Boost;
        }
    }
}