using System.Collections.Generic;
using PartyScreenEnhancements.Comparers;
using PartyScreenEnhancements.Saving;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace PartyScreenEnhancements
{
    /// <summary>
    ///     The ButtonWidget for sorting troops based on a given Comparer.
    ///     This does not follow the MVVM pattern in the slightest, primarily because I wanted to avoid transpiling a custom VM
    ///     in place of the normal PartyVM
    ///     If I run into a method to do this in a compatible way I'll happily switch to that.
    /// </summary>
    public class SortAllTroopsWidget : ButtonWidget
    {
        private readonly MBBindingList<PartyCharacterVM> _mainPartyList;
        private readonly PartyScreenLogic _partyLogic;
        private readonly PartySort _partySorter;
        private readonly PartyVM _partyVM;

        public SortAllTroopsWidget(UIContext context) : base(context)
        {
            if (ScreenManager.TopScreen is GauntletPartyScreen)
            {
                _partyVM = (PartyVM) ((GauntletPartyScreen) ScreenManager.TopScreen).GetField("_dataSource");
                _partyLogic = (PartyScreenLogic) _partyVM.GetField("_partyScreenLogic");
                _mainPartyList = _partyVM.MainPartyTroops;
            }

            _partySorter = new TypeComparer(new TrueTierComparer(null, true), false);
            EventFire += EventHandler;
        }

        /**
         * Instead of replacing PartyVM for inserting a HintVM as a DataSource,
         * I opted to go for adding the hint functionality in the Widget itself.
         */
        private void EventHandler(Widget widget, string eventName, object[] args)
        {
            if (IsVisible)
            {
                if (eventName == "HoverBegin") InformationManager.AddHintInformation("Sort Party");

                if (eventName == "HoverEnd") InformationManager.HideInformations();
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            // Probably a better way than this, but it's rare enough that the simplicity permits the performance overhead

            var sortedList = new List<TroopRosterElement>();

            for (var i = 0; i < _partyLogic.MemberRosters[(int) PartyScreenLogic.PartyRosterSide.Right].Count; i++)
            {
                TroopRosterElement t = _partyLogic.MemberRosters[(int) PartyScreenLogic.PartyRosterSide.Right]
                    .GetElementCopyAtIndex(i);
                sortedList.Add(t);
            }

            _partyLogic.MemberRosters[(int) PartyScreenLogic.PartyRosterSide.Right].Clear();
            sortedList.Sort(new TroopComparer(PartyScreenConfig.Sorter));

            foreach (TroopRosterElement rosterElement in sortedList)
                _partyLogic.MemberRosters[(int) PartyScreenLogic.PartyRosterSide.Right].AddToCounts(
                    rosterElement.Character, rosterElement.Number, false, rosterElement.WoundedNumber,
                    rosterElement.Xp);

            // Other option, no need to reset 
            //_partyVM.Call("InitializeTroopLists");

            // Update the current View, not necessary for the state to be preserved.
            _mainPartyList.Sort(new VMComparer(PartyScreenConfig.Sorter));
        }
    }

    internal class VMComparer : IComparer<PartyCharacterVM>
    {
        private readonly IComparer<CharacterObject> _trueComparer;

        public VMComparer(IComparer<CharacterObject> trueComparer)
        {
            _trueComparer = trueComparer;
        }

        public int Compare(PartyCharacterVM x, PartyCharacterVM y)
        {
            return _trueComparer.Compare(x.Character, y.Character);
        }
    }

    internal class TroopComparer : IComparer<TroopRosterElement>
    {
        private readonly IComparer<CharacterObject> _trueComparer;

        public TroopComparer(IComparer<CharacterObject> trueComparer)
        {
            _trueComparer = trueComparer;
        }

        public int Compare(TroopRosterElement x, TroopRosterElement y)
        {
            return _trueComparer.Compare(x.Character, y.Character);
        }
    }
}