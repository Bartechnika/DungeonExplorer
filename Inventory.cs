using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class Inventory
    {
        public const int maxPocketSlots = 12;

        public GearSlot comfortToy;
        public GearSlot book;
        public GearSlot hat;
        public GearSlot snack;
        public GearSlot[] gearSlots;

    public InventorySlot[] Pockets { get; } = new InventorySlot[maxPocketSlots];

        /// <value>
        /// Property <c>_nextEmptyPocket</c> points to the next inventory pocket slot that is available.
        /// </value>
        private int _nextEmptyPocket;
        public int NextEmptyPocket
        {
            get => _nextEmptyPocket;
            set
            {
                if (value > maxPocketSlots)
                {
                    _nextEmptyPocket = maxPocketSlots;
                }
                else
                {
                    _nextEmptyPocket = value;
                }
            }
        }

        public Inventory()
        {
            for (int i = 0; i < Pockets.GetLength(0); i++)
            {
                Pockets[i] = new InventorySlot();
            }
            NextEmptyPocket = 1;

            comfortToy = new GearSlot("comfort-toy");
            book = new GearSlot("book");
            hat = new GearSlot("hat");
            snack = new GearSlot("snack");

            gearSlots = new GearSlot[] { comfortToy, book, hat, snack };
        }

        /// <summary>
        /// Method <c>InitialiseRoom</c> loads all necessary data for the room from the "rooms.xml" file.
        /// </summary>
        /// <param name="room"></param>
        /// <exception cref="MissingFieldException"></exception>
        public void StorePocket(Item item, int amount)
        {
            if (NextEmptyPocket == maxPocketSlots)
            {
                Console.WriteLine("You cannot pick this item up, your Pockets are full!");
            }
            else
            {
                InventorySlot slot = Pockets[NextEmptyPocket - 1];
                slot.ItemStack = new ItemStack(item, amount);
                slot.IsEmpty = false;
                NextEmptyPocket++;
            }
        }
            
        public void CheckPockets()
        {
            Dictionary<string, string> ops = new Dictionary<string, string>()
            {
                {"1", "Inspect Items"},
                {"2", "Assign Gear"},
                {"3", "Filter Inventory"},
                {"¬", "back" }
            };

            string sel;
            bool cont = true;

            while (cont)
            {
                UI.GetLocation(GameMap.CurRoom.CurLoc, Game.playerManager);
                UI.GetPockets(Pockets);
                sel = Game.SelectOption("Select an action to perform with your inventory", ops);
                switch(sel) 
                { 
                    case "1": { InspectItems(); break; }
                    case "2": { AssignGear(); break; }
                    case "3": { FilterInventory(); break; }
                    case "¬": { cont = false; break; }
                }

            }
        }

        public void InspectItems()
        {
            string[] ops = new string[maxPocketSlots + 1];
            ops[ops.Length - 1] = "¬";

            for (int i = 0; i < maxPocketSlots; i++)
            {
                ops[i] = ((char)(i + 65)).ToString();
            }

            string sel;

            bool cont = true;
            while (cont)
            {
                Console.WriteLine("\nSelect an item to view in detail (A, B...)\n");
                Console.WriteLine("¬ back");
                sel = Game.ValidateInputSelection(ops);
                if (sel == "¬") { break; }
                InspectItem(Pockets[(int)(sel[0]) - 97].ItemStack.Item);
            }
        }

        public static void InspectItem(Item item)
        {
            Console.WriteLine(($"\nName: {item.ToString()}\nDescription: \n{item.GetDescription()}\n"));
        }

        public void FilterItems()
        {

        }

        public void AssignGear()
        {
            string[] ops = new string[maxPocketSlots + 1];
            ops[ops.Length - 1] = "¬";

            for (int i = 0; i < maxPocketSlots; i++)
            {
                ops[i] = ((char)(i + 65)).ToString();
            }

            string sel;

            bool cont = true;
            while (cont)
            {
                Console.WriteLine("\nSelect an item to equip as gear (A, B...)\n");
                Console.WriteLine("¬ back");
                sel = Game.ValidateInputSelection(ops);
                if (sel == "¬") { break; }
                EquipGear(Pockets[(int)(sel[0]) - 97]);
            }
        }

        public void EquipGear(InventorySlot slot)
        {
            bool equipped = true;
            switch(slot.ItemStack.Item.Type)
            {
                case "comfort-toy": { comfortToy.item = slot.ItemStack.Item; comfortToy.isEmpty = false; break; }
                case "book": { book.item = slot.ItemStack.Item; book.isEmpty = false; break; }
                case "hat": { hat.item = slot.ItemStack.Item; hat.isEmpty = false; break; }
                case "snack": { snack.item = slot.ItemStack.Item; snack.isEmpty = false; break; }
                default: { equipped = false; break; }
            }

            if(!equipped)
            {
                Console.WriteLine("Cannot equip item: not gear");
            }
            else
            {
                Console.WriteLine("Item equipped successfully!");
                Game.playerManager.UpdateStats();
            }
        }

        public List<InventorySlot> GetGear(GearSlot gearSlot)
        {
            InventorySlot[] filteredPockets = new InventorySlot[maxPocketSlots];
            for(int i=0; i<maxPocketSlots; i++)
            {
                filteredPockets[i] = new InventorySlot();
            }
            InventorySlot[] availableGear = (from item in Pockets where item.ItemStack.Item.Type == gearSlot.Type select item).ToArray();
            for(int i = 0; i < availableGear.Length; i++)
            {
                filteredPockets[i] = availableGear[i];
            }
            return filteredPockets.ToList();
        }

        public void FilterInventory()
        {
            List<InventorySlot> filteredItems;
            string sel;
            bool cont = true;       // Comfort toy | book | hat | snack
            Dictionary<string, string> ops = new Dictionary<string, string>()
            {
                {"1", "Comfort toys"},
                {"2", "Books"},
                {"3", "Hats"},
                {"4", "Snacks" },
                {"¬", "back" }
            };
            while (cont)
            {
                sel = Game.SelectOption("Please choose which filter to use", ops);
                if(sel=="¬") { cont = false; break; }
                filteredItems = GetGear(Game.playerManager.inventory.gearSlots[int.Parse(sel)-1]);

                UI.ClearConsole();
                UI.GetLocation(GameMap.CurRoom.CurLoc, Game.playerManager);
                UI.GetPockets(filteredItems.ToArray());
            }
        }
    }
}
