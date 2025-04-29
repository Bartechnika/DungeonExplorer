using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DungeonExplorer
{
    internal static class CombatManager
    {
        public static Breathe breathe = new Breathe();
        public static Distract distract = new Distract();
        public static Reassure reassure = new Reassure();
        public static Express express = new Express();
        public static Ground ground = new Ground();

        public static Random rnd = new Random();

        public static Dictionary<string, Dictionary<string, int>> abilityDamageMatrix = new Dictionary<string, Dictionary<string, int>>()
        {
            // Effectiveness against Internal / Sensory / Social
            {"breathe", new Dictionary<string, int>() { { "internal", 5 }, { "sensory", 5 }, { "social", 15 } } },
            {"distract", new Dictionary<string, int>() { { "internal", 5 }, { "sensory", 15 }, { "social", 10 } } },
            {"reassure", new Dictionary<string, int>() { { "internal", 10 }, { "sensory", 5 }, { "social", 10 } } },
            {"express", new Dictionary<string, int>() { { "internal", 5 }, { "sensory", 15 }, { "social", 10 } } },
            {"ground", new Dictionary<string, int>() { { "internal", 5 }, { "sensory", 5 }, { "social", 10 } } }
        };

        public static Dictionary<string, Ability> abilitySelectionMatrix = new Dictionary<string, Ability>()
        {
            {"1", breathe},
            {"2", distract},
            {"3", reassure},
            {"4", express},
            {"5", ground}
        };

    }

    public abstract class Ability
    {
        public abstract string Name { get; set; }
        public abstract void Use();
    }


    public class Breathe : Ability
    {
        public override string Name { get; set; } = "breathe";
        public override void Use()
        {
            string breath = UI.GetArt("breathe", Game.abilitiesDir);
            UI.WriteDialogue(breath);
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("\nIn");
                for (int j = 0; j < 4; j++)
                {
                    Console.Write(".");
                    //Game.Wait(1);
                }

                Console.WriteLine("\nHold");
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(".");
                    //Game.Wait(1);
                }

                Console.WriteLine("\nOut");
                for (int j = 0; j < 4; j++)
                {
                    Console.Write(".");
                    //Game.Wait(1);
                }
            }
        }
    }

    public class Distract : Ability
    {
        public override string Name { get; set; } = "breathe";
        public override void Use() 
        {
            bool correct = true;
            int op = CombatManager.rnd.Next(1, 4);
            switch (op)
            {
                case 1: { GetPicture(); break; }
                case 2: { GetQuestion(); break; }
                case 3: { correct = GetProblem(); break; }
            }
        }

        public void GetPicture()
        {
            Console.WriteLine("♪┏(・o･)┛♪┗ ( ･o･) ┓");
        }

        public void GetQuestion()
        {
            string[] questions = UI.GetArt("distract", Game.abilitiesDir).Split('\n');
            int op = CombatManager.rnd.Next(0, questions.Length);
            Console.WriteLine(questions[op]);
            Console.ReadLine();
            Game.Wait(1);
            Console.WriteLine("Agreed (＾ｖ＾) - that is very interesting!");

        }

        public bool GetProblem()
        {
            bool correct = true;
            int x1 = CombatManager.rnd.Next(1, 50);
            int x2 = CombatManager.rnd.Next(1, 50);
            int op = CombatManager.rnd.Next(1, 4);
            string correctAnswer = "";
            char sign = ' ';

            switch (op)
            {
                case 1: { correctAnswer = (x1 + x2).ToString(); sign = '+'; break; }
                case 2: { correctAnswer = (x1 - x2).ToString(); sign = '-'; break; }
                case 3: { correctAnswer = (x1 * x2).ToString(); sign = 'x';  break; }
            }
            Console.WriteLine($"\nWhat is {x1} {sign} {x2}?");
            string answer = Console.ReadLine();
            if(answer == correctAnswer)
            {
                correct = true;
                Console.WriteLine("You got it!");
            }
            else
            {
                Console.WriteLine("Not exactly, but good try!");
            }

            return correct;
        }
    }
    public class Reassure : Ability
    {
        public override string Name { get; set; } = "reassure";
        public override void Use() 
        {
            Console.WriteLine("Repeat after me...");
            string[] phrases = UI.GetArt("reassure", Game.abilitiesDir).Split('\n');

            int op;
            string phrase;
            string input = "";
            for(int i = 0; i < 3; i++)
            {
                op = CombatManager.rnd.Next(0, phrases.Length);
                phrase = phrases[op].Trim();
                Console.WriteLine($"\n{phrase}");

                while(input != phrase)
                {
                    input = Console.ReadLine().Trim();
                    if(input != phrase)
                    {
                        Console.WriteLine("You can do it! Try again...");
                    }
                }
            }
        }
    }
    public class Express : Ability
    {
        public override string Name { get; set; } = "express";
        public override void Use() 
        {
            Console.WriteLine("How are you feeling? Write as much or as little as you want");
            Console.ReadLine();
        }
    }
    public class Ground : Ability
    {
        public override string Name { get; set; } = "ground";
        public override void Use() 
        { 
            Console.WriteLine("Feel the ground beneath your feet");
        }
    }
}
