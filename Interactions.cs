using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
    /// Class <c>Interaction</c> is the base class for every interaction: the basic unit of play for a location.
    /// </summary>
    public abstract class Interaction
    {
        public string dialogue;
        public PlayerManager PlayerManager;

        public Interaction(string dialogue, PlayerManager playerManager)
        {
            this.dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue));
            this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
        }
        public abstract bool Interact();
    }

    public class Dialogue : Interaction
    {
        public Dialogue(string dialogue, PlayerManager playerManager) : base(dialogue, playerManager)
        {

        }

        public override bool Interact()
        {
            Game.WriteDialogue(this.dialogue);
            Console.WriteLine("");
            return false;
        }
    }

    public class FoundItem : Interaction
    {
        private Item item { get; }
        private int Amount { get; set; }
        private string Store { get; set; }
        private bool Found { get; set; }
        public FoundItem(string dialogue, PlayerManager playerManager, Id id, int amount) : base(dialogue, playerManager)
        {
            this.item = Game.Items[id] ?? throw new ArgumentNullException(nameof(id));
            this.Found = false;
        }

        /// <summary>
        /// Method <c>Interact</c> triggers the <c>Playermanager.PickupItem()</c> method only if the item has not already been found.
        /// </summary>
        /// <returns>true if player flees room as a result; false otherwise</returns>
        public override bool Interact()
        {
            if (Found)
            {
                Console.WriteLine("Item found!\n");
            }
            else
            {
                Game.WriteDialogue(this.dialogue);
                string sel = Game.ValidateInputSelection("Store item in pockets or rucksack? (pockets/rucksack): ", new string[] { "pockets", "rucksack" });
                PlayerManager.PickupItem(sel, item, Amount);
                Found = true;
                return false;
            }
            return false;
        }
    }


    public class FightMonster : Interaction
    {
        public Monster Monster;
        public FightMonster(string dialogue, PlayerManager playerManager, Id monsterId) : base(dialogue, playerManager)
        {
            Monster = Game.Monsters[monsterId] ?? throw new ArgumentNullException(nameof(monsterId));
        }

        public override bool Interact()
        {
            return CreateMonster();
        }

        public bool CreateMonster()
        {
            bool flee = false;

            Game.WriteDialogue($"*{Monster.Aura}*");
            string art = Game.GetArt("creature");
            art = Game.PopulateField(art, "{name~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}", Monster.Name);
            art = Game.PopulateField(art, "{type~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}", Monster.Type);
            art = Game.PopulateField(art, "{OF~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}", Monster.OverwhelmFactor.ToString());
            Console.WriteLine(art);

            Game.WriteDialogue(Monster.Dialogue);
            string sel = Game.ValidateInputSelection("Fight or flight? (fight/flee) ", new string[] { "fight", "flee" });

            switch (sel)
            {
                case "fight":
                    Fight();
                    break;
                case "flee":
                    flee = true;
                    RunAway();
                    break;
            }

            return flee;
        }

        public void RunAway()
        {
            Console.WriteLine("You need to get out of here - you flee\n");
        }

        public void Fight()
        {
            Console.WriteLine("You will stay and work through the pain");
            Monster.Attack();
        }
    }

    public class Mirror : Interaction
    {
        public Mirror(string dialogue, PlayerManager playerManager) : base(dialogue, playerManager)
        {

        }

        public override bool Interact()
        {
            Game.WriteDialogue(this.dialogue);
            PlayerManager.PlayerState();
            PlayerManager.InventoryContents();
            return false;
        }
    }
}
