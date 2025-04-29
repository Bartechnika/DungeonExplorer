using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer;

namespace DungeonExplorer
{   
    /// <summary>
    /// Abstract class <c>Monster</c> used as a base class for other Monsters.
    /// </summary>
    public abstract class Monster : Creature
    {
        /// <value>
        /// Sub-type the monster belongs to.
        /// </value>
        public string Type {  get; protected set; }

        /// <value>
        /// A phrase associated with each monster sub-type signalling its villainy.
        /// </value>
        public string Aura {  get; protected set; }

        /// <value>
        /// Any dialogue spoken around or providing context to the monster.
        /// </value>
        public string Dialogue {  get; protected set; }

        /// <value>
        /// Equivalent to attack damage: drains the players Energy.
        /// </value>
        public CreatureAttribute Damage { get; protected set; } = new CreatureAttribute("Damage");

        /// <value>
        /// Whether the monster has been defeated.
        /// </value>
        public bool Defeated {  get; protected set; }

        /// <value>
        /// Reference to the <c>PlayerManager</c> instance the monster may interacts with.
        /// </value>
        public PlayerManager PlayerManager;

        public Monster(string name, string dialogue, int damage, PlayerManager playerManager)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null.");
            this.Dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue), "The dialogue cannot be null.");
            this.Damage.Value = damage;
            this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(dialogue), "The PlayerManager cannot be null.");
            this.Defeated = false;
            this.Energy.Value = 100;

            Damage.Value = damage;
        }

        public int Attack()
        {   
            int damage = Damage.Value;
            damage = PlayerManager.TakeDamage(Damage.Value);
            return damage;
        }

        public void TakeDamage(int damage)
        {
            Energy.Value -= damage;
        }
    }

    public class SocialMonster : Monster
    {   
        public SocialMonster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
        {
            Type = "social";
            Aura = "Countless voices clutter your mind as you find yourself unable to think clearly.";
        }
    }

    public class SensoryMonster : Monster
    {
        public SensoryMonster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
        {
            Type = "sensory";
            Aura = "Your nerves tingle as your mind clouds over, clouded by its relentless stabbing.";
        }
    }

    public class InternalMonster : Monster
    {
        public InternalMonster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
        {
            Type = "internal";
            Aura = "Your vision fades and the surroundings become a blur, you shake yourself from head to toe.";
        }
    }
}
