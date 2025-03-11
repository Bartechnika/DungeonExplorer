using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
    /// Abstract class <c>Item</c> used as a base class for other items.
    /// </summary>
    public abstract class Item
    {   
        /// <value>
        /// Property <c>id</c> is a string identifier of numerical chars that
        /// uniquely represents every Item type.
        /// </value>
        public string id { get; private set; }

        /// <value>
        /// Property <c>name</c> is a string identifier displayed to the player
        /// to give infomation about the Item.
        /// </value>
        public string name {  get; private set; }

        /// <value>
        /// Property <c>BaseBoost</c> represents the boost given to the player's
        /// resilience.
        /// </value>
        public int BaseBoost;

        public Item(string id, string name, int baseBoost=0)
        {
            this.id = id;
            this.name = name;
            this.BaseBoost = baseBoost;
        }

        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// Class <c>Empty : Item</c> only exists to symbolise an empty inventory slot.
    /// Interaction method is empty.
    /// </summary>
    public class Empty : Item
    {
        public Empty(string id="-1", string name = "empty", int baseBoost = 0) : base(id, name, baseBoost)
        {

        }
    }

    /// <summary>
    /// Class <c>Comfort : Item</c> represents an item that increases the users resilience
    /// by providing a comforting sensory experience with associated mental relief.
    /// Interaction method is empty.
    /// </summary>
    public class Card : Item
    {
        public Card(string id, string name, int baseBoost=0) : base(id, name, baseBoost)
        {

        }
    }

    /// <summary>
    /// Class <c>Comfort : Item</c> represents an item that increases the users resilience
    /// by providing a comforting sensory experience with associated mental relief.
    /// Interaction method is empty.
    /// </summary>
    public class ComfortToy : Item
    {
        public ComfortToy(string id, string name, int baseBoost) : base(id, name, baseBoost)
        { 
            
        }
    }
}
