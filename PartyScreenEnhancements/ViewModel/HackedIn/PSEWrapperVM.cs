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
        private readonly TaleWorlds.Library.ViewModel _primary;


        public PSEWrapperVM(TaleWorlds.Library.ViewModel primaryViewModel)
        {
            _primary = primaryViewModel;
        }


        public override bool Equals(object obj)
        {
            if (obj is PSEWrapperVM other)
            {
                return Equals(other);
            }

            return false;
        }

        protected bool Equals(PSEWrapperVM other)
        {
            return Equals(_primary, other._primary);
        }

        public override int GetHashCode()
        {
            return (_primary != null ? _primary.GetHashCode() : 0);
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

        [DataSourceProperty]
        public bool IsPartyCharacter
        {
            get => this._primary.GetType() == typeof(PartyCharacterVM);
        }

    }
}
