using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal interface IMachinePartFactory
    {
        string TexturePath { get; }
        bool HasBoxCollision { get; }
        void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine);
    }
}