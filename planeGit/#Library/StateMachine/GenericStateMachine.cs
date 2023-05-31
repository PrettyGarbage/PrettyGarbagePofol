using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

///<summary>
///스테이트 머신 기능을 정의한다.
///주의사항으로는 이벤트의 구독을 전부 처리한 후 스테이트 머신을 시작 시킬 것을 권장한다.
///만약 그러지 아니하면 이벤트 구독 전에 이벤트가 발생하는 문제가 생길 수 있다.
///</summary>
///<typeparam name="T"></typeparam>
public class GenericStateMachine<T>
{
    #region Fields

    Subject<T> beginSubject = new();
    Subject<T> endSubject = new();
    BoolReactiveProperty isActive = new (false);
    
    #endregion
    
    #region Properties

    public T CurrentState { get; private set; } 
    public T PrevState { get; private set; }
    
    public bool IsActive => isActive.Value;

    public IObservable<Unit> OnActive => isActive.Where(active => active).AsUnitObservable().Share();
    public IObservable<Unit> OnInactive => isActive.Where(active => !active).AsUnitObservable().Share();
    public IObservable<T> OnBegin => beginSubject.Where(_ => isActive.Value).Share();
    public IObservable<T> OnEnd => endSubject.Where(_ => isActive.Value).Share();
    public IObservable<T> OnUpdate => Observable.EveryUpdate().Where(_ => isActive.Value).Select(_ => CurrentState).Share();
    public IObservable<T> OnLateUpdate => Observable.EveryLateUpdate().Where(_ => isActive.Value).Select(_ => CurrentState).Share();
    public IObservable<T> OnFixedUpdate => Observable.EveryFixedUpdate().Where(_ => isActive.Value).Select(_ => CurrentState).Share();
    public IObservable<T> OnEndOfFrame => Observable.EveryEndOfFrame().Where(_ => isActive.Value).Select(_ => CurrentState).Share();

    #endregion

    #region Public Methods

    public void Start(T initState)
    {
        isActive.Value = true;
        
        CurrentState = initState;
        
        beginSubject.OnNext(CurrentState);
    }
    
    public void Stop() => isActive.Value = false;

    public void Transition(T nextState)
    { 
        endSubject.OnNext(CurrentState);
        
        PrevState = CurrentState;
        CurrentState = nextState;
        
        beginSubject.OnNext(CurrentState);
    }

    //Generic으로 선언된 Enum을 boxing 없이 비교 -> EqualityComparer<T>.Default.Equals(TEnum1, TEnum2)  
    //https://stackoverflow.com/questions/29929488/compare-two-system-enum-of-type-t
    public IObservable<T> OnBeginByState(T targetState) =>
        OnBegin.Where(state => EqualityComparer<T>.Default.Equals(state, targetState));
    public IObservable<T> OnEndByState(T targetState) =>
        OnEnd.Where(state => EqualityComparer<T>.Default.Equals(state, targetState));
    public IObservable<T> OnUpdateByState(T targetState) =>
        OnUpdate.Where(state => EqualityComparer<T>.Default.Equals(state, targetState));
    public IObservable<T> OnLateUpdateByState(T targetState) =>
        OnLateUpdate.Where(state => EqualityComparer<T>.Default.Equals(state, targetState));
    public IObservable<T> OnFixedUpdateByState(T targetState) =>
        OnFixedUpdate.Where(state => EqualityComparer<T>.Default.Equals(state, targetState));
    public IObservable<T> OnEndOfFrameByState(T targetState) =>
        OnEndOfFrame.Where(state => EqualityComparer<T>.Default.Equals(state, targetState));
    
    public async UniTask WaitOnBeginByStateAsync(T targetState, CancellationToken cancellationToken = default) =>
        await OnBeginByState(targetState).FirstOrDefault().ToUniTask(cancellationToken:cancellationToken);
    public async UniTask WaitOnEndByStateAsync(T targetState, CancellationToken cancellationToken = default) =>
        await OnEndByState(targetState).FirstOrDefault().ToUniTask(cancellationToken:cancellationToken);

    #endregion


}
