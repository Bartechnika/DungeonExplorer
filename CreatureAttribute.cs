using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
    /// Class <c>CreatureAttribute</c> represents an attribute of the player.
    /// </summary>
    public class CreatureAttribute
    {
        public string Name { get; private set; }
        public Creature creature { get; private set; }

        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                if (value < 0 || value > 100)
                {
                    _value = 0;
                }
            }
        }

        public CreatureAttribute(string name, int value=0)
        {
            // Use of guard clause
            Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null.");
            Value = value;
        }
    }
}
