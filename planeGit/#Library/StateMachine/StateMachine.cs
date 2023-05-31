using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public abstract class State
{
    protected StateMachine Machine { get; private set; }
    
    public void Initialize(StateMachine machine) => Machine = machine;
    
    public virtual void OnBegin() { }
    public virtual void OnEnd() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnEndOfFrame() { }
}

public class StateMachine
{
    #region Fields

    Dictionary<Type, State> states = new Dictionary<Type, State>();
    Subject<State> beginSubject = new();
    Subject<State> endSubject = new();
    BoolReactiveProperty isActive = new (false);
    
    #endregion
    
    #region Properties

    public State CurrentState { get; private set; } = null;
    public State PrevState { get; private set; }
    
    public bool IsActive => isActive.Value;

    public IObservable<Unit> OnActive => isActive.Where(active => active).AsUnitObservable().Share();
    public IObservable<Unit> OnInactive => isActive.Where(active => !active).AsUnitObservable().Share();
    public IObservable<State> OnBegin => beginSubject.Where(_ => isActive.Value).Share();
    public IObservable<State> OnEnd => endSubject.Where(_ => isActive.Value).Share();
    public IObservable<State> OnUpdate => Observable.EveryUpdate().Where(_ => isActive.Value).Select(_ => CurrentState).Share();
    public IObservable<State> OnLateUpdate => Observable.EveryLateUpdate().Where(_ => isActive.Value).Select(_ => CurrentState).Share();
    public IObservable<State> OnFixedUpdate => Observable.EveryFixedUpdate().Where(_ => isActive.Value).Select(_ => CurrentState).Share();
    public IObservable<State> OnEndOfFrame => Observable.EveryEndOfFrame().Where(_ => isActive.Value).Select(_ => CurrentState).Share();

    #endregion

    #region Public Methods

    public void Start(params State[] states)
    {
        isActive.Value = true;
        
        this.states = states.GroupBy(state => state.GetType()).ToDictionary(state => state.Key, state => state.FirstOrDefault());
        this.states.ForEach(kvp => kvp.Value.Initialize(this));
        
        CurrentState = states[0];
        
        OnBegin.Subscribe(state => state.OnBegin()).AddTo();
        OnEnd.Subscribe(state => state.OnEnd()).AddTo();
        OnUpdate.Subscribe(state => state.OnUpdate()).AddTo();
        OnLateUpdate.Subscribe(state => state.OnLateUpdate()).AddTo();
        OnFixedUpdate.Subscribe(state => state.OnFixedUpdate()).AddTo();
        OnEndOfFrame.Subscribe(state => state.OnEndOfFrame()).AddTo();
        
        beginSubject.OnNext(CurrentState);
    }
    
    public void Stop()
    {
        isActive.Value = false;

        CurrentState = null;
    }

    public void Transition(Type targetStateType)
    { 
        endSubject.OnNext(CurrentState);
        
        PrevState = CurrentState;
        CurrentState = states[targetStateType];
        
        beginSubject.OnNext(CurrentState);
    }

    public IObservable<State> OnBeginByState(Type targetStateType) =>
        OnBegin.Where(currentState => currentState.GetType() == targetStateType);
    public IObservable<State> OnEndByState(Type targetStateType) =>
        OnEnd.Where(currentState => currentState.GetType() == targetStateType);
    public IObservable<State> OnUpdateByState(Type targetStateType) =>
        OnUpdate.Where(currentState => currentState.GetType() == targetStateType);
    public IObservable<State> OnLateUpdateByState(Type targetStateType) =>
        OnLateUpdate.Where(currentState => currentState.GetType() == targetStateType);
    public IObservable<State> OnFixedUpdateByState(Type targetStateType) =>
        OnFixedUpdate.Where(currentState => currentState.GetType() == targetStateType);
    public IObservable<State> OnEndOfFrameByState(Type targetStateType) =>
        OnEndOfFrame.Where(currentState => currentState.GetType() == targetStateType);
    
    public async UniTask WaitOnBeginByStateAsync(Type targetStateType, CancellationToken cancellationToken = default) =>
        await OnBeginByState(targetStateType).FirstOrDefault().ToUniTask(cancellationToken:cancellationToken);
    public async UniTask WaitOnEndByStateAsync(Type targetStateType, CancellationToken cancellationToken = default) =>
        await OnEndByState(targetStateType).FirstOrDefault().ToUniTask(cancellationToken:cancellationToken);
    
    

    #endregion
}
