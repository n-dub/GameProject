using System.Collections.Generic;
using GameProject.Ecs;

namespace GameProject.CoreEngine
{
    /// <summary>
    /// An abstract factory that creates game scenes (collections of entities)
    /// </summary>
    internal interface ISceneFactory
    {
        List<GameEntity> CreateScene();
    }
}