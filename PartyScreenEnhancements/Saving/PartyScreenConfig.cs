using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using Path = System.IO.Path;

namespace PartyScreenEnhancements.Saving
{
    public static class PartyScreenConfig
    {
        internal static Dictionary<string, int> PathsToUpgrade =
            new Dictionary<string, int>();

        private static string _filename = Utilities.GetConfigsPath() + "PartyScreenEnhancements.xml";

        public static void Initialize()
        {
            if (!File.Exists(_filename))
            {
                Save();
            }
            else
            {
                Load();
            }
        }

        public static void Save()
        {
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);
				XmlElement modNode = xmlDocument.CreateElement("PartyScreenConfig");
                xmlDocument.AppendChild(modNode);
                XElement el = new XElement("UpgradePaths",
					PathsToUpgrade.Select(kv => new XElement(kv.Key, kv.Value)));

				XmlElement element = xmlDocument.ReadNode(el.CreateReader()) as XmlElement;

                modNode.AppendChild(element);

                xmlDocument.Save(_filename);
			}
			catch(Exception e)
			{
				Trace.WriteLine(e.ToString());
            }
		}

        public static void Load()
        {
            if (!File.Exists(_filename))
            {
                return;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(_filename);

                foreach (object obj in xmlDocument.DocumentElement.ChildNodes)
                {
                    XmlNode xmlNode = (XmlNode) obj;
                    if (xmlNode.Name == "UpgradePaths")
                    {
                        XElement rootElement = XElement.Parse(xmlNode.OuterXml);
                        PathsToUpgrade = rootElement.Elements()
                            .ToDictionary(key => key.Name.LocalName, val => Int32.Parse(val.Value));
					}
                    else
                    {
						continue;
                    }
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
			}
        }
    }


}