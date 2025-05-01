using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DungeonExplorer
{
    /// <summary>
    /// Class <c>CreatureAttribute</c> represents an attribute of the player.
    /// </summary>

    public interface ILevelable
    {
        float Level(float xp);
    }

    public abstract class CreatureAttribute
    {
        public PlayerManager playerManager;
        public string Name { get; private set; }

        public float MaxValue { get; private set; }
        public float StartValue { get; private set; }
        public float BaseValue { get; set; }
        private float _value;

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                if (value < 0)
                {
                    _value = 0;
                }
                if (value > MaxValue)
                {
                    _value = MaxValue;
                }
            }
        }

        public CreatureAttribute(PlayerManager playerManager, float maxValue, float startValue)
        {
            MaxValue = maxValue;
            StartValue = startValue;
            BaseValue = startValue;
            Value = startValue;
        }
    }

    public class Energy : CreatureAttribute
    {
        public Energy(PlayerManager playerManager, float maxValue, float startValue) : base(playerManager, maxValue, startValue)
        {

        }
    }


    public class Resilience : CreatureAttribute, ILevelable
    {
        public Resilience(PlayerManager playerManager, float maxValue, float startValue) : base(playerManager, maxValue, startValue)
        {

        }
        public float Level(float xp)
        {
            // At level 1, we need to defeat a 10 energy 10 damage boss to gain 1 level
            float x1 = xp / 10;
            float x2 = ((float)-Value / 250f) + (1f / 2f);
            float x3 = x1 * x2;
            BaseValue += x3;

            return playerManager.player.Resilience.Value;
        }
    }

    public class Love : CreatureAttribute, ILevelable
    {
        public Love(PlayerManager playerManager, float maxValue, float startValue) : base(playerManager, maxValue, startValue)
        {

        }

        public float Level(float xp)
        {
            float x1 = xp / 10;
            float x2 = ((float)-Value / 125f) + (1f / 2f);
            float x3 = x1 * x2;
            Value += x3;

            return Value;
        }
    }

    public class Imagination : CreatureAttribute, ILevelable
    {
        public Imagination(PlayerManager playerManager, float maxValue, float startValue) : base(playerManager, maxValue, startValue)
        {

        }
        public float Level(float xp)
        {
            float x1 = xp / 10;
            float x2 = ((float)-Value / 125f) + (1f / 2f);
            float x3 = x1 * x2;
            Value += x3;

            return Value;
        }
    }

    public class MonsterDamage : CreatureAttribute
    {
        public MonsterDamage(PlayerManager playerManager, float maxValue, float startValue) : base(playerManager, maxValue, startValue)
        {

        }
    }
}
