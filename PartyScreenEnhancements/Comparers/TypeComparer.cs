using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class TypeComparer : PartySort
    {
        public TypeComparer(PartySort equalSorter, bool descending) : base(equalSorter, descending, null)
        {
        }

        internal TypeComparer()
        {
        }

        public override string GetHintText()
        {
            return
                "Compares units based on their Formation Class.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Formation Type Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending
                ? x.Character.GetFormationClass(PartyBase.MainParty) < y.Character.GetFormationClass(PartyBase.MainParty)
                : x.Character.GetFormationClass(PartyBase.MainParty) > y.Character.GetFormationClass(PartyBase.MainParty))
                return 1;

            if (y.Character.GetFormationClass(PartyBase.MainParty) == x.Character.GetFormationClass(PartyBase.MainParty))
            {
                if (EqualSorter != null)
                    return EqualSorter.Compare(x, y);
                return 0;
            }

            return -1;
        }
    }
}