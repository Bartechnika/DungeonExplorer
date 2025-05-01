using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
        public float Boost {  get; protected set; }
        public string Type { get; protected set; }

        public DateTime lastUsed;

        public Item(Id id, string name, string description)
        {
            ItemId = id ?? throw new ArgumentNullException(nameof(id), "The id cannot be null.");
            Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(name), "The description cannot be null.");
            Boost = 0;
            Type = "empty";
        }
        public virtual void Use() { Inventory.InspectItem(this); lastUsed = DateTime.Now; }
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
        public Empty(Id id, string name = "empty", string description="empty-inventory-slot") : base(id, name, description)
        {

        }
    }

    /// <summary>
    /// Class <c>Comfort : Item</c> represents an item that increases the users resilience
    /// by providing a comforting sensory experience with associated mental relief.
    /// </summary>
    public class ComfortToy : Item // boosts resilience
    {
        public ComfortToy(Id id, string name, float loveBoost, string description = "empty-inventory-slot") : base(id, name, description)
        {
            Boost = loveBoost;
            Type = "comfort-toy";
        }
        public override void Use()
        {
            base.Use();
            Console.WriteLine("You snuggle your toy and feel warmer inside <3");
            Game.Wait(3);
        }
    }

    public class Book : Item // boosts imagination
    {
        public Book(Id id, string name, float imaginationBoost, string description = "empty-inventory-slot") : base(id, name, description)
        {
            Boost = imaginationBoost;
            Type = "book";
        }

        public override void Use()
        {
            base.Use();
            Console.WriteLine("You discover pages of insightful infomation about 18th century peruvian merchants...");
            Game.Wait(3);
        }
    }

    public class Hat : Item // boosts identity
    {
        public Hat(Id id, string name, float resilienceBoost, string description = "empty-inventory-slot") : base(id, name, description)
        {
            Boost = resilienceBoost;
            Type = "hat";
        }
    }

    public class Snack : Item // boosts concentration 
    {
        public Snack(Id id, string name, float energyBoost, string description = "empty-inventory-slot") : base(id, name, description)
        {
            Boost = energyBoost;
            Type = "snack";
        }
    }




    /// <summary>
    /// Class <c>M : Item</c> represents an item that displays infomation or has some utility as
    /// an ID.
    /// </summary>
    public class Miscellaneous : Item
    {
        public Miscellaneous(Id id, string name, string description = "empty-inventory-slot") : base(id, name, description)
        {
            Type = "miscellaneous";
        }
    }
}
