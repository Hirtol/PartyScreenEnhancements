namespace PartyScreenEnhancements.ViewModel.Settings.Sorting
{
    public enum SettingSide
    {
        LEFT,
        RIGHT
    }

    public static class EnumExtensions
    {
        public static SettingSide GetOtherSide(this SettingSide side)
        {
            switch (side)
            {
                case SettingSide.LEFT:
                    return SettingSide.RIGHT;
                case SettingSide.RIGHT:
                    return SettingSide.LEFT;
                default:
                    return SettingSide.LEFT;
            }
        }
    }
}