using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using HarmonyLib;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    //[PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']")]
    public class CustomPartyScreenWidgetPatch : CustomPatch<XmlDocument>
    {
        public override void Apply(XmlDocument xmldoc)
        { 
            //TODO: FIX
            XmlNode windowNode = xmldoc.SelectSingleNode("Prefab/Window");

            FileLog.Log($"WINDOWNODE {windowNode != null} ");

            XmlNode NodeToCopy = windowNode.FirstChild;

            FileLog.Log($"Node name {NodeToCopy.Name}");

            // Create a new node with the name of your new server
            XmlElement newNode = xmldoc.CreateElement("CustomPartyScreenWidget");
            foreach (XmlAttribute attribute in NodeToCopy.Attributes)
            {
                newNode.SetAttribute(attribute.LocalName, attribute.Value);
            }

            //XElement newNodes = new XElement("CustomPartyScreenWidget");
            

            // FileLog.Log($"Name: {newNodes.Name} with attirubtes: ");
            // newNodes.Attributes().ToList().ForEach(it => FileLog.Log($"Att: {it.Name}"));
            //
            // XmlNode newNode = newNodes.ToXmlNode();

            // set the inner xml of a new node to inner xml of original node
            newNode.InnerXml = NodeToCopy.InnerXml;

            // append new node to DocumentElement, not XmlDocument
            windowNode.ReplaceChild(newNode, NodeToCopy);
            //windowNode.RemoveChild(NodeToCopy);
            //windowNode.AppendChild(newNode);
        }
    }

    public static class MyExtensions
    {
        public static XElement ToXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        public static XmlNode ToXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
    }
}
