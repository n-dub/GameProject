using System;
using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.RenderShapes;

namespace GameProject.GameLogic.Scripts
{
    internal class NoiseAnimation : GameScript
    {
        private readonly Random random = new Random();
        private IList<IRenderShape> shapes;
        
        protected override void Initialize()
        {
            shapes = Entity.GetComponent<Sprite>().Shapes;
            
            StartCoroutine(AnimCoroutine);
        }

        private IEnumerable<Awaiter> AnimCoroutine()
        {
            var prevIndex = 0;
            yield return Awaiter.WaitForFrames(2);
            foreach (var shape in shapes)
                shape.IsActive = false;
            while (true)
            {
                shapes[prevIndex].IsActive = false;
                prevIndex = random.Next(0, shapes.Count);
                shapes[prevIndex].IsActive = true;
                yield return Awaiter.WaitForSeconds(0.03f);
            }
        }
    }
}