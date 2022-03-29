using TaleWorlds.Library;

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
                return _imageIDs;
            }
            set
            {
                if (value != _imageIDs)
                {
                    _imageIDs = value;
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
