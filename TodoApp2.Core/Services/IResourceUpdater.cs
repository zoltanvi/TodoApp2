namespace TodoApp2.Core
{
    public interface IResourceUpdater
    {
        void UpdateResource(string resourceName, object resourceValue);

        object GetResourceValue(string resourceName);
    }
}
