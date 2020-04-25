using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel
{
    public class RecruitTillLimitVM : TaleWorlds.Library.ViewModel
    {

        private PartyVM _partyVm;
        private HintViewModel _limitHint;

        public RecruitTillLimitVM(PartyVM partyVm)
        {
            this._partyVm = partyVm;
            this._limitHint = new HintViewModel("Transfer All Units, top to bottom, up to your party limit.");
        }



        [DataSourceProperty]
        public HintViewModel LimitHint
        {
            get => _limitHint;
            set
            {
                if (value != this._limitHint)
                {
                    this._limitHint = value;
                    base.OnPropertyChanged(nameof(LimitHint));
                }
            }
        }

    }
}
