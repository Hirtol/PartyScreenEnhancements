using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace PartyScreenEnhancements.Comparers
{
    [XmlInclude(typeof(TrueTierComparer))]
    [XmlInclude(typeof(TypeComparer))]
    [XmlInclude(typeof(AlphabetComparer))]
    [XmlInclude(typeof(BasicTypeComparer))]
    [XmlInclude(typeof(LevelComparer))]
    [XmlInclude(typeof(CultureComparer))]
    [XmlInclude(typeof(NumberComparer))]
    [XmlInclude(typeof(UpgradeableComparer))]
    public abstract class PartySort : IComparer<PartyCharacterVM>
    {

        private List<string> _customSettings;

        [XmlElement("Descending")]
        public bool Descending
        {
            get;
            set;
        }
        [XmlElement("SecondarySort", IsNullable = false)]
        public PartySort EqualSorter
        {
            get;
            set;
        }

        [XmlElement("SortingOrder", IsNullable = false)]
        public List<string> CustomSettingsList {
            get
            {
                return _customSettings;
            }
            set => _customSettings = value;
        }

        protected PartySort(PartySort equalSorter, bool descending, List<string> customSort)
        {
            EqualSorter = equalSorter;
            Descending = descending;
            if (customSort != null)
            {
                _customSettings = customSort;
            }
        }

        internal PartySort()
        {

        }

        public abstract string GetHintText();

        public abstract string GetName();

        public abstract bool HasCustomSettings();

        public virtual void FillCustomList()
        {
            CustomSettingsList = new List<string>();
        }

        public int Compare(PartyCharacterVM x, PartyCharacterVM y)
        {
            if (x.Character.IsPlayerCharacter)
            {
                return -1;
            }
            if (y.Character.IsPlayerCharacter)
            {
                return 1;
            }
            if (x.IsHero && !y.IsHero)
            {
                return -1;
            } 
            if (y.IsHero && !x.IsHero)
            {
                return 1;
            }
            if (x.IsHero && y.IsHero)
            {
                return StringComparer.CurrentCulture.Compare(x.Name, y.Name);
            }

            if(HasCustomSettings() && (CustomSettingsList == null || CustomSettingsList.IsEmpty())) 
                FillCustomList();

            return localCompare(ref x, ref y);
        }

        protected abstract int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y);
    }
}