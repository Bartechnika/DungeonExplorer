using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DungeonExplorer
{
    public class Room
    {
        public string name;
        public string description;
        public List<location> locations;

        public Room(string roomName)
        {   
            this.locations = new List<location>();

            string path = Game.textDir + "rooms.xml";
            if (File.Exists(path))
            {
                XElement room = XElement.Load(path).Element(roomName);
                name = room.Element("name").Value.Trim();
                description = room.Element("description").Value.Trim();

                var locationsList = room.Elements("location");
                foreach (var loc in locationsList)
                {
                    location newLocation = new location(loc.Attribute("name").Value.ToString(), bool.Parse(loc.Attribute("locked").Value.ToString()));
                    locations.Add(newLocation);
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }
        }

        public struct location
        {
            public string Name;
            public bool Locked;

            public location(string name, bool locked)
            {
                this.Name = name;
                this.Locked = locked;
            }
        }

        public string GetDescription()
        {
            return description;
        }
    }
}