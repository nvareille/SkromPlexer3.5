namespace SkromPlexer.ServerCore
{
    public interface IModule
    {
        void Init(Core core);

        void Start(Core core);

        void Update(Core core);
    }
}
