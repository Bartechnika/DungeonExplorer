using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer
{
    /// <summary>
    /// Class <c>Id</c> is a base class of differnet ids.
    /// </summary>
    public class Id 
    {
        /// <value>
        /// Property <c>Id</c> has the format #XXX where X is an integer value from 0-9.
        /// </value>
        string val;
        public string Val
        {
            get { return val; }
            set
            {
                if (value.Length > 4)
                {
                    throw new ArgumentException("Id must be 4 characters long.");
                }
                if (value[0] != '#')
                {
                    throw new ArgumentException("Id must begin with a hashtag # symbol");
                }
                if (!(Char.IsDigit(value[1]) && Char.IsDigit(value[2]) && Char.IsDigit(value[3])))
                {
                    throw new ArgumentException("All entries in Id must be digits 0-9");
                }
                val = value;
            }
        }

        public Id(string value) { Val = value; }
    }
}