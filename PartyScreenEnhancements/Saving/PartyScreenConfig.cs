using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using Path = System.IO.Path;

namespace PartyScreenEnhancements.Saving
{
    public static class PartyScreenConfig
    {
        [XmlElement]
        internal static Dictionary<string, int> PathsToUpgrade =
            new Dictionary<string, int>();

        private static string _filename;

        public static void Initialize()
        {
            _filename = Utilities.GetConfigsPath() + Path.DirectorySeparatorChar + "PartyScreenEnhancements.xml";
            if (!File.Exists(_filename))
            {
                Save();
            }
            else
            {
                
            }
        }

        public static void Save()
        {

        }

		// public static void Load()
		// {
		// 	if (!File.Exists(HotKeyManager._savePath))
		// 	{
		// 		return;
		// 	}
		// 	try
		// 	{
		// 		XmlDocument xmlDocument = new XmlDocument();
		// 		xmlDocument.Load(HotKeyManager._savePath);
		// 		foreach (object obj in xmlDocument.DocumentElement.ChildNodes)
		// 		{
		// 			XmlNode xmlNode = (XmlNode)obj;
		// 			string attribute = ((XmlElement)xmlNode).GetAttribute("name");
		// 			GameKeyContext gameKeyContext;
		// 			if (HotKeyManager._categories.TryGetValue(attribute, out gameKeyContext))
		// 			{
		// 				foreach (object obj2 in xmlNode.ChildNodes)
		// 				{
		// 					XmlNode xmlNode2 = (XmlNode)obj2;
		// 					string name = ((XmlElement)xmlNode2).Name;
		// 					if (name == "GameKey")
		// 					{
		// 						string innerText = xmlNode2["Id"].InnerText;
		// 						GameKey gameKey = gameKeyContext.GetGameKey(innerText);
		// 						if (gameKey != null)
		// 						{
		// 							XmlElement xmlElement = xmlNode2["Keys"];
		// 							XmlElement xmlElement2 = xmlElement["Key"];
		// 							if (xmlElement2 != null)
		// 							{
		// 								InputKey key = (InputKey)Enum.Parse(typeof(InputKey), xmlElement2.InnerText);
		// 								gameKey.PrimaryKey.ChangeKey(key);
		// 							}
		// 							XmlElement xmlElement3 = xmlElement["ControllerKey"];
		// 							if (xmlElement3 != null)
		// 							{
		// 								InputKey key2 = (InputKey)Enum.Parse(typeof(InputKey), xmlElement3.InnerText);
		// 								if (gameKey.ControllerKey != null)
		// 								{
		// 									gameKey.ControllerKey.ChangeKey(key2);
		// 								}
		// 								else
		// 								{
		// 									gameKey.ControllerKey = new Key(key2);
		// 								}
		// 							}
		// 							else
		// 							{
		// 								gameKey.ControllerKey = null;
		// 							}
		// 						}
		// 					}
		// 					else if (name == "GameAxisKey")
		// 					{
		// 						string innerText2 = xmlNode2["Id"].InnerText;
		// 						GameAxisKey gameAxisKey = gameKeyContext.GetGameAxisKey(innerText2);
		// 						if (gameAxisKey != null)
		// 						{
		// 							XmlElement xmlElement4 = xmlNode2["Keys"];
		// 							if (!gameAxisKey.IsBinded)
		// 							{
		// 								XmlElement xmlElement5 = xmlElement4["PositiveKey"];
		// 								if (xmlElement5 != null)
		// 								{
		// 									if (xmlElement5.InnerText != "None")
		// 									{
		// 										InputKey inputKey = (InputKey)Enum.Parse(typeof(InputKey), xmlElement5.InnerText);
		// 										gameAxisKey.PositiveKey = new GameKey(-1, gameAxisKey.Id + "_p", attribute, inputKey, "");
		// 									}
		// 									else
		// 									{
		// 										gameAxisKey.PositiveKey = null;
		// 									}
		// 								}
		// 								XmlElement xmlElement6 = xmlElement4["NegativeKey"];
		// 								if (xmlElement6 != null)
		// 								{
		// 									if (xmlElement6.InnerText != "None")
		// 									{
		// 										InputKey inputKey2 = (InputKey)Enum.Parse(typeof(InputKey), xmlElement6.InnerText);
		// 										gameAxisKey.NegativeKey = new GameKey(-1, gameAxisKey.Id + "_n", attribute, inputKey2, "");
		// 									}
		// 									else
		// 									{
		// 										gameAxisKey.NegativeKey = null;
		// 									}
		// 								}
		// 							}
		// 							XmlElement xmlElement7 = xmlElement4["AxisKey"];
		// 							if (xmlElement7 != null)
		// 							{
		// 								if (xmlElement7.InnerText != "None")
		// 								{
		// 									InputKey key3 = (InputKey)Enum.Parse(typeof(InputKey), xmlElement7.InnerText);
		// 									gameAxisKey.AxisKey = new Key(key3);
		// 								}
		// 								else
		// 								{
		// 									gameAxisKey.AxisKey = null;
		// 								}
		// 							}
		// 						}
		// 					}
		// 					else if (name == "HotKey")
		// 					{
		// 						string innerText3 = xmlNode2["Id"].InnerText;
		// 						HotKey hotKey = gameKeyContext.GetHotKey(innerText3);
		// 						if (hotKey != null)
		// 						{
		// 							new List<HotKey>();
		// 							XmlElement xmlElement8 = xmlNode2["Keys"];
		// 							hotKey.Keys = new List<Key>();
		// 							for (int i = 0; i < xmlElement8.ChildNodes.Count; i++)
		// 							{
		// 								XmlNode xmlNode3 = xmlElement8.ChildNodes.get_ItemOf(i);
		// 								InputKey key4 = (InputKey)Enum.Parse(typeof(InputKey), xmlNode3.InnerText);
		// 								hotKey.Keys.Add(new Key(key4));
		// 							}
		// 						}
		// 					}
		// 				}
		// 			}
		// 		}
		// 	}
		// 	catch
		// 	{
		// 	}
		// }
		//
		// public static void Save()
		// {
		// 	try
		// 	{
		// 		XmlDocument xmlDocument = new XmlDocument();
		// 		XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
		// 		XmlElement documentElement = xmlDocument.DocumentElement;
		// 		xmlDocument.InsertBefore(xmlDeclaration, documentElement);
		// 		XmlElement xmlElement = xmlDocument.CreateElement("HotKeyCategories");
		// 		xmlDocument.AppendChild(xmlElement);
		// 		foreach (KeyValuePair<string, GameKeyContext> keyValuePair in HotKeyManager._categories)
		// 		{
		// 			if (!HotKeyManager._serializeIgnoredCategories.Contains(keyValuePair.Key))
		// 			{
		// 				XmlElement xmlElement2 = xmlDocument.CreateElement("HotKeyCategory");
		// 				xmlElement.AppendChild(xmlElement2);
		// 				xmlElement2.SetAttribute("name", keyValuePair.Key);
		// 				foreach (GameKey gameKey in keyValuePair.Value.RegisteredGameKeys)
		// 				{
		// 					if (gameKey != null)
		// 					{
		// 						XmlElement xmlElement3 = xmlDocument.CreateElement("GameKey");
		// 						xmlElement2.AppendChild(xmlElement3);
		// 						XmlElement xmlElement4 = xmlDocument.CreateElement("Id");
		// 						xmlElement3.AppendChild(xmlElement4);
		// 						xmlElement4.InnerText = gameKey.StringId;
		// 						XmlElement xmlElement5 = xmlDocument.CreateElement("Keys");
		// 						xmlElement3.AppendChild(xmlElement5);
		// 						XmlElement xmlElement6 = xmlDocument.CreateElement("Key");
		// 						xmlElement5.AppendChild(xmlElement6);
		// 						xmlElement6.InnerText = ((gameKey.PrimaryKey != null) ? gameKey.PrimaryKey.InputKey.ToString() : "None");
		// 						if (gameKey.ControllerKey != null)
		// 						{
		// 							XmlElement xmlElement7 = xmlDocument.CreateElement("ControllerKey");
		// 							xmlElement5.AppendChild(xmlElement7);
		// 							xmlElement7.InnerText = gameKey.ControllerKey.InputKey.ToString();
		// 						}
		// 					}
		// 				}
		// 				foreach (GameAxisKey gameAxisKey in keyValuePair.Value.RegisteredGameAxisKeys)
		// 				{
		// 					XmlElement xmlElement8 = xmlDocument.CreateElement("GameAxisKey");
		// 					xmlElement2.AppendChild(xmlElement8);
		// 					XmlElement xmlElement9 = xmlDocument.CreateElement("Id");
		// 					xmlElement8.AppendChild(xmlElement9);
		// 					xmlElement9.InnerText = gameAxisKey.Id;
		// 					XmlElement xmlElement10 = xmlDocument.CreateElement("Keys");
		// 					xmlElement8.AppendChild(xmlElement10);
		// 					XmlElement xmlElement11 = xmlDocument.CreateElement("PositiveKey");
		// 					xmlElement10.AppendChild(xmlElement11);
		// 					xmlElement11.InnerText = ((gameAxisKey.PositiveKey != null) ? gameAxisKey.PositiveKey.PrimaryKey.InputKey.ToString() : "None");
		// 					XmlElement xmlElement12 = xmlDocument.CreateElement("NegativeKey");
		// 					xmlElement10.AppendChild(xmlElement12);
		// 					xmlElement12.InnerText = ((gameAxisKey.NegativeKey != null) ? gameAxisKey.NegativeKey.PrimaryKey.InputKey.ToString() : "None");
		// 					XmlElement xmlElement13 = xmlDocument.CreateElement("AxisKey");
		// 					xmlElement10.AppendChild(xmlElement13);
		// 					xmlElement13.InnerText = ((gameAxisKey.AxisKey != null) ? gameAxisKey.AxisKey.InputKey.ToString() : "None");
		// 				}
		// 				foreach (HotKey hotKey in keyValuePair.Value.RegisteredHotKeys)
		// 				{
		// 					XmlElement xmlElement14 = xmlDocument.CreateElement("HotKey");
		// 					xmlElement2.AppendChild(xmlElement14);
		// 					XmlElement xmlElement15 = xmlDocument.CreateElement("Id");
		// 					xmlElement14.AppendChild(xmlElement15);
		// 					xmlElement15.InnerText = hotKey.Id;
		// 					XmlElement xmlElement16 = xmlDocument.CreateElement("Keys");
		// 					xmlElement14.AppendChild(xmlElement16);
		// 					foreach (Key key in hotKey.Keys)
		// 					{
		// 						XmlElement xmlElement17 = xmlDocument.CreateElement("Key");
		// 						xmlElement16.AppendChild(xmlElement17);
		// 						xmlElement17.InnerText = key.InputKey.ToString();
		// 					}
		// 				}
		// 			}
		// 		}
		// 		xmlDocument.Save(HotKeyManager._savePath);
		// 	}
		// 	catch
		// 	{
		// 	}
		// }

	}


}