namespace Utilities.ConditionalActionSystem
{
    public interface ITrigger
    {
        public event OnTriggered Triggered;
    }
}