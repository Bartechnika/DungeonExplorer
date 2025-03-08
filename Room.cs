using System;
using System.Data;
using System.IO;
using System.Xml;

namespace DungeonExplorer
{
    public class Room
    {
        public string name;
        public string description;

        public Room(string roomName)
        {
            string path = Game.textDir + "rooms.xml";
            if (File.Exists(path))
            {
                XmlTextReader reader = new XmlTextReader(path);
                while (reader.Read())
                {
                    if (reader.GetAttribute("name") == roomName)
                    {
                        var innerReader = reader.ReadSubtree();
                        while (innerReader.Read())
                        {
                            switch (innerReader.Name)
                            {
                                case "name":
                                    this.name = innerReader.ReadInnerXml().Trim();
                                    break;
                                case "description":
                                    this.description = innerReader.ReadInnerXml().Trim();
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }
        }

        public string GetDescription()
        {
            return description;
        }
    }
}