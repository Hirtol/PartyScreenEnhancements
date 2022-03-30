using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options;

namespace PartyScreenEnhancements.Widgets
{
    /// <summary>
    /// This is necessary since the OptionsScreenWidget now includes the PerformanceTab within the widget code, which means it's no longer as
    /// general as it should be for our purposes. This override simply ensures the references to PerformanceTab are not reached, so that
    /// we don't get a null pointer exception.
    /// </summary>
	public class ClassicOptionsScreenWidget : OptionsScreenWidget
    {
        private bool _initialized;

        public ClassicOptionsScreenWidget(UIContext context) : base(context)
        {
        }

        protected override void OnUpdate(float dt)
        {
            if (!_initialized)
            {
                using (IEnumerator<Widget> enumerator = base.AllChildren.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        OptionsItemWidget optionsItemWidget;
                        if ((optionsItemWidget = (enumerator.Current as OptionsItemWidget)) != null)
                        {
                            optionsItemWidget.SetCurrentScreenWidget(this);
                        }
                    }
                }
                _initialized = true;
            }
        }

    }
}
