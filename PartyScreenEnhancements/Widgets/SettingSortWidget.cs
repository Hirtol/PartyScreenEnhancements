using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI;
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

        protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
            //screenWidget.SetCurrentTuple(this, this.IsTupleLeftSide);
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
                    base.OnPropertyChanged(value, "Main");
                }
            }
        }

        private Widget _main;
    }
}
