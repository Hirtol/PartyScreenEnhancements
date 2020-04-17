using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PartyScreenEnhancements.ViewModel.Settings.Options
{
    public class GenericOptionDataVM : TaleWorlds.Library.ViewModel
    {
        private string _name;
        private string _description;
        private int _id;
        private string[] _imageIDs;

        public GenericOptionDataVM(string name, string description, int id)
        {
            _name = name;
            _description = description;
            _id = id;
        }


        [DataSourceProperty]
        public string[] ImageIDs
        {
            get
            {
                return this._imageIDs;
            }
            set
            {
                if (value != this._imageIDs)
                {
                    this._imageIDs = value;
                    base.OnPropertyChanged("ImageIDs");
                }
            }
        }

        [DataSourceProperty]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [DataSourceProperty]
        public int OptionTypeID
        {
            get => _id;
            set => _id = value;
        }

        [DataSourceProperty]
        public string Description
        {
            get => _description;
            set => _description = value;
        }
    }
}
