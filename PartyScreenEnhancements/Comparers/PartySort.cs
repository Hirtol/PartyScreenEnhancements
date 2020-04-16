using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

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

        protected PartySort(PartySort equalSorter, bool descending)
        {
            EqualSorter = equalSorter;
            Descending = descending;
        }

        internal PartySort()
        {

        }

        public abstract string GetHintText();

        public abstract string GetName();

        public int Compare(PartyCharacterVM x, PartyCharacterVM y)
        {
            if (x.Character.IsPlayerCharacter)
            {
                return -1;
            }
            else if (y.Character.IsPlayerCharacter)
            {
                return 1;
            }
            if (x.IsHero && !y.IsHero)
            {
                return -1;
            } 
            else if (y.IsHero && !x.IsHero)
            {
                return 1;
            }
            else if (x.IsHero && y.IsHero)
            {
                return 0;
            }
            return localCompare(ref x, ref y);
        }

        protected abstract int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y);
    }
}