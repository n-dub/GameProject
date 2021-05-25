using System.Collections.Generic;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class ShellFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/shell.png";
        public bool HasBoxCollision => false;
        public bool Connectible => false;
        
        private readonly List<Vector2F> destroyedCells = new List<Vector2F>();

        public void CreatePart(Vector2F cellPosition, float rotation, GameState gameState, GameEntity machine)
        {
            if (destroyedCells.Any(c => c == cellPosition))
                return;
            var shell = LevelUtility.CreatePhysicalRectangle(cellPosition.TransformBy(machine.GlobalTransform),
                MachineEditor.CellSize * Vector2F.One * 0.9f);
            shell.GetComponent<PhysicsBody>().Colliders.First().Density = 5;
            if (shell.GetComponent<Sprite>().Shapes.First() is QuadRenderShape shape)
                shape.ImagePath = TexturePath;
            ShellScript.Shells.Add(shell);
            gameState.AddEntity(shell);
            var explosive = new ShellScript {Impulse = Vector2F.UnitX * 7f};
            explosive.OnExplode += () => destroyedCells.Add(cellPosition);
            shell.AddComponent(explosive);
        }

        public void CleanUp()
        {
            foreach (var shell in ShellScript.Shells)
                shell.Destroy();
            ShellScript.Shells.Clear();
        }

        public GameEntity CreateDestroyedPart(Vector2F position, float rotation)
        {
            return null;
        }
    }
}
