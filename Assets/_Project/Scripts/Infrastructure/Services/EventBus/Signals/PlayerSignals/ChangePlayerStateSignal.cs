using PlayerScripts;

namespace Infrastructure.Services.EventBus.Signals.PlayerSignals
{
    public class ChangePlayerStateSignal
    {
        public State PlayerState { get; }
        public ChangePlayerStateSignal(State state)
        {
            PlayerState = state;
        }
    }
}