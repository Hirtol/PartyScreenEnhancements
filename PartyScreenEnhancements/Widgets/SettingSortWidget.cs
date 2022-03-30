using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.Widgets
{
    public class SettingSortWidget : ButtonWidget
    {
        public SettingSortWidget(UIContext context) : base(context)
        {
            base.OverrideDefaultStateSwitchingEnabled = true;
            base.AddState("Selected");
        }

        private void SetWidgetsState(string state)
        {
            base.SetState(state);
            _main.SetState(state);
        }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);
            if (HasCustomSettings)
            {
                NameWidget.Brush.FontColor = Color.ConvertStringToColor("#FFD700FF");
            }
        }

        protected override void RefreshState()
        {
            base.RefreshState();

            if (base.IsDisabled)
            {
                SetWidgetsState("Disabled");
                return;
            }
            if (base.IsPressed)
            {
                SetWidgetsState("Pressed");
                return;
            }
            if (base.IsHovered)
            {
                SetWidgetsState("Hovered");
                return;
            }
            if (base.IsSelected)
            {
                SetWidgetsState("Selected");
                return;
            }
            SetWidgetsState("Default");

        }

        public void ResetIsSelected()
        {
            base.IsSelected = false;
        }

        private void OnValueChanged(PropertyOwnerObject arg1, string arg2, object arg3)
        {
            if (arg2 == "ValueInt")
            {
                base.AcceptDrag = ((int)arg3 > 0);
            }
        }

        [Editor(false)]
        public Widget Main
        {
            get
            {
                return _main;
            }
            set
            {
                if (_main != value)
                {
                    _main = value;
                    base.OnPropertyChanged(value, nameof(Main));
                }
            }
        }

        public RichTextWidget NameWidget
        {
            get => _nameWidget;
            set
            {
                if (_nameWidget != value)
                {
                    _nameWidget = value;
                    base.OnPropertyChanged(value, nameof(NameWidget));
                }
            }
        }

        public bool HasCustomSettings
        {
            get => _hasCustomSettings;
            set
            {
                if (_hasCustomSettings != value)
                {
                    _hasCustomSettings = value;
                    base.OnPropertyChanged(value, nameof(HasCustomSettings));
                }
            }
        }

        private Widget _main;
        private RichTextWidget _nameWidget;
        private bool _hasCustomSettings;
    }
}
