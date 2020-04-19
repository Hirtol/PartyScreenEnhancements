using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PartyScreenEnhancements.Comparers;

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

        [XmlElement("SeparateSortingProfiles")]
        public bool SeparateSortingProfiles { get; set; } = true;

        [XmlElement("AutomaticSorting")]
        public bool AutomaticSorting { get; set; } = false;

        [XmlElement("PartySorter")]
        public PartySort PartySorter { get; set; } = PartyScreenConfig.Sorter;

        [XmlElement("PrisonerSorter")]
        public PartySort PrisonerSorter { get; set; } = PartyScreenConfig.Sorter;

        [XmlElement("GarrisonSorter")]
        public PartySort GarrisonSorter { get; set; } = PartyScreenConfig.Sorter;





        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
