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
        public static Random rnd = new Random();

        public static Dictionary<string, Dictionary<string, int>> abilityDamageMatrix = new Dictionary<string, Dictionary<string, int>>()
        {
            // Effectiveness against Internal / Sensory / Social
            {"breathe", new Dictionary<string, int>() { { "internal", 1 }, { "sensory", 1 }, { "social", 2 } } },
            {"distract", new Dictionary<string, int>() { { "internal", 1 }, { "sensory", 3 }, { "social", 2 } } },
            {"reassure", new Dictionary<string, int>() { { "internal", 2 }, { "sensory", 1 }, { "social", 1 } } },
            {"express", new Dictionary<string, int>() { { "internal", 1 }, { "sensory", 3 }, { "social", 2 } } },
            {"ground", new Dictionary<string, int>() { { "internal", 1 }, { "sensory", 1 }, { "social", 2 } } }
        };

    }
}
