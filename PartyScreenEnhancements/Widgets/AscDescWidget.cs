using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

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
            if (UpArrowBrush == null || DownArrowBrush == null)
            {
                return;
            }

            if (IsDescending)
            {
                base.Brush = DownArrowBrush;
            }
            else
            {
                base.Brush = UpArrowBrush;
            }
        }

        [Editor(false)]
        public Brush UpArrowBrush
        {
            get
            {
                return _upBrush;
            }
            set
            {
                if (_upBrush != value)
                {
                    _upBrush = value;
                    base.OnPropertyChanged(value, nameof(UpArrowBrush));
                }
            }
        }

        [Editor(false)]
        public Brush DownArrowBrush
        {
            get
            {
                return _downBrush;
            }
            set
            {
                if (_downBrush != value)
                {
                    _downBrush = value;
                    base.OnPropertyChanged(value, nameof(DownArrowBrush));
                }
            }
        }

        [Editor(false)]
        public bool IsDescending
        {
            get
            {
                return _isDescending;
            }
            set
            {
                if (_isDescending != value)
                {
                    _isDescending = value;
                    base.OnPropertyChanged(value, nameof(IsDescending));
                }
                UpdateVisual();
            }
        }
    }
}
