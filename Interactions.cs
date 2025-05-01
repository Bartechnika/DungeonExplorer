using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        public bool Completed;

        public Interaction(string name, string dialogue, PlayerManager playerManager)
        {
            this.Name = name;
            this.dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue));
            this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            Completed = false;
        }
        public abstract void Interact();
    }

    public class Dialogue : Interaction
    {
        public Dialogue(string name, string dialogue, PlayerManager playerManager) : base(name, dialogue, playerManager)
        {

        }

        public override void Interact()
        {
            UI.WriteDialogue(this.dialogue);
            Console.WriteLine("");
            UI.WaitForUser();

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
            this.item = GameMap.Items[id.Val] ?? throw new ArgumentNullException(nameof(id));
            this.Found = false;
        }

        /// <summary>
        /// Method <c>Interact</c> triggers the <c>Playermanager.PickupItem()</c> method only if the item has not already been found.
        /// </summary>
        /// <returns>true if player flees room as a result; false otherwise</returns>
        public override void Interact()
        {
            if (Found)
            {
                Console.WriteLine("Item found!\n");
            }
            else
            {
                UI.WriteDialogue(this.dialogue);
                Console.WriteLine("Would you like to store this item in your pockets? (Y/N)");
                string sel = Game.ValidateInputSelection();

                if(sel=="y")
                {   
                    PlayerManager.PickupItem(sel, item, Amount);
                    Found = true;
                }
                else
                {
                    Console.WriteLine("Leaving item behind...");
                }
            }
        }
    }

    public class FightMonster : Interaction
    {
        public Monster Monster;
        public FightMonster(Id id, string name, string dialogue, PlayerManager playerManager) : base(name, dialogue, playerManager)
        {
            Monster = GameMap.Monsters[id.Val];
        }

        public override void Interact()
        {
            if( Monster.Defeated == true)
            {
                Console.WriteLine("Monster defeated! Please try another interaction.");
            }
            else
            {
                CreateMonster();
            }
        }

        public bool CreateMonster()
        {
            bool playerDefeated = false;

            //ChooseItems();

            string fightArt = UI.GetFight(Monster, PlayerManager);

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

        /*
        public void ChooseItems()
        {
            string chooseItems = UI.GetArt("monster_start", Game.monsterDir);
            Console.WriteLine(chooseItems);
            PlayerManager.ChooseItems();
            UI.ClearConsole();
        }*/

        public bool Fight(string fightArt)
        {
            float mDmg;
            string sel;
            string[] ops = new string[] { "A", "B" };
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

                sel = Game.ValidateInputSelection(ops);
                if (sel == "a")
                {
                    PickAbility(fightArt);
                }
                else
                {
                    PickAction(fightArt);
                }

                if (Monster.Energy.Value == 0)
                {
                    monsterDefeated = true;
                }

            }
            if(monsterDefeated)
            {
                float oldResilience = PlayerManager.player.Resilience.Value;
                float xp = PlayerManager.CalculateXP(Monster);
                float newResilience = PlayerManager.player.Resilience.Value;
                Monster.Defeated = true;
                UI.WinFight(xp, oldResilience, newResilience);
                Game.Wait(10);
                UI.WaitForUser();
            }

            return playerDefeated;
        }

        public void PickAbility(string fightArt)
        {
            Ability ability;

            Dictionary<string, string> ops = new Dictionary<string, string>()
            {
                {"1", "breathe"},
                {"2", "distract"},
                {"3", "ground"},
                {"4", "express"},
                {"5", "reassure"},
            };
            string sel;
            float pDmg;

            Console.WriteLine("\nPlease pick an ability (1-5)...\n");
            Console.WriteLine("¬ back");
            sel = Game.SelectOption("Please pick an ability (1-5)..", ops);
            if (sel != "¬")
            {
                ability = PlayerManager.player.abilitySelectionMatrix[sel];
                ability.Use();
                Game.Wait(5);
                pDmg = Math.Min(ability.Damage * CombatManager.abilityDamageMatrix[ability.Name][Monster.Type], Monster.Energy.Value);
                Monster.TakeDamage(pDmg);
                UI.UpdateFight(fightArt, Monster, PlayerManager, 1, pDmg, 0, ability);

                ability.LevelUp(pDmg);
                Game.Wait(2);
            }
        }

        public void PickAction(string fightArt)
        {
            Dictionary<string, string> ops = new Dictionary<string, string>()
            {
                {"1", "Comfort toy"},
            };
            string sel = Game.SelectOption("Please pick an action", ops);
            switch (sel)
            {
                case "1": 
                    {
                        if(PlayerManager.inventory.comfortToy.isEmpty == true)
                        {
                            Console.WriteLine("You don't have a toy to hug :(");
                            Game.Wait(2);
                        }
                        break; 
                    }
                case "2":
                    {
                        if (PlayerManager.inventory.snack.isEmpty == true)
                        {
                            Console.WriteLine("You don't have a snack to use!");
                            Game.Wait(2);
                        }
                        break;
                    }
            }
        }
    }

    public class Mirror : Interaction
    {
        public Mirror(string name, string dialogue, PlayerManager playerManager) : base(name, dialogue, playerManager)
        {

        }

        public override void Interact()
        {
            UI.WriteDialogue(this.dialogue);
        }
    }
}
