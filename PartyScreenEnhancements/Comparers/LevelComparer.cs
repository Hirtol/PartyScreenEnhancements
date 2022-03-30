using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyScreenEnhancements.Comparers
{
    public class LevelComparer : PartySort
    {
        public LevelComparer(PartySort equalSorter, bool descending) : base(equalSorter, @descending, null)
        {
        }

        public LevelComparer()
        {
        }

        public override string GetHintText()
        {
            return "Compares units based on their Level.\nAscending order is low to high.\nDescending order is high to low.";
        }

        public override string GetName()
        {
            return "Level Comparer";
        }

        public override bool HasCustomSettings()
        {
            return false;
        }

        protected override int localCompare(ref PartyCharacterVM x, ref PartyCharacterVM y)
        {
            if (Descending ? x.Character.Level > y.Character.Level : y.Character.Level > x.Character.Level) return -1;

            if (x.Character.Level == y.Character.Level)
                return EqualSorter?.Compare(x, y) ?? 0;

            return 1;
        }
    }
}
