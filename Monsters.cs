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
    public abstract class Monster : Creature, IDamageAble
    {
        public Id id {  get; private set; }
        public string Name { get; protected set; }
        /// <value>
        /// Sub-type the monster belongs to.
        /// </value>
        public string Type {  get; protected set; }

        /// <value>
        /// A phrase associated with each monster sub-type signalling its villainy.
        /// </value>
        public string Dialogue {  get; protected set; }

        /// <value>
        /// Equivalent to attack damage: drains the players Energy.
        /// </value>
        public Energy Energy { get; protected set; }

        /// <value>
        /// Equivalent to attack damage: drains the players Energy.
        /// </value>
        public MonsterDamage Damage { get; protected set; }

        /// <value>
        /// Whether the monster has been defeated.
        /// </value>
        public bool Defeated {  get; set; }

        /// <value>
        /// Reference to the <c>PlayerManager</c> instance the monster may interacts with.
        /// </value>
        public PlayerManager PlayerManager { get; protected set; }

        public Monster(Id id, string name, string dialogue, int energy, int damage, PlayerManager playerManager)
        {
            id = id ?? throw new ArgumentNullException(nameof(name), "The id cannot be null.");
            Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null.");
            Dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue), "The dialogue cannot be null.");
            Energy = new Energy(playerManager, energy, energy);
            Damage = new MonsterDamage(playerManager, damage, damage);

            this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(dialogue), "The PlayerManager cannot be null.");
            this.Defeated = false;
        }

        public float Attack()
        {   
            float damage = Damage.Value;
            damage = PlayerManager.player.TakeDamage(Damage.Value);
            return damage;
        }

        public float TakeDamage(float damage)
        {
            Energy.Value -= damage;
            return damage;
        }
    }

    public class InternalMonster : Monster
    {
        public InternalMonster(Id id, string name, string dialogue, int energy, int damage, PlayerManager playerManager) : base(id, name, dialogue, energy, damage, playerManager)
        {
            Type = "internal";
            Dialogue = "Your vision fades and the surroundings become a blur, you shake yourself from head to toe.";
        }
    }

    public class SensoryMonster : Monster
    {
        public SensoryMonster(Id id, string name, string dialogue, int energy, int damage, PlayerManager playerManager) : base(id, name, dialogue, energy, damage, playerManager)
        {
            Type = "sensory";
            Dialogue = "Your nerves tingle as your mind clouds over, clouded by its relentless stabbing.";
        }
    }

    public class SocialMonster : Monster
    {
        public SocialMonster(Id id, string name, string dialogue, int energy, int damage, PlayerManager playerManager) : base(id, name, dialogue, energy, damage, playerManager)
        {
            Type = "social";
            Dialogue = "Countless voices clutter your mind as you find yourself unable to think clearly.";
        }
    }
}
