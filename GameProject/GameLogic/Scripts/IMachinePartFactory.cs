using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal interface IMachinePartFactory
    {
        string TexturePath { get; }
        void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine);
    }
}