public class EnemyStateMachine
{
    private EnemyState currentState;

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState?.Exit();
        currentState = newState; 
        currentState?.Enter();

    }
    public void Update()
    {
        currentState?.Update();
    }
}