using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonExplorer
{
    public class PlayerManager
    {   

        public Dictionary<string, Item> items = new Dictionary<string, Item>();
        public Player player;
        readonly Inventory inventory;

        public PlayerManager()
        {
            player = new Player();
            inventory = new Inventory();
        }

        public void PickupItem(string store, Item item, int amount)
        {
            inventory.StoreItem(store, item, amount);
        }

        public Item GetItem(string id)
        {
            //Debug.Assert(String.IsNullOrEmpty(id), "The id cannot be null"); // Check that an item id is provided
            Item thisItem;
            try
            {
                thisItem = items[id];
            }
            catch
            {
                throw new KeyNotFoundException($"No item was found with id: {id}.");
            }

            return items[id];
        }

        public int TakeDamage(int overwhelmFactor)
        {
            int damage = overwhelmFactor * (1 - player.Resilience.Value); // formula for damage
            player.Energy.Value -= damage;
            //Console.WriteLine($"\nYour energy was reduced to {player.Energy.Value} by {overwhelmFactor} points.\n");
            if (player.Energy.Value == 0)
            {
                player.overwhelmed = true;
            }

            return damage;
        }

        public void CheckPockets()
        {
            inventory.CheckPockets();
        }

        public void CheckRucksack()
        {
            inventory.CheckRucksack();
        }

        /// <summary>
        /// Output a visual representation of the player's state.
        /// </summary>
        public void PlayerState()
        {
            /*
            string player_art = UI.GetArt("player");
            string s = player_art.Replace("{resilience}", player.Resilience.Value.ToString());
            s = s.Replace("{imagination}", player.Imagination.Value.ToString());
            s = s.Replace("{energy}", player.Energy.Value.ToString());
            Console.WriteLine(s);
            */
        }
    }
}