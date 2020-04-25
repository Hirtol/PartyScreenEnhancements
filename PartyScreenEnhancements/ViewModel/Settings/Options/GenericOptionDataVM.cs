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
        private string[] _imageIDs;

        public GenericOptionDataVM(string name, string description, int id)
        {
            Name = name;
            Description = description;
            OptionTypeID = id;
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
                    base.OnPropertyChanged(nameof(ImageIDs));
                }
            }
        }

        [DataSourceProperty]
        public string Name { get; set; }

        [DataSourceProperty]
        public int OptionTypeID { get; set; }

        [DataSourceProperty]
        public string Description { get; set; }
    }
}
