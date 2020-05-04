using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;

namespace PartyScreenEnhancements.Widgets
{
    public class RadioButtonWidget : ButtonWidget
    {
        public RadioButtonWidget(UIContext context) : base(context)
        {
            base.SetState("Selected");
        }

        protected override void RefreshState()
        {
            base.RefreshState();
            if (true)
            {
                if (base.IsSelected)
                {
                    this.SetState("Selected");
                    this.Brush = FullBrush;
                    return;
                }
                if(!base.IsSelected)
                {
                    this.SetState("Default");
                    this.Brush = EmptyBrush;
                }
            }
        }

        private void GetSelectedState()
        {
            if (PartyScreenConfig.PathsToUpgrade.TryGetValue(TroopId, out var upgradePath))
            {
                if (upgradePath == _upgradePath)
                {
                    this.IsSelected = true;
                }
            }
        }


        [Editor(false)]
        public Brush EmptyBrush
        {
            get
            {
                return this._emptyBrush;
            }
            set
            {
                if (this._emptyBrush != value)
                {
                    this._emptyBrush = value;
                    base.OnPropertyChanged(value, nameof(EmptyBrush));
                }
            }
        }

        [Editor(false)]
        public Brush FullBrush
        {
            get
            {
                return this._fullBrush;
            }
            set
            {
                if (this._fullBrush != value)
                {
                    this._fullBrush = value;
                    base.OnPropertyChanged(value, nameof(FullBrush));
                }
            }
        }

        [Editor(false)]
        public string TroopId
        {
            get
            {
                return this._troopName;
            }
            set
            {
                if (this._troopName != value)
                {
                    this._troopName = value;
                    GetSelectedState();
                    base.OnPropertyChanged(value, nameof(TroopId));
                }
            }
        }

        [Editor(false)]
        public int UpgradePath
        {
            get
            {
                return this._upgradePath;
            }
            set
            {
                if (this._upgradePath != value)
                {
                    this._upgradePath = value;
                    base.OnPropertyChanged(value, nameof(UpgradePath));
                }
            }
        }


        private Brush _emptyBrush;
        private Brush _fullBrush;
        private string _troopName;
        private int _upgradePath;





    }
}
