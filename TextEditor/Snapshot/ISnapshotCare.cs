namespace TextEditor.Snapshot
{
    public interface ISnapshotCare<T>
    {
        void CreateSnapshot(T state);
        ISnapshot<T> GetBackSnapshot();
        ISnapshot<T> GetForwardSnapshot();
        void Reset();
    }
}