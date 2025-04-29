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
        public string Name {  get; set; }
        public string dialogue;
        public PlayerManager PlayerManager;

        public Interaction(string name, string dialogue, PlayerManager playerManager)
        {
            this.Name = name;
            this.dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue));
            this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
        }
        public abstract bool Interact();
    }

    public class Dialogue : Interaction
    {
        public Dialogue(string name, string dialogue, PlayerManager playerManager) : base(name, dialogue, playerManager)
        {

        }

        public override bool Interact()
        {
            UI.WriteDialogue(this.dialogue);
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
        public FoundItem(string name, string dialogue, PlayerManager playerManager, Id id, int amount) : base(name, dialogue, playerManager)
        {
            this.item = Game.Items[id.Val] ?? throw new ArgumentNullException(nameof(id));
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
                UI.WriteDialogue(this.dialogue);
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
        public FightMonster(string name, string dialogue, PlayerManager playerManager, Id monsterId, string monsterName, int of) : base(name, dialogue, playerManager)
        {
            string myId = monsterId.Val;
            Monster monster = null;
            switch (myId)
            {
                case "#001":
                    monster = new SensoryMonster(monsterName, dialogue, of, playerManager);
                    break;
                case "#002":
                    monster = new SocialMonster(monsterName, dialogue, of, playerManager);
                    break;
                case "#003":
                    monster = new InternalMonster(monsterName, dialogue, of, playerManager);
                    break;
            }
            Monster = monster;
            //Monster = Game.Monsters[monsterId] ?? throw new ArgumentNullException(nameof(monsterId));
        }

        public override bool Interact()
        {
            return CreateMonster();
        }

        public bool CreateMonster()
        {
            bool playerDefeated = false;

            string fightArt = UI.GetFight(Monster);

            //string sel = Game.ValidateInputSelection("Fight or flight? (fight/flee) ", new string[] { "fight", "flee" });
            /*
            switch (sel)
            {
                case "fight":
                    Fight();
                    break;
                case "flee":
                    flee = true;
                    RunAway();
                    break;
            }*/

            playerDefeated = Fight(fightArt);

            return playerDefeated;
        }

        public void RunAway()
        {
            Console.WriteLine("You need to get out of here - you flee\n");
        }

        public bool Fight(string fightArt)
        {
            Ability ability;

            string[] ops = new string[5 + 1];
            ops[ops.Length - 1] = "¬";
            for(int i = 0; i < ops.Length-1; i++)
            {
                ops[i] = (i+1).ToString();
            }

            string sel;

            int pDmg;
            int mDmg;
            bool monsterDefeated = false;
            bool playerDefeated = false;
            while(!monsterDefeated && !playerDefeated)
            {
                // Monster turn
                mDmg = Monster.Attack();
                UI.UpdateFight(fightArt, Monster, PlayerManager, 2, 0, mDmg);

                if (PlayerManager.player.overwhelmed == true)
                {
                    playerDefeated = true;
                    break;
                }

                Console.WriteLine("\nPlease pick an ability (1-5)...\n");
                Console.WriteLine("¬ back");
                sel = Game.ValidateInputSelection("\n-> ", ops);
                if (sel != "¬")
                {
                    ability = CombatManager.abilitySelectionMatrix[sel];
                    ability.Use();
                    Game.Wait(2);

                    pDmg = CombatManager.abilityDamageMatrix[ability.Name][Monster.Type];

                    Monster.TakeDamage(pDmg);
                    UI.UpdateFight(fightArt, Monster, PlayerManager, 1, pDmg, 0, ability);
                    Console.ReadKey();
                }

                if (Monster.Energy.Value == 0)
                {
                    monsterDefeated = true;
                }

            }
            if(monsterDefeated)
            {
                Win();
            }

            return playerDefeated;
        }

        public void Win()
        {
            Console.WriteLine("You won the fight!");
        }
    }

    public class Mirror : Interaction
    {
        public Mirror(string name, string dialogue, PlayerManager playerManager) : base(name, dialogue, playerManager)
        {

        }

        public override bool Interact()
        {
            UI.WriteDialogue(this.dialogue);
            PlayerManager.PlayerState();
            return false;
        }
    }
}
