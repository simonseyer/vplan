using System;
using System.ComponentModel;

namespace FLSVertretungsplan
{
    public class Property<T> : INotifyPropertyChanged
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set { _value = value; NotifyPropertyChanged("Value"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
