using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.CoreEngine
{
    internal static class ResourceManager
    {
        private static readonly Dictionary<string, WeakReference> loadedResources = new Dictionary<string, WeakReference>();

        public static T LoadResource<T>(Func<string, T> loader, string fileName)
            where T : class
        {
            if (loadedResources.TryGetValue(fileName, out var reference) && reference.IsAlive)
                return reference.Target as T;

            var resource = loader(fileName);
            loadedResources[fileName] = new WeakReference(resource, false);
            return resource;
        }
    }
}
