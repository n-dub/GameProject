using System.Collections.Generic;
using GameProject.Ecs;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Represents data of one game level
    /// </summary>
    internal struct SceneData
    {
        /// <summary>
        ///     All scene's entities
        /// </summary>
        public List<GameEntity> Entities;

        public Camera Camera;
    }
}