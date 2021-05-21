using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal interface IMachinePartFactory
    {
        string TexturePath { get; }
        bool HasBoxCollision { get; }
        bool Connectible { get; }
        void CreatePart(Vector2F cellPosition, float rotation, GameState gameState, GameEntity machine);
        void CleanUp();
        GameEntity CreateDestroyedPart(Vector2F position, float rotation);
    }
}