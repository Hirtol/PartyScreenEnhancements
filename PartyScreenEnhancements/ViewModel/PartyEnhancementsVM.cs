using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.ViewManagers;
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
            this._sortTroopsVM = new SortAllTroopsVM(partyVM, partyScreenLogic);
            this._upgradeTroopsVM = new UpgradeAllTroopsVM(partyScreenLogic, partyVM);
        }


        [DataSourceProperty]
        public UpgradeAllTroopsVM UpgradeAllTroops
        {
            get
            {
                return _upgradeTroopsVM;
            }
            set
            {
                if (value != this._upgradeTroopsVM)
                {
                    this._upgradeTroopsVM = value;
                    base.OnPropertyChanged("UpgradeAllTroops");
                }
            }
        }

        [DataSourceProperty]
        public SortAllTroopsVM SortAllTroops
        {
            get
            {
                return _sortTroopsVM;
            }
            set
            {
                if (value != this._sortTroopsVM)
                {
                    this._sortTroopsVM = value;
                    base.OnPropertyChanged("SortAllTroops");
                }
            }
        }

        [DataSourceProperty]
        public PartyVM EnhancementPartyVM
        {
            get
            {
                return this._partyVM;
            }
        }

        [DataSourceProperty]
        public PartyScreenLogic EnhancementPartyLogic
        {
            get
            {
                return this._partyScreenLogic;
            }
        }



        private SortAllTroopsVM _sortTroopsVM;
        private UpgradeAllTroopsVM _upgradeTroopsVM;
        
    }
}
