using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    public class Testing
    {
        public Game ThisGame;
        public PlayerManager ThisPlayerManager;
        public GameMap ThisRoomManager;
        public Testing()
        {
            ThisGame = new Game();
            ThisPlayerManager = new PlayerManager();
            ThisRoomManager = new GameMap(ThisPlayerManager);
        }
        private void PlayerAttribute_CapsValue()
        {
            // Arrange
            int expected = 0;

            // Act
            ThisPlayerManager.player.Energy.Value = -500;
            int actual = ThisPlayerManager.player.Energy.Value;

            Debug.Assert(expected == actual, "The player attribute function is not capping values correctly.");
        }

        private void PlayerManager_CapsInventoryItems()
        {
            // Arrange
            int expected = 4;

            /*
            ThisPlayerManager.PickupItem("pockets", "#001", 1);
            ThisPlayerManager.PickupItem("pockets", "#001", 1);
            ThisPlayerManager.PickupItem("pockets", "#001", 1);
            */

            //Debug.Assert(ThisPlayerManager.NextEmptyPocket == expected, "The NextEmptyPocket pointer is assigned incorrectly.");
        }

        public void UnitTest_1()
        {
            PlayerAttribute_CapsValue();
            PlayerManager_CapsInventoryItems();
        }
    }
}
