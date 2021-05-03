using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Class to manage all game resources. It loads all required resources on-demand and
    ///     stores them in weak references.
    /// </summary>
    internal static class ResourceManager
    {
        private static readonly Dictionary<string, WeakReference<ResourceHandle>> loadedResources =
            new Dictionary<string, WeakReference<ResourceHandle>>();

        /// <summary>
        ///     Load a resource
        /// </summary>
        /// <param name="loader">A function that loads certain resource from file</param>
        /// <param name="fileName">File path to use for call to the loader</param>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <returns>Strong reference to the loaded resource</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResourceHandle<T> LoadResource<T>(Func<string, T> loader, string fileName) where T : class
        {
            if (loadedResources.TryGetValue(fileName, out var reference)
                && reference.TryGetTarget(out var target))
                return new ResourceHandle<T>(target);

            var resource = new ResourceHandle(loader(fileName), fileName);
            loadedResources[fileName] = new WeakReference<ResourceHandle>(resource, false);
            return new ResourceHandle<T>(resource);
        }
    }
}