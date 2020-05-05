using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyScreenEnhancements.ViewModel.HackedIn;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using UIExtenderLib.Interface;

namespace PartyScreenEnhancements.Extensions
{
    [ViewModelMixin]
    public class PartyVMMixin : BaseViewModelMixin<PartyVM>
    {

        private readonly PartyVM _viewModel;

        public PartyVMMixin(PartyVM viewModel) : base(viewModel)
        {
            this._categoryList = new MBBindingList<PartyCategoryVM>();

            if (_vm.TryGetTarget(out PartyVM vm))
            {
                _viewModel = vm;
            }
            else
            {
                throw new NullReferenceException("Could not get PartyVM from weak reference");
            }

            //(_categoryList as IMBBindingList).ListChanged

            _categoryList.Add(new PartyCategoryVM(_viewModel.MainPartyTroops));
        }

        public override void OnRefresh()
        {
            base.OnRefresh();
            _categoryList.Add(new PartyCategoryVM(_viewModel.MainPartyTroops));
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _categoryList = null;
        }

        [DataSourceProperty]
        public MBBindingList<PartyCategoryVM> CategoryList
        {
            get => _categoryList;
            set
            {
                if (value != _categoryList && _vm.TryGetTarget(out var pvm))
                {
                    _categoryList = value;
                    pvm.OnPropertyChanged(nameof(CategoryList));
                }
            }
        }

        private MBBindingList<PartyCategoryVM> _categoryList;

    }
}
