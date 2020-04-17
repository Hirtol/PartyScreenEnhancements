using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PartyScreenEnhancements.Saving
{
    public class ExtraSettings
    {

        public static List<object> GeneralSettings { get; set; } = new List<object>();

        [XmlElement("GeneralLog")] public bool ShowGeneralLogMessage { get; set; } = true;

        [XmlElement("RecruitByDefault")] public bool RecruitByDefault { get; set; } = false;

        [XmlElement("CategoryNumbers")] public bool DisplayCategoryNumbers { get; set; } = false;

    }
}
