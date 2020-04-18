using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PartyScreenEnhancements.Saving
{
    public class ExtraSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _displayCategory = false;

        public static List<object> GeneralSettings { get; set; } = new List<object>();

        [XmlElement("GeneralLog")] public bool ShowGeneralLogMessage { get; set; } = true;

        [XmlElement("RecruitByDefault")] public bool RecruitByDefault { get; set; } = true;

        [XmlElement("CategoryNumbers")]
        public bool DisplayCategoryNumbers
        {
            get => _displayCategory;
            set
            {
                _displayCategory = value;
                OnPropertyChanged();
            }
        }
        [XmlElement("HalfHalfUpgrades")]
        public bool HalfHalfUpgrades { get; set; } = false;

        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
