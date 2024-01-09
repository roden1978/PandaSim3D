namespace Common
{
    public interface IPayloadState<in TPayload> : IUpdateableState
    {
        public void Enter(TPayload payload);
    }
}