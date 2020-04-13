using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    [XmlInclude(typeof(TrueTierComparer))]
    [XmlInclude(typeof(TypeComparer))]
    public abstract class PartySort : IComparer<CharacterObject>
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

        protected PartySort(bool descending, PartySort equalSorter = null)
        {
            EqualSorter = equalSorter;
            Descending = descending;
        }

        internal PartySort()
        {

        }

        public int Compare(CharacterObject x, CharacterObject y)
        {
            if (x.IsPlayerCharacter)
            {
                return -1;
            }
            else if (y.IsPlayerCharacter)
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
            return localCompare(x, y);
        }

        protected abstract int localCompare(CharacterObject x, CharacterObject y);
    }
}