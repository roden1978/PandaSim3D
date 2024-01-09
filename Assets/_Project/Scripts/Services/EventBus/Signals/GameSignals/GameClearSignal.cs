namespace CustomEventBus.Signals{
    public class GameClearSignal{
        public readonly bool Restart;
        public GameClearSignal(bool restart=false)
        {
            Restart = restart;
        }
    }
}