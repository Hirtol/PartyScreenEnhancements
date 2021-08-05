using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using PartyScreenEnhancements.Comparers;

namespace PartyScreenEnhancements.Saving
{
    /// <summary>
    ///     Settings class used for any 'small' settings such as simple booleans
    ///     Instantiated and used primarily by <see cref="PartyScreenConfig" />
    /// </summary>
    public class ExtraSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _displayCategory;
        private bool _separateSorting = true;
        private bool _shouldShowCompletePartyNumber;

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

        [XmlElement("HalfHalfUpgrades")] public bool EqualUpgrades { get; set; }

        [XmlElement("SeparateSortingProfiles")]
        public bool SeparateSortingProfiles
        {
            get => _separateSorting;
            set
            {
                _separateSorting = value;
                OnPropertyChanged();
            }
        }

        [XmlElement("AutomaticSorting")] public bool AutomaticSorting { get; set; }

        [XmlElement("KeepHeroesOnTop")] public bool KeepHeroesOnTop { get; set; } = true;

        [XmlElement("ShouldShowCompletePartyNumber")]
        public bool ShouldShowCompletePartyNumber
        {
            get => _shouldShowCompletePartyNumber;
            set
            {
                _shouldShowCompletePartyNumber = value;
                OnPropertyChanged();
            }
        }

        [XmlElement("UpgradeTooltips")] public bool PathSelectTooltips { get; set; } = true;

        [XmlElement("PartySorter")] public PartySort PartySorter { get; set; } = PartyScreenConfig.DefaultSorter;

        [XmlElement("PrisonerSorter")] public PartySort PrisonerSorter { get; set; } = PartyScreenConfig.DefaultSorter;

        [XmlElement("GarrisonSorter")]
        public PartySort GarrisonAndAlliedPartySorter { get; set; } = PartyScreenConfig.DefaultSorter;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}