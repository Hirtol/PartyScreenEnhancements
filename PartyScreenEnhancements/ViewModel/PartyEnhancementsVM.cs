using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class PartyEnhancementsVM : TaleWorlds.Library.ViewModel
    {

        protected readonly PartyVM _partyVM;
        protected readonly PartyScreenLogic _partyScreenLogic;
        

        public PartyEnhancementsVM(PartyVM partyVM, PartyScreenLogic partyScreenLogic)
        {
            this._partyVM = partyVM;
            this._partyScreenLogic = partyScreenLogic;
            _upgradeHint = new HintViewModel("Upgrade All Troops");
        }


        [DataSourceProperty]
        public HintViewModel UpgradeHint
        {
            get
            {
                return _upgradeHint;
            }
            set
            {
                if (value != this._upgradeHint)
                {
                    this._upgradeHint = value;
                    base.OnPropertyChanged("UpgradeHint");
                }
            }
        }


        private HintViewModel _upgradeHint;

    }
}
