namespace Utilities.ConditionalActionSystem
{
    public delegate void OnTriggered<in T>(T data);
    public delegate void OnTriggered(object data);
}