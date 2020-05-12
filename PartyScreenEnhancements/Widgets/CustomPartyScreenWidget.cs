using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

namespace PartyScreenEnhancements.Widgets
{
    public class CustomPartyScreenWidget : Widget
    {
        public PartyTroopTupleWidget CurrentMainTuple { get; private set; }

        public PartyTroopTupleWidget CurrentOtherTuple { get; private set; }

        public AutoScrollablePanelWidget MainScrollPanel { get; set; }

        public AutoScrollablePanelWidget OtherScrollPanel { get; set; }


        public CustomPartyScreenWidget(UIContext context) : base(context)
        {
        }

        protected override void OnConnectedToRoot()
        {
            Context.TwoDimensionContext.PlaySound("panels/twopanel_open");
        }

        protected override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            Widget latestMouseUpWidget = EventManager.LatestMouseUpWidget;
            if (CurrentMainTuple != null && latestMouseUpWidget != null &&
                !(latestMouseUpWidget is PartyTroopTupleWidget) &&
                !CurrentMainTuple.CheckIsMyChildRecursive(latestMouseUpWidget))
            {
                PartyTroopTupleWidget currentOtherTuple = CurrentOtherTuple;
                if (currentOtherTuple != null && !currentOtherTuple.CheckIsMyChildRecursive(latestMouseUpWidget) &&
                    IsWidgetChildOfType<PartyFormationDropdownWidget>(latestMouseUpWidget) == null)
                {
                    SetCurrentTuple(null, false);
                }
            }
        }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);
            if (_newAddedTroop != null)
            {
                var characterID = _newAddedTroop.CharacterID;
                PartyTroopTupleWidget currentMainTuple = CurrentMainTuple;
                if (characterID == (currentMainTuple != null ? currentMainTuple.CharacterID : null))
                {
                    var isPrisoner = _newAddedTroop.IsPrisoner;
                    PartyTroopTupleWidget currentMainTuple2 = CurrentMainTuple;
                    var flag = currentMainTuple2 != null ? new bool?(currentMainTuple2.IsPrisoner) : null;
                    if ((isPrisoner == flag.GetValueOrDefault()) & (flag != null))
                    {
                        var isTupleLeftSide = _newAddedTroop.IsTupleLeftSide;
                        PartyTroopTupleWidget currentMainTuple3 = CurrentMainTuple;
                        flag = currentMainTuple3 != null ? new bool?(currentMainTuple3.IsTupleLeftSide) : null;
                        if (!((isTupleLeftSide == flag.GetValueOrDefault()) & (flag != null)))
                        {
                            CurrentOtherTuple = _newAddedTroop;
                            CurrentOtherTuple.IsSelected = true;
                            UpdateScrollTarget();
                        }
                    }
                }

                _newAddedTroop = null;
            }
        }

        public void SetCurrentTuple(PartyTroopTupleWidget tuple, bool isLeftSide)
        {
            if (CurrentMainTuple != null && CurrentMainTuple != tuple)
            {
                CurrentMainTuple.IsSelected = false;
                if (CurrentOtherTuple != null)
                {
                    CurrentOtherTuple.IsSelected = false;
                    CurrentOtherTuple = null;
                }
            }

            if (tuple == null)
            {
                CurrentMainTuple = null;
                RemoveZeroCountItems();
                if (CurrentOtherTuple != null)
                {
                    CurrentOtherTuple.IsSelected = false;
                    CurrentOtherTuple = null;
                }
            }
            else
            {
                if (tuple == CurrentMainTuple || tuple == CurrentOtherTuple)
                {
                    SetCurrentTuple(null, false);
                    return;
                }

                CurrentMainTuple = tuple;
                CurrentOtherTuple = FindTupleWithTroopIDInList(CurrentMainTuple.CharacterID,
                    CurrentMainTuple.IsTupleLeftSide, CurrentMainTuple.IsPrisoner);
                if (CurrentOtherTuple != null)
                {
                    CurrentOtherTuple.IsSelected = true;
                    UpdateScrollTarget();
                }
            }
        }

        private void RefreshWarningStatuses()
        {
            TextWidget prisonerLabel = PrisonerLabel;
            if (prisonerLabel != null)
            {
                prisonerLabel.SetState(IsPrisonerWarningEnabled ? "OverLimit" : "Default");
            }

            TextWidget troopLabel = TroopLabel;
            if (troopLabel != null)
            {
                troopLabel.SetState(IsTroopWarningEnabled ? "OverLimit" : "Default");
            }

            TextWidget otherTroopLabel = OtherTroopLabel;
            if (otherTroopLabel == null)
            {
                return;
            }

            otherTroopLabel.SetState(IsOtherTroopWarningEnabled ? "OverLimit" : "Default");
        }

        private PartyTroopTupleWidget FindTupleWithTroopIDInList(string troopID, bool searchMainList, bool isPrisoner)
        {
            IEnumerable<PartyTroopTupleWidget> source;
            if (searchMainList)
            {
                //TODO: Make this more elegant
                // This entire copy paste job was necessary just to change this one line ._.
                // Please at least make your public properties be settable TW D:
                source = isPrisoner ? MainPrisonerList.Children.Cast<PartyTroopTupleWidget>() : EnumerateMainMemberList();
            }
            else
            {
                source = isPrisoner
                    ? OtherPrisonerList.Children.Cast<PartyTroopTupleWidget>()
                    : OtherMemberList.Children.Cast<PartyTroopTupleWidget>();
            }

            return source.SingleOrDefault(i => i.CharacterID == troopID);
        }

        private IEnumerable<PartyTroopTupleWidget> EnumerateMainMemberList()
        {
            IEnumerable<PartyTroopTupleWidget> result = new List<PartyTroopTupleWidget>();

            foreach (ListPanel listPanel in MainMemberList.Children.Cast<ListPanel>())
            {
                result = result.Concat( listPanel.Children.Cast<PartyTroopTupleWidget>());
            }

            return result;
        }

        private void RemoveZeroCountItems()
        {
            EventFired("RemoveZeroCounts", Array.Empty<object>());
        }

        private void UpdateScrollTarget()
        {
            try
            {
                if (CurrentOtherTuple != null && CurrentOtherTuple.ParentWidget != null)
                {
                    AutoScrollablePanelWidget autoScrollablePanelWidget =
                        CurrentOtherTuple.IsTupleLeftSide ? OtherScrollPanel : MainScrollPanel;
                    if (autoScrollablePanelWidget != null)
                    {
                        var y = autoScrollablePanelWidget.Size.Y;
                        var num = 50f * Context.InverseScale;
                        var num2 = 66f * Context.InverseScale;
                        var num3 = CurrentOtherTuple.IsPrisoner
                            ? CurrentOtherTuple.IsTupleLeftSide ? OtherMemberList.Size.Y : MainMemberList.Size.Y + num2
                            : num2 + num;
                        var currentTargetScroll =
                            CurrentOtherTuple.GetVisibleSiblingIndex() *
                            (num2 + CurrentOtherTuple.ScaledMarginTop + CurrentOtherTuple.ScaledMarginBottom) + num3 -
                            y / 2f;
                        autoScrollablePanelWidget.CurrentTargetScroll = currentTargetScroll;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnNewTroopAdded(Widget parent, Widget addedChild)
        {
            PartyTroopTupleWidget newAddedTroop;
            if (CurrentMainTuple != null && (newAddedTroop = addedChild as PartyTroopTupleWidget) != null)
            {
                _newAddedTroop = newAddedTroop;
            }
        }

        [Editor]
        public int MainPartyTroopSize
        {
            get => _mainPartyTroopSize;
            set
            {
                if (_mainPartyTroopSize != value)
                {
                    _mainPartyTroopSize = value;
                    OnPropertyChanged(value);
                }
            }
        }

        [Editor]
        public bool IsPrisonerWarningEnabled
        {
            get => _isPrisonerWarningEnabled;
            set
            {
                if (_isPrisonerWarningEnabled != value)
                {
                    _isPrisonerWarningEnabled = value;
                    OnPropertyChanged(value);
                    RefreshWarningStatuses();
                }
            }
        }

        [Editor]
        public bool IsOtherTroopWarningEnabled
        {
            get => _isOtherTroopWarningEnabled;
            set
            {
                if (_isOtherTroopWarningEnabled != value)
                {
                    _isOtherTroopWarningEnabled = value;
                    OnPropertyChanged(value);
                    RefreshWarningStatuses();
                }
            }
        }

        [Editor]
        public bool IsTroopWarningEnabled
        {
            get => _isTroopWarningEnabled;
            set
            {
                if (_isTroopWarningEnabled != value)
                {
                    _isTroopWarningEnabled = value;
                    OnPropertyChanged(value);
                    RefreshWarningStatuses();
                }
            }
        }

        [Editor]
        public TextWidget TroopLabel
        {
            get => _troopLabel;
            set
            {
                if (_troopLabel != value)
                {
                    _troopLabel = value;
                    OnPropertyChanged(value);
                    RefreshWarningStatuses();
                }
            }
        }

        [Editor]
        public TextWidget PrisonerLabel
        {
            get => _prisonerLabel;
            set
            {
                if (_prisonerLabel != value)
                {
                    _prisonerLabel = value;
                    OnPropertyChanged(value);
                    RefreshWarningStatuses();
                }
            }
        }

        [Editor]
        public TextWidget OtherTroopLabel
        {
            get => _otherTroopLabel;
            set
            {
                if (_otherTroopLabel != value)
                {
                    _otherTroopLabel = value;
                    OnPropertyChanged(value);
                    RefreshWarningStatuses();
                }
            }
        }

        [Editor]
        public ListPanel OtherMemberList
        {
            get => _otherMemberList;
            set
            {
                if (_otherMemberList != value)
                {
                    _otherMemberList = value;
                    value.ItemAddEventHandlers.Add(OnNewTroopAdded);
                }
            }
        }

        [Editor]
        public ListPanel OtherPrisonerList
        {
            get => _otherPrisonerList;
            set
            {
                if (_otherPrisonerList != value)
                {
                    _otherPrisonerList = value;
                    value.ItemAddEventHandlers.Add(OnNewTroopAdded);
                }
            }
        }

        [Editor]
        public ListPanel MainMemberList
        {
            get => _mainMemberList;
            set
            {
                if (_mainMemberList != value)
                {
                    _mainMemberList = value;
                    value.ItemAddEventHandlers.Add(OnNewTroopAdded);
                }
            }
        }

        [Editor]
        public ListPanel MainPrisonerList
        {
            get => _mainPrisonerList;
            set
            {
                if (_mainPrisonerList != value)
                {
                    _mainPrisonerList = value;
                    value.ItemAddEventHandlers.Add(OnNewTroopAdded);
                }
            }
        }

        private T IsWidgetChildOfType<T>(Widget currentWidget) where T : Widget
        {
            while (currentWidget != null)
            {
                T result;
                if ((result = currentWidget as T) != null)
                {
                    return result;
                }

                currentWidget = currentWidget.ParentWidget;
            }

            return default;
        }

        private PartyTroopTupleWidget _newAddedTroop;

        private int _mainPartyTroopSize;

        private bool _isPrisonerWarningEnabled;

        private bool _isTroopWarningEnabled;

        private bool _isOtherTroopWarningEnabled;

        private TextWidget _troopLabel;

        private TextWidget _prisonerLabel;

        private TextWidget _otherTroopLabel;

        private ListPanel _otherMemberList;

        private ListPanel _otherPrisonerList;

        private ListPanel _mainMemberList;

        private ListPanel _mainPrisonerList;
    }
}