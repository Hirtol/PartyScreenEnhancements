using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;

namespace PartyScreenEnhancements.ViewModel.HackedIn
{
    public class PSEWrapperVM : TaleWorlds.Library.ViewModel
    {
        private TaleWorlds.Library.ViewModel _primary;


        public PSEWrapperVM(TaleWorlds.Library.ViewModel primaryViewModel)
        {
            _primary = primaryViewModel;
        }

        [DataSourceProperty]
        public TaleWorlds.Library.ViewModel WrapperViewModel
        {
            get => _primary;
        }

        [DataSourceProperty]
        public bool IsCategory
        {
            get => _primary.GetType() == typeof(PartyCategoryVM);
        }

        public bool IsPartyCharacter
        {
            get => this._primary.GetType() == typeof(PartyCharacterVM);
        }

    }
}
