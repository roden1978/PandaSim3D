namespace Common
{
    public interface IState : IUpdateableState
    {
        public void Enter();
    }
}