namespace GameProject.CoreEngine
{
    /// <summary>
    ///     An abstract factory that creates game scenes (collections of entities)
    /// </summary>
    internal interface ISceneFactory
    {
        SceneData CreateScene();
    }
}