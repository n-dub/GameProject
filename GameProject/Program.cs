using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameLogic.Levels;

namespace GameProject
{
    internal static class Program
    {
        private static readonly ISceneFactory[] levels =
        {
            new TestSceneFactory(),
            new TestSceneFactory()
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