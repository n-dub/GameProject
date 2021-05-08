using System;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameLogic.Levels;

namespace GameProject
{
    internal static class Program
    {
        private static readonly ISceneFactory[] levels =
        {
            new BrickWallTest()
        };

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow(levels));
        }
    }
}