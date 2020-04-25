using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;

namespace PartyScreenEnhancements.Widgets
{
    public class RadioButtonWidget : ButtonWidget
    {
        public RadioButtonWidget(UIContext context) : base(context)
        {
            base.SetState("Selected");
            //TODO: FIND OUT WHY THESE ARE NOT APPEARING UNTIL YOU HOVER OVER THEM (TRIGGER RefreshState()?)
        }

        protected override void RefreshState()
        {
            base.RefreshState();
            if(base.IsVisible)
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


        private Brush _emptyBrush;
        private Brush _fullBrush;





    }
}
