using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{   
    public class Program
    {
        private const bool test = true;
        static void Main(string[] args)
        {   
            if(test)
            {
                Testing testing = new Testing();
                testing.UnitTest_1();
            }
            else
            {
                Game game = new Game();
                game.Start();
                Console.WriteLine("Waiting for your Implementation");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
