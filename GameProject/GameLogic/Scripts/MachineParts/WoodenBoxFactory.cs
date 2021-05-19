using System;
using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class WoodenBoxFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/wooden_box.png";
        public bool HasBoxCollision => true;

        public void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine)
        {
            var sprite = machine.GetComponent<Sprite>();
            sprite.AddShape(gameState, new QuadRenderShape(1)
            {
                ImagePath = TexturePath,
                Offset = cellPosition,
                Scale = Vector2F.One * MachineEditor.CellSize
            });
        }
    }
}