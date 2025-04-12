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

            /* Create a dictionary <items> and initialise
             * every item in the items.xml file. The dictionary stores
             * items as a key value pair where the key is the string
             * identifier (xml element id) for the item.
             */
            
            // Add the empty item
            items.Add("-1", new Empty());

            string path = Game.textDir + "items.xml";
            if (File.Exists(path))
            {
                XElement allItems = XElement.Load(path);
                List<XElement> itemElements = allItems.Elements().ToList();
                foreach (XElement itemElement in itemElements)
                {
                    string id = itemElement.Attribute("id").Value;
                    string name = itemElement.Attribute("name").Value;
                    string description = itemElement.Value.Trim();
                    string type = itemElement.Attribute("type").Value;
                    switch (type)
                    {   
                        case "comfort-toy":
                            string baseBoost = itemElement.Attribute("baseBoost").Value;
                            Item item = new ComfortToy(id, name, description);
                            items.Add(id, item);
                            break;
                        case "card":
                            baseBoost = itemElement.Attribute("baseBoost").Value;
                            item = new ComfortToy(id, name, description);
                            items.Add(id, item);
                            break;
                        default:
                            throw new InstanceNotFoundException($"No item of type {type} exists");

                    }
                }
            }
            else
            {
                //throw new FileNotFoundException("File does not exist...");
            }
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

        public void TakeDamage(int overwhelmFactor)
        {
            player.Energy.Value -= overwhelmFactor;
            Console.WriteLine($"\nYour energy was reduced to {player.Energy.Value} by {overwhelmFactor} points.\n");
            if (player.Energy.Value == 0)
            {
                player.Rest();
            }
        }

        public void InventoryContents()
        {
            inventory.InventoryContents();
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
            string player_art = Game.GetArt("player");
            string s = player_art.Replace("{resilience}", player.Resilience.Value.ToString());
            s = s.Replace("{imagination}", player.Imagination.Value.ToString());
            s = s.Replace("{energy}", player.Energy.Value.ToString());
            Console.WriteLine(s);
        }
    }
}