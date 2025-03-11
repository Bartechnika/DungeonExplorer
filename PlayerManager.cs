using System;
using System.Collections.Generic;
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
        public InventorySlot[] pockets = new InventorySlot[5];
        public int nextEmptyPocket;
        public InventorySlot[] rucksack = new InventorySlot[20];
        public int nextEmptyRucksack;
        public Dictionary<string, Item> items = new Dictionary<string, Item>();
        public Player player;

        public PlayerManager()
        {
            player = new Player();

            for (int i = 0; i < pockets.GetLength(0); i++)
            {
                pockets[i] = new InventorySlot();
            }
            nextEmptyPocket = 1;

            for (int i = 0; i < rucksack.GetLength(0); i++)
            {
                rucksack[i] = new InventorySlot();
            }
            nextEmptyRucksack = 1;

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
                    string type = itemElement.Attribute("type").Value;
                    switch (type)
                    {   
                        case "comfort-toy":
                            string baseBoost = itemElement.Attribute("baseBoost").Value;
                            Item item = new ComfortToy(id, name, int.Parse(baseBoost));
                            items.Add(id, item);
                            break;
                        case "card":
                            baseBoost = itemElement.Attribute("baseBoost").Value;
                            item = new ComfortToy(id, name, int.Parse(baseBoost));
                            items.Add(id, item);
                            break;
                        default:
                            throw new InstanceNotFoundException($"No item of type {type} exists");

                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }
        }

        public void PickupItem(string store, string id, int amount)
        {
            if(store == "pockets")
            {
                if(nextEmptyPocket == pockets.Length)
                {
                    Console.WriteLine("You cannot pick this item up, your pockets are full!");
                }
                else
                {
                    Item item = GetItem(id);
                    pockets[nextEmptyPocket - 1].ItemStack = new ItemStack(item, amount);
                    nextEmptyPocket++;
                }
            }

            if (store == "rucksack")
            {
                if (nextEmptyRucksack == rucksack.Length)
                {
                    Console.WriteLine("You cannot pick this item up, your rucksack is full!");
                }
                else
                {
                    Item item = GetItem(id);
                    rucksack[nextEmptyRucksack - 1].ItemStack = new ItemStack(item, amount);
                    nextEmptyRucksack++;
                }
            }
            Console.WriteLine();
        }

        public Item GetItem(string id)
        {
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

        public void CalculateBoost()
        {
            int boost = 0;
            foreach(InventorySlot slot in pockets)
            {
                boost += slot.ItemStack.Item.BaseBoost;
            }

            player.Resilience.Value = boost;
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
            string art = Game.GetArt("pockets");
            Console.WriteLine(art);
            var contents = new StringBuilder();
            foreach (var slot in pockets)
            {
                contents.Append(slot.ToString() + " ");
            }
            Console.WriteLine(contents.ToString());
            Console.WriteLine();
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