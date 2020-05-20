using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PartyScreenEnhancements.Saving
{
    public class CategoryInformation
    {

        [XmlElement("Name")] public string Name { get; set; }

        [XmlElement("SelectedFormation")] public int SelectedFormation { get; set; } = 1;

        [XmlElement("CurrentMainListIndex")] public int CurrentIndexInMainList { get; set; } = -1;

        public CategoryInformation()
        {
        }
    }
}
