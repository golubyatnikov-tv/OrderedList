using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tests.Annotations;

namespace Tests
{
    public class TestNotifyObj : INotifyPropertyChanged
    {
        private int _order;
        private static int _lastId;        

        public TestNotifyObj()
        {
            Id = ++_lastId;
        }

        public TestNotifyObj(int order) : this()
        {
            Order = order;
        }

        public int Id { get; }

        public int Order
        {
            get => _order;
            set
            {
                if (value == _order) return;
                _order = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"Id: {Id}, Order: {Order}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}