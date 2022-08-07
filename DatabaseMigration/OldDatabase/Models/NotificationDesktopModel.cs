using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xunkong.Core.Old.XunkongApi;

namespace Xunkong.Desktop.Database.Old.Models
{

    [Table("Notifications")]
    [Index(nameof(Category))]
    [Index(nameof(HasRead))]
    public class NotificationDesktopModel : NotificationModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool _HasRead;
        public bool HasRead
        {
            get { return _HasRead; }
            set
            {
                _HasRead = value;
                OnPropertyChanged();
            }
        }

        //public bool HasRead { get; set; }


    }
}
