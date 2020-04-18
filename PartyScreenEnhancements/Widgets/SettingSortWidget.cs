using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

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
            this._main.SetState(state);
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
				this.SetWidgetsState("Disabled");
				return;
			}
			if (base.IsPressed)
			{
				this.SetWidgetsState("Pressed");
				return;
			}
			if (base.IsHovered)
			{
				this.SetWidgetsState("Hovered");
				return;
			}
			if (base.IsSelected)
			{
				this.SetWidgetsState("Selected");
				return;
			}
			this.SetWidgetsState("Default");
            
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
                return this._main;
            }
            set
            {
                if (this._main != value)
                {
                    this._main = value;
                    base.OnPropertyChanged(value, nameof(Main));
                }
            }
        }

        public RichTextWidget NameWidget
        {
            get => _nameWidget;
            set
            {
                if (this._nameWidget != value)
                {
                    this._nameWidget = value;
                    base.OnPropertyChanged(value, nameof(NameWidget));
                }
            }
        }

        public bool HasCustomSettings
        {
            get => _hasCustomSettings;
            set
            {
                if (this._hasCustomSettings != value)
                {
                    this._hasCustomSettings = value;
                    base.OnPropertyChanged(value, nameof(HasCustomSettings));
                }
            }
        }

        private Widget _main;
        private RichTextWidget _nameWidget;
        private bool _hasCustomSettings;
    }
}
