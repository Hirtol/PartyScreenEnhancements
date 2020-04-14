using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using PartyScreenEnhancements.Comparers;
using TaleWorlds.Engine;
using Path = System.IO.Path;

namespace PartyScreenEnhancements.Saving
{
    public static class PartyScreenConfig
    {
        internal static Dictionary<string, int> PathsToUpgrade =
            new Dictionary<string, int>();
        internal static Dictionary<string, int> PrisonersToRecruit = new Dictionary<string, int>();
        internal static PartySort Sorter = new TypeComparer(new TrueTierComparer(new AlphabetComparer(null, false), true), false);

        internal const double VERSION = 1.02;

        private static readonly string modDir = Utilities.GetConfigsPath() + "Mods" + Path.DirectorySeparatorChar;
        private static readonly string _filename = modDir + "PartyScreenEnhancements.xml";
        private static readonly string _sorterfile = modDir + "Sorter.xml";
        // Used to reset Sorters to their initial state in case some changes were made.
        private static bool _upgradedVersion = true;

        public static void Initialize()
        {
            Directory.CreateDirectory(modDir);
            
            if (!File.Exists(_filename))
            {
                Save();
            }
            else
            {
                Load();
                if (_upgradedVersion)
                {
                    Save();
                }
            }
            if (!File.Exists(_sorterfile))
            {
                SaveSorter();
            }
            else
            {
                if(!_upgradedVersion)
                {
                    LoadSorter();
                }
                else
                {
                    SaveSorter();
                }
            }
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
                var version = xmlDocument.CreateElement("Version");
                version.InnerText = VERSION.ToString();

                sortingOptions.AppendChild(version);
                modNode.AppendChild(sortingOptions);

                addDictionaryToXML(ref PathsToUpgrade, ref xmlDocument, ref modNode, "UpgradePaths");
                addDictionaryToXML(ref PrisonersToRecruit, ref xmlDocument, ref modNode, nameof(PrisonersToRecruit));

                xmlDocument.Save(_filename);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        private static void addDictionaryToXML(ref Dictionary<string, int> dictionary, ref XmlDocument document, ref XmlElement parent, string name)
        {
            var el = new XElement(name,
                dictionary.Select(kv => new XElement(kv.Key, kv.Value)));

            var element = document.ReadNode(el.CreateReader()) as XmlElement;

            parent.AppendChild(element);
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
                    if (xmlNode.Name == "UpgradePaths")
                    {
                        XElement rootElement = XElement.Parse(xmlNode.OuterXml);
                        PathsToUpgrade = rootElement.Elements()
                            .ToDictionary(key => key.Name.LocalName, val => int.Parse(val.Value));
                    }

                    if (xmlNode.Name == nameof(PrisonersToRecruit))
                    {
                        XElement rootElement = XElement.Parse(xmlNode.OuterXml);
                        PrisonersToRecruit = rootElement.Elements()
                            .ToDictionary(key => key.Name.LocalName, val => int.Parse(val.Value));
                    }

                    if (xmlNode.Name == "Options")
                    {
                        XElement options = XElement.Parse(xmlNode.OuterXml);
                        foreach (XElement element in options.Elements())
                        {
                            if (element.Name == "Version")
                            {
                                if(Double.Parse(element.Value) == VERSION)
                                    _upgradedVersion = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                Save();
            }
        }
    }

    
}