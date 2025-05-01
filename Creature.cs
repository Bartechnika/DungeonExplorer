using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public interface IDamageAble
    {
        float TakeDamage(float damage);
    }

    public abstract class Creature
    {
        /// <value>
        /// Property <c>Name</c> is a short identifier for the creature.
        /// </value>
        public string Name;

        /// <value>
        /// Reference to the <c>PlayerManager</c> instance the monster may interacts with.
        /// </value>
        public PlayerManager playerManager;

        public Creature()
        {

        }
    }
}
