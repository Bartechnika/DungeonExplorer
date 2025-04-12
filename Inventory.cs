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
        public const int maxPocketSlots = 3;
        public const int maxRucksackSlots = 10;
        public InventorySlot[] Pockets { get; } = new InventorySlot[maxPocketSlots];
        public InventorySlot[] Rucksack { get; }  = new InventorySlot[maxRucksackSlots];

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

        public Inventory()
        {
            for (int i = 0; i < Pockets.GetLength(0); i++)
            {
                Pockets[i] = new InventorySlot();
            }
            NextEmptyPocket = 1;

            for (int i = 0; i < Rucksack.GetLength(0); i++)
            {
                Rucksack[i] = new InventorySlot();
            }
            NextEmptyRucksack = 1;
        }

        /// <summary>
        /// Method <c>InitialiseRoom</c> loads all necessary data for the room from the "rooms.xml" file.
        /// </summary>
        /// <param name="room"></param>
        /// <exception cref="MissingFieldException"></exception>
        public void StoreItem(string store, Item item, int amount)
        {
            if (store == "Pockets")
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

            if (store == "Rucksack")
            {
                if (NextEmptyRucksack == maxRucksackSlots)
                {
                    Console.WriteLine("You cannot pick this item up, your Rucksack is full!");
                }
                else
                {
                    InventorySlot slot = Rucksack[NextEmptyRucksack - 1];
                    slot.ItemStack = new ItemStack(item, amount);
                    slot.IsEmpty = false;
                    NextEmptyRucksack++;
                }
            }
            Console.WriteLine();
        }

        public void InventoryContents()
        {
            string art = Game.GetArt("Pockets");
            Console.WriteLine(art);
            var contents = new StringBuilder();
            foreach (var slot in Pockets)
            {
                contents.Append(slot.ToString() + " ");
            }
            Console.WriteLine(contents.ToString());
            Console.WriteLine();
        }

        public void CheckPockets()
        {
            Game.WriteDialogue("\nYou rummage through your Pockets and find:\n");
            if (NextEmptyPocket == 1)
            {
                Console.WriteLine("Your Pockets are empty!\n");
            }
            else
            {
                var contents = new StringBuilder();
                foreach (var slot in (Pockets.Where(s => s.IsEmpty == false)))
                {
                    contents.Append($"\nName: {slot.ToString()}\nDescription: \n{slot.GetDescription()}\n");
                }
                Console.WriteLine(contents.ToString());
                Console.WriteLine();
            }
        }

        public void CheckRucksack()
        {
            Game.WriteDialogue("\nYou rummage through your Rucksack and find:\n");
            if (NextEmptyPocket == 1)
            {
                Console.WriteLine("Your Rucksack is empty!\n");
            }
            else
            {
                var contents = new StringBuilder();
                foreach (var slot in (Rucksack.Where(s => s.IsEmpty == false)))
                {
                    contents.Append($"\nName: {slot.ToString()}\nDescription: \n{slot.GetDescription()}\n");
                }
                Console.WriteLine(contents.ToString());
                Console.WriteLine();
            }
        }
    }
}
