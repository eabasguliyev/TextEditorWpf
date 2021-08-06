using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace TextEditor.Snapshot
{
    public class SnapshotCare<T> : ISnapshotCare<T> where T: class
    {
        private readonly List<Snapshot<T>> _snapshots;

        private int _currentIndex;
        public SnapshotCare()
        {
            _snapshots = new List<Snapshot<T>>();
            _currentIndex = -1;
        }


        public void CreateSnapshot(T data)
        {
            if (_snapshots.Any() && data == _snapshots[_currentIndex].GetState())
                return;

            if (_currentIndex < _snapshots.Count - 1)
                _snapshots.RemoveRange(_currentIndex + 1, _snapshots.Count - _currentIndex - 1);
            
            var snapshot = new Snapshot<T>(data);

            _snapshots.Add(snapshot);
            _currentIndex++;
        }

        public ISnapshot<T> GetBackSnapshot()
        {
            if (_currentIndex == 0) return null;

            _currentIndex--;
            return _snapshots[_currentIndex];
        }

        public ISnapshot<T> GetForwardSnapshot()
        {
            if (_currentIndex == _snapshots.Count - 1)
                return null;

            _currentIndex++;
            return _snapshots[_currentIndex];
        }

        public void Reset()
        {
            _snapshots.Clear();
            _currentIndex = 0;
        }
    }
}