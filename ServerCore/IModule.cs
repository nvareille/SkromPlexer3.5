namespace SkromPlexer.ServerCore
{
    /// <summary>
    /// The definition for Modules
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Called upon Core.Init()
        /// </summary>
        /// <param name="core">A reference to Core</param>
        void Init(Core core);

        /// <summary>
        /// Called upon Core.Start()
        /// </summary>
        /// <param name="core">A reference to Core</param>
        void Start(Core core);

        /// <summary>
        /// Called upon Core.Update()
        /// </summary>
        /// <param name="core">A reference to Core</param>
        void Update(Core core);
    }
}
