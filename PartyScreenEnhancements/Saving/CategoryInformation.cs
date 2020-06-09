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

        [XmlElement("SelectedFormation")] public int SelectedFormation { get; set; } = 0;

        [XmlElement("CurrentMainListIndex")] public int CurrentIndexInMainList { get; set; } = -1;

        [XmlElement("IsCollapsed")] public bool IsCollapsed { get; set; } = false;

        public CategoryInformation()
        {
        }

        public CategoryInformation(string name, int initialFormation = 1, int initialIndex = -1)
        {
            this.Name = name;
            this.SelectedFormation = initialFormation;
            this.CurrentIndexInMainList = initialIndex;
        }
    }
}
