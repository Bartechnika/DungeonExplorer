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
        public Id ItemId { get; private set; }

        /// <value>
        /// Property <c>name</c> is a string identifier displayed to the player
        /// to give infomation about the Item.
        /// </value>
        public string Name {  get; private set; }

        /// <value>
        /// Property <c>description</c> is a brief statement explaining what the item is.
        /// </value>
        public string Description { get; private set; }

        public Item(string id, string name, string description)
        {
            Console.WriteLine(id);
            ItemId.Val = id ?? throw new ArgumentNullException(nameof(id), "The id cannot be null.");
            Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(name), "The description cannot be null.");
        }

        public string GetDescription()
        {
            return Description;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Class <c>Empty : Item</c> only exists to symbolise an empty inventory slot.
    /// Interaction method is empty.
    /// </summary>
    public class Empty : Item
    {
        public Empty(string id="-1", string name = "empty", string description="empty-inventory-slot") : base(id, name, description)
        {

        }
    }

    /// <summary>
    /// Class <c>Card : Item</c> represents an item that displays infomation or has some utility as
    /// an ID.
    /// </summary>
    public class Card : Item
    {
        public Card(string id, string name, string description = "empty-inventory-slot") : base(id, name, description)
        {

        }
    }

    /// <summary>
    /// Class <c>Comfort : Item</c> represents an item that increases the users resilience
    /// by providing a comforting sensory experience with associated mental relief.
    /// </summary>
    public class ComfortToy : Item
    {
        public ComfortToy(string id, string name, string description = "empty-inventory-slot") : base(id, name, description)
        { 
            
        }
    }
}
