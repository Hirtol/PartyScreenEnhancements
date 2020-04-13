using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PartyScreenEnhancements.Comparers;
using TaleWorlds.Engine;
using Path = System.IO.Path;

namespace PartyScreenEnhancements.Saving
{
    public static class PartyScreenConfig
    {
        internal static Dictionary<string, int> PathsToUpgrade =
            new Dictionary<string, int>();
        internal static PartySort Sorter = new TypeComparer(new TrueTierComparer(null, true), false);

        private static readonly string modDir = Utilities.GetConfigsPath() + "Mods" + Path.DirectorySeparatorChar;
        private static readonly string _filename = modDir + "PartyScreenEnhancements.xml";
        private static readonly string _sorterfile = modDir + "Sorter.xml";

        public static void Initialize()
        {
            Directory.CreateDirectory(modDir);
            if (!File.Exists(_sorterfile))
            {
                SaveSorter();
            }
            else
            {
                LoadSorter();
            }
            if (!File.Exists(_filename))
                Save();
            else
                Load();
        }

        public static void SaveSorter()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            var test = new XmlSerializer(typeof(PartySort));

            ns.Add("", "");

            StreamWriter sw = new StreamWriter(_sorterfile);
            test.Serialize(sw, Sorter, ns);
            sw.Close();
        }

        public static void LoadSorter()
        {
            var test = new XmlSerializer(typeof(PartySort));
            StreamReader sw = new StreamReader(_sorterfile);
            Sorter = test.Deserialize(sw) as PartySort;
        }

        public static void Save()
        {
            try
            {
                var xmlDocument = new XmlDocument();

                XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);
                XmlElement modNode = xmlDocument.CreateElement("PartyScreenConfig");
                xmlDocument.AppendChild(modNode);

                var sortingOptions = xmlDocument.CreateElement("Options");

                modNode.AppendChild(sortingOptions);

                var el = new XElement("UpgradePaths",
                    PathsToUpgrade.Select(kv => new XElement(kv.Key, kv.Value)));

                var element = xmlDocument.ReadNode(el.CreateReader()) as XmlElement;

                modNode.AppendChild(element);

                xmlDocument.Save(_filename);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        //TODO: To come back to
        // var sorterElement = xmlDocument.CreateElement("Sorter");
        //
        // StreamWriter sw = new StreamWriter(_sorterfile);
        // test.Serialize(sw, Sorter, ns);
        // sw.Close();
        //
        //
        // sorterElement.InnerXml = Convert.ToString(sw);
        // sortingOptions.AppendChild(sorterElement);

        public static void Load()
        {
            if (!File.Exists(_filename)) return;

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(_filename);

                foreach (object obj in xmlDocument.DocumentElement.ChildNodes)
                {
                    var xmlNode = (XmlNode) obj;
                    Trace.WriteLine("Node name " + xmlNode.Name);
                    if (xmlNode.Name == "UpgradePaths")
                    {
                        XElement rootElement = XElement.Parse(xmlNode.OuterXml);
                        PathsToUpgrade = rootElement.Elements()
                            .ToDictionary(key => key.Name.LocalName, val => int.Parse(val.Value));
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
    }

    
}