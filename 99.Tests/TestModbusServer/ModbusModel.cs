using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestModbusServer
{
    public class ModbusItem : INotifyPropertyChanged
    {
        private bool _value;

        public int Address { get; set; }
        public string Name { get; set; }

        public bool Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}