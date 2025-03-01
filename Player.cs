using System;
using System.Collections.Generic;
using System.Security.Policy;
using DungeonExplorer;

namespace DungeonExplorer
{
    public class Player
    {
        public string Name { get; private set; }
        public InventoryManager inventoryManager;
        public PlayerAttribute Hope;
        public PlayerAttribute Resilience;
        public PlayerAttribute Creativity;

        public Player(string name, int hope) 
        {
            Name = name;
            Hope = new PlayerAttribute("Hope", "100");
            Resilience = new PlayerAttribute("Resilience", "100");
            Creativity = new PlayerAttribute("Creativity", "100");
            inventoryManager = new InventoryManager();
        }

        public class PlayerAttribute
        {
            public string Name { get; private set; }
            public string Value { get; private set; }

            public PlayerAttribute(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public void AccumulateStuff(string stuff)
        {
            Console.WriteLine($"{stuff} is stored safely in your pocket.");
        }
        public string InventoryContents()
        {   
            return inventoryManager.ToString();
        }

        /// <summary>
        /// Output a visual representation of the player's state.
        /// </summary>
        public void PlayerState()
        {
            string player_art = Game.GetText("player");
            string s = player_art.Replace("{hope}", Hope.Value);
            s = s.Replace("{planning}", Resilience.Value);
            s = s.Replace("{creativity}", Creativity.Value);
            Console.WriteLine(s);
        }
    }
}