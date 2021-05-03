namespace GameProject.CoreEngine
{
    internal class ResourceHandle<T> where T : class
    {
        public T Resource => handle.Resource as T;

        public string Name => handle.Name;

        private readonly ResourceHandle handle;

        public ResourceHandle(ResourceHandle resource)
        {
            handle = resource;
        }
    }

    internal class ResourceHandle
    {
        public object Resource { get; }

        public string Name { get; }

        public ResourceHandle(object resource, string name)
        {
            Resource = resource;
            Name = name;
        }
    }
}