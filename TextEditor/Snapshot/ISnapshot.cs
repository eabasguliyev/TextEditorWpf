namespace TextEditor.Snapshot
{
    public interface ISnapshot<T>
    {
        T GetState();
    }
}