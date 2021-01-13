using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options;
using TaleWorlds.TwoDimension;

namespace PartyScreenEnhancements.Widgets
{
	public class ClassicOptionsScreenWidget : OptionsScreenWidget
	{
        public ClassicOptionsScreenWidget(UIContext context) : base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			// Override to ensure that the PerformanceTabToggle doesn't get used, since the OptionsScreenWidget is sadly no longer
			// as generic as it used to be.
		} 

	}
}
