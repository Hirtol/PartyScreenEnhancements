using System.Collections.Generic;
using System.Xml.Serialization;
using PartyScreenEnhancements.Saving;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace PartyScreenEnhancements.Comparers
{
    /// <summary>
    /// This is the base class for all Sorters, it takes care of verifying that the to be compared units aren't heroes/companions.
    /// The entire sorting structure is essentially a linked list, where the next link is used when a sorter encounters equality
    /// </summary>
    [XmlInclude(typeof(TrueTierComparer))]
    [XmlInclude(typeof(TypeComparer))]
    [XmlInclude(typeof(AlphabetComparer))]
    [XmlInclude(typeof(BasicTypeComparer))]
    [XmlInclude(typeof(LevelComparer))]
    [XmlInclude(typeof(CultureComparer))]
    [XmlInclude(typeof(NumberComparer))]
    [XmlInclude(typeof(UpgradeableComparer))]
    [XmlInclude(typeof(WoundedComparer))]
    public abstract class PartySort : IComparer<PartyCharacterVM>
    {
        [XmlElement("Descending")]
        public bool Descending { get; set; }

        [XmlElement("SecondarySort", IsNullable = false)]
        public PartySort EqualSorter { get; set; }

        [XmlElement("SortingOrder", IsNullable = false)]
        public List<string> CustomSettingsList { get; set; }

        protected PartySort(PartySort equalSorter, bool descending, List<string> customSort)
        {
            EqualSorter = equalSorter;
            Descending = descending;
            if (customSort != null)
            {
                CustomSettingsList = customSort;
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
            // Potentially a duplicate check, as we should already sort the player character to the top in SortAllTroopsVM
            // But since this is so critical, better safe than sorry.
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
                return PartyScreenConfig.ExtraSettings.KeepHeroesOnTop ? -1 : 1;
            } 
            if (y.IsHero && !x.IsHero)
            {
                return PartyScreenConfig.ExtraSettings.KeepHeroesOnTop ? 1 : -1;
            }
            if (x.IsHero && y.IsHero)
            {
                return 0;
            }

            if(HasCustomSettings() && (CustomSettingsList == null || CustomSettingsList.IsEmpty())) 
                FillCustomList();

            return localCompare(ref x, ref y);
        }

        protected abstract int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y);
    }
}