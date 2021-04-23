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

        /// <summary>
        ///     Scene width in meters
        /// </summary>
        public float Width;

        /// <summary>
        ///     The value to add to X coordinate of the scene to put it to 0
        /// </summary>
        public float Offset;
    }
}