using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonExplorer
{
    public static class UI
    {
        const string roomName = "{room-name~~~~~~~~~~}";

        const string locName = "{location-name~~~~~~~~~~~}";
        const string locDesc = "{description~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";
        const string locDialogue = "{intro~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";
        const string locNav = "~~~~~~~~~~}";
        const string locInt = "{int1~~~~~~~~~~}";

        const string playerResilience = "{resilience}";
        const string playerEnergy = "{energy}";

        const string itemName = "{itemZ~~~~~~~~~}";
        const string itemQuantity = "{Zq}";

        const string monsterAura = "{aura~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";
        const string monsterName = "{name~~~~~~~~~~~~~~~~~~~~~}";
        const string monsterType = "{type~~~~~~~~}";
        const string monsterDmg = "{dmg~~~~~~~~}";
        const string monsterEnergy = "{energy~~}";
        const string monsterArtField = "{X~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";

        const string fightPlayerDamage = "{player-dmg~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";
        const string fightMonsterDamage = "{monster-dmg~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";



        /// <summary>
        /// Method <c>GetArt></c> Retrieves an art file from the assets folder.
        /// </summary>
        /// <param name="file"></param>
        /// <returns> 
        /// The art file
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the art file is not found.
        /// </exception>
        public static string GetArt(string file, string sub="")
        {
            string path = Game.artDir + sub + file + ".txt";
            Debug.Assert(File.Exists(path)); // check if the art file exists
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }
        }

        /// <summary>
        /// Method <c>PopulateField</c> replaces dummy text in an art file with the supplied parameter.
        /// </summary>
        /// <param name="art">the art file.</param>
        /// <param name="field">the field in the art file to populate.</param>
        /// <param name="para">the data to populate the field.</param>
        /// <returns> 
        /// A new art file 
        /// </returns>
        public static string PopulateField(string art, string field, string para)
        {
            if (para.Length > field.Length)
            {
                throw new ArgumentOutOfRangeException("Parameter must be less than or equal to length of field to occupy.");
            }
            para += new string(' ', field.Length - para.Length);
            return art.Replace(field, para);
        }

        /// <summary>
        /// Method <c>GetDialogue</c> writes dialogue to the console.
        /// </summary>
        /// <param name="header_name">the XElement with name <c>header_name</c> storing the dialogue to be retrieved.</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the dialogue XML file is not found.
        /// </exception>
        public static void GetDialogue(string header_name)
        {
            string txt = "";
            string path = Game.textDir + "dialogues.xml";
            if (File.Exists(path))
            {
                try
                {
                    // TODO: Add further exception handling for case of Element("Text") returning null.
                    txt = XElement.Load(path).Element(header_name).Element("text").Value.Trim();
                }
                catch
                {
                    throw new NullReferenceException("Either dialogue does not exist or is not properly formatted.");
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }

            WriteDialogue(txt);
        }

        /// <summary>
        /// Method <c>StripText</c> splits input text into new lines.
        /// </summary>
        /// <param name="txt"></param>
        /// <returns>
        /// An array of string values, each representing a line of the input text.
        /// </returns>
        public static string[] StripText(string txt)
        {
            // Split the text using either new line characters as delimiters.
            string[] line = txt.Split(new[] { '\r', '\n' });
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = line[i].Trim();
            }
            return line;
        }

        /// <summary>
        /// Method <c>WriteDialogue</c> outputs text with a delay for ease of reading.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="wait"></param>
        public static void WriteDialogue(string txt, int wait = 10, bool clear=false)
        {
            string[] dialogue = StripText(txt);
            int charDelay = 35;

            Console.Write("> "); 
            foreach (string s in dialogue)
            {
                if (s.Length > 1)
                {
                    foreach (char c in s)
                    {
                        //System.Threading.Thread.Sleep(charDelay);
                        Console.Write(c);
                    }
                    //System.Threading.Thread.Sleep(wait);
                }
                Console.WriteLine();
                Console.Write("  ");
            }

            if (clear) { ClearConsole(); }
        }

        public static void GetMainMenu()
        {
            UI.ClearConsole();
            string f1 = "a~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
            string f2 = "b~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
            string f3 = "c~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
            string f4 = "d~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
            string f5 = "e~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";

            string[] arrow = { "  __", " / /", "/ /___________________", @"\ \", @" \_\" };
            string[] nullString = { "", "", "", "", "" };

            string[] ops = { "1", "2", "3" };
            string[] repr;

            string frame;

            int menuSel = 1;
            bool cont = true;
            while (cont)
            {
                frame = UI.GetArt("menu_screen", Game.menuDir);
                foreach (string op in ops)
                {
                    repr = nullString;
                    if (menuSel.ToString() == op)
                    {
                        repr = arrow;
                    }

                    frame = PopulateField(frame, op + f1, repr[0]);
                    frame = PopulateField(frame, op + f2, repr[1]);
                    frame = PopulateField(frame, op + f3, repr[2]);
                    frame = PopulateField(frame, op + f4, repr[3]);
                    frame = PopulateField(frame, op + f5, repr[4]);
                }

                ClearConsole();
                Console.WriteLine(frame);

                char key = Console.ReadKey().KeyChar;
                switch (key)
                {
                    case 'w': if (menuSel != 1) { menuSel -= 1; }; break;
                    case 's': if (menuSel != 3) { menuSel += 1; }; break;
                    case (char)13: cont = false; break;
                }

                if (!cont) { break; }
            }
        }

        public static void GetRoom(Room room)
        {
            string roomArt = GetArt("room", Game.locDir);
            roomArt = PopulateField(roomArt, roomName, room.Name);
            Console.WriteLine(roomArt);
        }

        public static void GetLocation(location loc)
        {
            ClearConsole();
            string locArt = GetArt("location", Game.locDir);
            locArt = PopulateField(locArt, locName, loc.Name);
            locArt = PopulateField(locArt, locDesc, loc.Description);
            locArt = PopulateField(locArt, locDialogue, loc.Dialogue);

            for (int i = 0; i < loc.adjacentLocations.Keys.ToArray().Length; i++)
            {
                locArt = PopulateField(locArt, "{nav" + (i+1).ToString() + locNav, loc.adjacentLocations.Keys.ToArray()[i]);
            }

            GetRoom(loc.ThisRoom);
            Console.WriteLine(locArt);

        }

        public static string GetFight(Monster monster)
        {
            ClearConsole();
            string fightArt = GetArt("monster", Game.monsterDir);
            string[] monsterArt = GetArt(monster.Type, Game.monsterDir).Split('\n');
            fightArt = PopulateField(fightArt, monsterAura, monster.Aura);
            fightArt = PopulateField(fightArt, monsterName, monster.Name);
            fightArt = PopulateField(fightArt, monsterType, monster.Type);
            fightArt = PopulateField(fightArt, monsterDmg, monster.Damage.Value.ToString());

            string field = "{X~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}";
            for (int i=0; i<10; i++)
            {
                fightArt = PopulateField(fightArt, field.Replace("X", (i + 1).ToString()), monsterArt[i]);
            }

            Console.WriteLine(fightArt);
            return fightArt;

        }

        public static void UpdateFight(string fightArt, Monster monster, PlayerManager playerManager, int turn = 0,  int pDmg=0, int mDmg=0, Ability ability=null)
        {
            ClearConsole();
            fightArt = PopulateField(fightArt, monsterEnergy, monster.Energy.Value.ToString());

            string playerDamage = "";
            string monsterDamage = "";
            if(turn==1) // player turn
            {
                playerDamage = $"The player dealt {pDmg} -> {monster.Energy.Value}% damage using {ability.Name}!";
            }
            if(turn==2) // monster turn
            {
                monsterDamage = $"The monster dealt {mDmg} -> {playerManager.player.Energy.Value} damage!";
            }

            fightArt = PopulateField(fightArt, fightPlayerDamage, playerDamage);
            fightArt = PopulateField(fightArt, fightMonsterDamage, monsterDamage);

            Console.WriteLine(fightArt);
        }

        public static void GetPockets(InventorySlot[] items)
        {
            string pockets = GetArt("pockets");
            string nextField = "";
            string nextQuantity = "";
            for (int i  = 0; i < items.Length; i++)
            {
                nextField = itemName.Replace('Z', (char)(i + 65)); // A : 65 ASCII
                nextQuantity= itemQuantity.Replace('Z', (char)(i + 65));
                if (items[i].IsEmpty)
                {
                    pockets = PopulateField(pockets, nextField, "");
                    pockets = PopulateField(pockets, nextQuantity, "");
                }
                else
                {
                    pockets = PopulateField(pockets, nextField, items[i].ItemStack.Item.Name);
                    pockets = PopulateField(pockets, nextQuantity, items[i].ItemStack.Amount.ToString());
                }
            }
            Console.WriteLine(pockets);
        }

        public static void ClearConsole()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J"); // escape character sequence to fix Clear not clearing
                                          // entire console in some terminal applications
        }
    }
}
