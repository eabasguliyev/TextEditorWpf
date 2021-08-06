using System.Windows.Documents;

namespace TextEditor.Snapshot
{
    public class Snapshot<T> : ISnapshot<T>
    {
        private readonly T _state;

        public Snapshot(T state)
        {
            _state = state;
        }
        public T GetState() => _state;
    }
}