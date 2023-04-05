public abstract class IState
{
    public virtual void Tick() {}
    public virtual void OnEnter() {}
    public virtual void OnExit() {}
    public virtual bool StateEnded() 
    {
        return false;
    }
}