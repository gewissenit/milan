namespace EcoFactory.Components.States
{
  public interface IState
  {
    void Enter();
    void Exit();
    bool Active { get; }
  }
}
