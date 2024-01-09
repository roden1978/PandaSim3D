namespace CustomEventBus.Signals
{
    public class ChangeActiveSlotSignal
    {
        public int Id;
        public ChangeActiveSlotSignal(int id)
        {
            Id = id;
        }
    }
}