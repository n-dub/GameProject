using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs.Physics;
using GameProject.GameInput;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject.GameLogic.Scripts
{
    internal class ShellHummerScript : GameScript
    {
        public readonly List<Vector2F> Hummers = new List<Vector2F>();

        protected override void Update()
        {
            if (GameState.Keyboard[Keys.F] != KeyState.Down)
                return;

            foreach (var shell in Hummers
                .Select(hummer => (Vector2) hummer.TransformBy(Entity.GlobalTransform))
                .Select(point => point + Vector2.UnitX * MachineEditor.CellSize)
                .SelectMany(point => ShellScript.Shells.Where(shell => shell
                    .GetComponent<PhysicsBody>().FarseerBody.FixtureList.First().TestPoint(ref point))))
                shell.GetComponent<ShellScript>().Exploded = true;
        }
    }
}