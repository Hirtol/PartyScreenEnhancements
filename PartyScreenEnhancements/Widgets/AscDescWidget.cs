using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.Comparers;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.Widgets
{
    public class AscDescWidget : ButtonWidget
    {
        private Brush _upBrush;
        private Brush _downBrush;
        private bool _isDescending;

        public AscDescWidget(UIContext context) : base(context)
        {
        }

        private void UpdateVisual()
        {
            if (this.UpArrowBrush == null || this.DownArrowBrush == null)
            {
                return;
            }

            if (this.IsDescending)
            {
                base.Brush = this.DownArrowBrush;
            }
            else
            {
                base.Brush = this.UpArrowBrush;
            }
        }

        [Editor(false)]
        public Brush UpArrowBrush
        {
            get
            {
                return this._upBrush;
            }
            set
            {
                if (this._upBrush != value)
                {
                    this._upBrush = value;
                    base.OnPropertyChanged(value, nameof(UpArrowBrush));
                }
            }
        }

        [Editor(false)]
        public Brush DownArrowBrush
        {
            get
            {
                return this._downBrush;
            }
            set
            {
                if (this._downBrush != value)
                {
                    this._downBrush = value;
                    base.OnPropertyChanged(value, nameof(DownArrowBrush));
                }
            }
        }

        [Editor(false)]
        public bool IsDescending
        {
            get
            {
                return this._isDescending;
            }
            set
            {
                if (this._isDescending != value)
                {
                    this._isDescending = value;
                    base.OnPropertyChanged(value, nameof(IsDescending));
                }
                this.UpdateVisual();
            }
        }
    }
}
