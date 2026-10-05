using UnityEngine;

// OO가 지닐 수 있는 상태들을 BaseState를 상속받는 각각의 클래스로 구현
// 상태에 따라 수행해야 할 행동을 정의
// 주의할 점: 상태 변경에 대한 로직은 작성하면 안됨
// BaseState를 상속받는 클래스는 다른 BaseState 자식 클래스들에 대해 알 수 없고
// 오직 어떤 행동을 해야 하는지에 대한 내용만을 구현
// 상태 변경에 대한 책임은 OO클래스에게 있다.
public interface IState
{
    void OnStateEnter();
    void OnStateUpdate();
    void OnStateFixedUpdate();
    void OnStateExit();
}

public abstract class BaseState<T> : IState
{
    protected T owner;  // // 주인. PlayerState에서는 Player, MonsterState에서는 Monster

    public BaseState(T _owner)
    {
        this.owner = _owner;
    }

    //IState 구현
    public abstract void OnStateEnter();    // 상태를 처음 진입했을 때 한 번만 호출되는 메서드
    
    public abstract void OnStateUpdate();   // 매 프레임마다 호출되어야 하는 메서드
    public virtual void OnStateFixedUpdate() { }
    
    public virtual void OnStateExit() { }   // 상태가 변경되면 호출되는 메서드
}

public abstract class PlayerState : BaseState<Player>
{
    protected PlayerData context => owner.context;  // 모든 상태가 같은 컨텍스트를 사용

    public PlayerState(Player _player) : base(_player) { }
}
public abstract class MonsterState : BaseState<Monster>
{
    //protected MonsterContext context => owner.context;
    public MonsterState(Monster _monster) : base(_monster) { }
}

public class StateMachine
{
    private IState currentState;
    public IState CurrentState => currentState;

    public StateMachine(IState _initState)
    {
        ChangeState(_initState);
    }

    public void ChangeState(IState _nextState)
    {
        if (currentState == _nextState)
            return;

        if (currentState != null)
            currentState.OnStateExit();

        currentState = _nextState;
        currentState.OnStateEnter();
    }

    public void UpdateState()
    {
        if (currentState != null)
            currentState.OnStateUpdate();
    }

    public void FixedUpdateState()
    {
        if (currentState != null)
            currentState.OnStateFixedUpdate();
    }
}