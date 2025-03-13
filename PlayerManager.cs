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
        private const int maxPocketSlots = 3;
        private const int maxRucksackSlots = 10;
        public InventorySlot[] pockets = new InventorySlot[maxPocketSlots];
        public InventorySlot[] rucksack = new InventorySlot[maxRucksackSlots];

        /// <value>
        /// Property <c>_nextEmptyPocket</c> points to the next inventory pocket slot that is available.
        /// </value>
        private int _nextEmptyPocket;
        public int NextEmptyPocket
        {
            get => _nextEmptyPocket;
            set
            {
                if(value > maxPocketSlots)
                {
                    _nextEmptyPocket = maxPocketSlots;
                }
                else
                {
                     _nextEmptyPocket = value;
                }
            }
        }

        /// <value>
        /// Property <c>_nextEmptyRucksack</c> points to the next inventory rucksack slot that is available.
        /// </value>
        private int _nextEmptyRucksack;
        public int NextEmptyRucksack
        {
            get => _nextEmptyRucksack;
            set
            {
                if (value > maxRucksackSlots)
                {
                    _nextEmptyRucksack = maxRucksackSlots;
                }
                else
                {
                    _nextEmptyRucksack = value;
                }
            }
        }
        public Dictionary<string, Item> items = new Dictionary<string, Item>();
        public Player player;

        public PlayerManager()
        {
            player = new Player();

            for (int i = 0; i < pockets.GetLength(0); i++)
            {
                pockets[i] = new InventorySlot();
            }
            NextEmptyPocket = 1;

            for (int i = 0; i < rucksack.GetLength(0); i++)
            {
                rucksack[i] = new InventorySlot();
            }
            NextEmptyRucksack = 1;

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
                            Item item = new ComfortToy(id, name, description, int.Parse(baseBoost));
                            items.Add(id, item);
                            break;
                        case "card":
                            baseBoost = itemElement.Attribute("baseBoost").Value;
                            item = new ComfortToy(id, name, description, int.Parse(baseBoost));
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

        public void PickupItem(string store, string id, int amount)
        {   
            if(store == "pockets")
            {
                if(NextEmptyPocket == pockets.Length)
                {
                    Console.WriteLine("You cannot pick this item up, your pockets are full!");
                }
                else
                {
                    Item item = GetItem(id);
                    InventorySlot slot = pockets[NextEmptyPocket - 1];
                    slot.ItemStack = new ItemStack(item, amount);
                    slot.IsEmpty = false;
                    NextEmptyPocket++;
                }
            }

            if (store == "rucksack")
            {
                if (NextEmptyRucksack == rucksack.Length)
                {
                    Console.WriteLine("You cannot pick this item up, your rucksack is full!");
                }
                else
                {
                    Item item = GetItem(id);
                    InventorySlot slot = rucksack[NextEmptyRucksack - 1];
                    slot.ItemStack = new ItemStack(item, amount);
                    slot.IsEmpty = false;
                    NextEmptyRucksack++;
                }
            }
            Console.WriteLine();
        }

        public Item GetItem(string id)
        {
            Debug.Assert(String.IsNullOrEmpty(id), "The id cannot be null"); // Check that an item id is provided
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

        public void CheckPockets()
        {
            Game.WriteDialogue("\nYou rummage through your pockets and find:\n");
            if(NextEmptyPocket == 1)
            {
                Console.WriteLine("Your pockets are empty!\n");
            }
            else
            {
                var contents = new StringBuilder();
                foreach (var slot in (pockets.Where(s => s.IsEmpty==false)))
                {
                    contents.Append($"\nName: {slot.ToString()}\nDescription: \n{slot.GetDescription()}\n");
                }
                Console.WriteLine(contents.ToString());
                Console.WriteLine();
            }
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