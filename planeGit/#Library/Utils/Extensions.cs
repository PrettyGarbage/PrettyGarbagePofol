using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class Extensions
{
    #region Transform Extensions

    //transform Initialize
    public static void InitializeTransform(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    //transform change X Value
    public static void SetPosX(this Transform transform, float x)
    {
        var position = transform.position;
        position = new Vector3(x, position.y, position.z);
        transform.position = position;
    }

    //transform change X Value
    public static void SetPosY(this Transform trans, float y)
    {
        var position = trans.position;
        var newPos = new Vector3(position.x, y, position.z);
        trans.position = newPos;
    }

    //transform change X Value
    public static void SetPosZ(this Transform trans, float z)
    {
        var position = trans.position;
        var newPos = new Vector3(position.x, position.y, z);
        trans.position = newPos;
    }

    #endregion

    #region Vector3 Extensions

    public static Vector3 SetX(this Vector3 vec, float x) => new(x, vec.y, vec.z);
    public static Vector3 SetY(this Vector3 vec, float y) => new(vec.x, y, vec.z);
    public static Vector3 SetZ(this Vector3 vec, float z) => new(vec.x, vec.y, z);
    public static Vector2 SetX(this Vector2 vec, float x) => new(x, vec.y);
    public static Vector2 SetY(this Vector2 vec, float y) => new(vec.x, y);
    public static Vector3 DropX(this Vector3 vec) => new(0, vec.y, vec.z);
    public static Vector3 DropY(this Vector3 vec) => new(vec.x, 0, vec.z);
    public static Vector3 DropZ(this Vector3 vec) => new(vec.x, vec.y, 0);
    public static Vector2 DropX(this Vector2 vec) => new(0, vec.y);
    public static Vector2 DropY(this Vector2 vec) => new(vec.x, 0);
    public static Vector3 XYToXZ(this Vector2 position) => new(position.x, 0, position.y);
    public static Vector2 XZToXY(this Vector3 position) => new(position.x, position.z);

    public static Vector3Int ToVector3Int(this Vector3 position) =>
        new((int)position.x, (int)position.y, (int)position.z);

    public static Vector2Int ToVector2Int(this Vector2 position) => new((int)position.x, (int)position.y);

    //두 Vector3 간에 sqrMagnitude를 이용한 거리 구하기
    public static float DistanceSqr(this Vector3 position, Vector3 otherPosition) =>
        (position - otherPosition).sqrMagnitude;

    //두 Vector3 간에 sqrMagnitude를 이용하여 거리를 구하고 거리가 지정된 거리보다 작으면 true를 반환한다.
    public static bool IsClose(this Vector3 position, Vector3 otherPosition, float distance) =>
        position.DistanceSqr(otherPosition) < distance * distance;

    #endregion

    #region Animator Extensions

    ///<summary>
    ///현재 재생중인 애니메이션이 종료하였는가?
    ///</summary>
    ///<param name="self">애니메이터 자신</param>
    ///<returns>애니메이션 종료되었는지 여부</returns>
    public static bool IsCompleted(this Animator self)
    {
        return self.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - self.GetAnimatorTransitionInfo(0).duration;
    }

    ///<summary>
    ///현재 재생중인 애니메이션이 지정한 스테이트에서 종료되었는지 확인
    ///</summary>
    ///<param name="self">애니메이터 자신</param>
    ///<param name="stateHash">설정 스테이트의 해쉬 </param>
    ///<returns>지정된 해쉬 도달 여부</returns>
    public static bool IsCompleted(this Animator self, int stateHash)
    {
        return self.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash && self.IsCompleted();
    }

    ///<summary>
    ///현재 재생중인 애니메이션 지정비율을 지나쳤는가? normalizeTime이기 떄문에
    ///비율로 생각해야함.
    ///</summary>
    ///<param name="self">애니메이터 자신</param>
    ///<param name="normalizeTime">지정 비율 시간</param>
    ///<returns>애니메이션이 현재 지정된 구간을 지나가는지 여부</returns>
    public static bool IsPassed(this Animator self, float normalizeTime)
    {
        return self.GetCurrentAnimatorStateInfo(0).normalizedTime > normalizeTime;
    }

    ///<summary>
    ///애니메이션을 최초부터 재생
    ///</summary>
    ///<param name="self">애니메이터 자신</param>
    ///<param name="shortNameHash">애니메이션의 해쉬</param>
    public static void PlayBegin(this Animator self, int shortNameHash)
    {
        self.Play(shortNameHash, 0, 0.0f);
    }

    #endregion

    #region Linq Extensions

    //모든 요소에 접근하는 기능을 Linq에 추가
    public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection, Action<TSource> action)
    {
        for (int i = collection.Count() - 1; i >= 0; i--)
        {
            action(collection.ElementAtOrDefault(i));
        }

        return collection;
    }

    //모든 요소에 접근 하는 기능을 Linq에 추가(index 포함)
    public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> collection,
        Action<TSource, int> action)
    {
        for (int i = 0; i < collection.Count(); i++)
        {
            action(collection.ElementAtOrDefault(i), i);
        }

        return collection;
    }

    //랜덤으로 하나 선택하는 기능을 Linq에 추가
    public static TSource Choose<TSource>(this IEnumerable<TSource> collection)
    {
        return collection.ElementAtOrDefault(Random.Range(0, collection.Count()));
    }

    //가중치 배열을 활용해 랜덤으로 하나 선택하는 기능을 Linq에 추가
    public static TSource Choose<TSource>(this IEnumerable<TSource> collection, params float[] weights)
    {
        collection = collection.Take(weights.Length);
        var total = weights.Take(collection.Count()).Sum();
        var rand = Random.Range(0.0f, total);

        for (int i = 0; i < collection.Count(); i++)
        {
            if (rand < weights[i]) return collection.ElementAtOrDefault(i);
            else rand -= weights[i];
        }

        return collection.LastOrDefault();
    }

    //Animation Curve를 활용해 가중치 배열을 만들어 랜덤으로 하나 선택하는 기능을 Linq에 추가
    public static TSource Choose<TSource>(this IEnumerable<TSource> collection, AnimationCurve curve)
    {
        var weights = new float[collection.Count()];
        for (int i = 0; i < collection.Count(); i++)
        {
            weights[i] = curve.Evaluate(i / (float)collection.Count());
        }

        return collection.Choose(weights);
    }

    //중복 없이 여러개의 요소를 랜덤으로 선택하는 기능을 Linq에 추가
    public static IEnumerable<TSource> ChooseSet<TSource>(this IEnumerable<TSource> collection, int count)
    {
        for (int numLeft = collection.Count(); numLeft > 0; numLeft--)
        {
            float prob = (float)count / (float)numLeft;

            if (Random.value <= prob)
            {
                count--;
                yield return collection.ElementAtOrDefault(numLeft - 1);

                if (count == 0) break;
            }
        }
    }

    //랜덤으로 섞기 기능을 Linq에 추가
    public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> collection)
    {
        var list = collection.ToList();
        for (int i = 0; i < list.Count(); i++)
        {
            int j = Random.Range(0, list.Count());
            yield return list[j];
            list[j] = list[i];
        }
    }

    //가장 가까운 오브젝트 찾기 기능을 Linq에 추가
    public static TSource Closet<TSource>(this IEnumerable<TSource> collection, Vector3 position)
        where TSource : MonoBehaviour
    {
        var closest = collection.FirstOrDefault();
        float closestDistance = float.MaxValue;
        foreach (var item in collection)
        {
            float distance = Vector3.Distance(position, item.transform.position);
            if (distance < closestDistance)
            {
                closest = item;
                closestDistance = distance;
            }
        }

        return closest;
    }

    //Print all elements of collection using Linq
    //컬렉션에 대하여 모든 요소를 한 줄로 출력하는 기능을 Linq에 추가
    public static void PrintCollection<TSource>(this IEnumerable<TSource> collection)
    {
        Logger.Log($"[{string.Join(", ", collection.Select(x => x.ToString()))}]");
    }

    public static string CollectionToString<TSource>(this IEnumerable<TSource> collection)
    {
        return $"[{string.Join(", ", collection.Select(x => x.ToString()))}]";
    }

    //컬렉션에 대하여 주어진 기준에 따른 가장 큰 요소를 반환하는 기능을 Linq에 추가
    public static TSource MaxBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector)
        where TResult : IComparable<TResult>
    {
        return collection.Aggregate((x, y) => selector(x).CompareTo(selector(y)) > 0 ? x : y);
    }

    //컬렉션에 대하여 주어진 기준에 따른 가장 작은 요소를 반환하는 기능을 Linq에 추가
    public static TSource MinBy<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector)
        where TResult : IComparable<TResult>
    {
        return collection.Aggregate((x, y) => selector(x).CompareTo(selector(y)) < 0 ? x : y);
    }

    //컬렉션에 대하여 주어진 조건에 해당하는 요소가 없다면 추가하는 기능을 Linq에 추가
    public static IEnumerable<TSource> AddIfNotExists<TSource>(this IEnumerable<TSource> collection, TSource element, Func<TSource, bool> predicate)
    {
        if (collection is null) return null;
        return collection.Any(predicate) ? collection : collection.Append(element);
    }

    //컬렉션에 대하여 주어진 조건에 해당하는 요소가 없다면 추가하고 있다면 갱신하는 기능을 Linq에 추가
    public static IEnumerable<TSource> AddOrUpdate<TSource>(this IEnumerable<TSource> collection, TSource element, Func<TSource, bool> predicate)
    {
        if (collection.Any(predicate)) return collection.Select(x => predicate(x) ? element : x);
        return collection.Append(element);
    }

    public static bool SameAll<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> selector)
    {
        IEnumerable<TResult> select = collection.Select(selector);
        return select.All(x => x != null ? x.Equals(select.FirstOrDefault()) : select.FirstOrDefault() == null);
    }

    public static bool SameAll<TSource>(this IEnumerable<TSource> collection)
    {
        return collection.All(x => x != null ? x.Equals(collection.FirstOrDefault()) : collection.FirstOrDefault() == null);
    }

    #endregion

    #region UniRx Extensions

    //OnMouseDownAsObservable과 OnMouseUpAsObservable을 이용하여 객체에 대한 클릭을 감지하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnMouseClickAsObservable(this Component component)
    {
        return component.OnMouseDownAsObservable().SelectMany(_ => component.OnMouseUpAsObservable()).FirstOrDefault().RepeatUntilDestroy(component).Share();
    }

    //일정 시간 내 객체의 더블 클릭하는 것을 감지하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnMouseDoubleClickAsObservable(this Component component, float interval = 0.5f)
    {
        return component.OnMouseClickAsObservable().Buffer(TimeSpan.FromSeconds(interval)).Where(xs => xs.Count >= 2)
            .AsUnitObservable().Share();
    }

    //객체를 클릭하고 있는 중인 것을 감지하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnLongClickAsObservable(this Component component)
    {
        return Observable.EveryUpdate().SkipUntil(component.OnMouseDownAsObservable())
            .TakeUntil(component.OnMouseUpAsObservable()).RepeatUntilDestroy(component).AsUnitObservable().Share();
    }

    //일정 시간 내 버튼의 Double Click을 감지하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnDoubleClickAsObservable(this Button button, float interval = 0.5f)
    {
        return button.OnClickAsObservable().Buffer(TimeSpan.FromSeconds(interval)).Where(xs => xs.Count >= 2)
            .AsUnitObservable().Share();
    }

    //버튼을 클릭하고 있는 중인 것을 감지하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnLongClickAsObservable(this Button button)
    {
        return Observable.EveryUpdate().SkipUntil(button.OnPointerDownAsObservable())
            .TakeUntil(button.OnPointerUpAsObservable()).RepeatUntilDestroy(button).AsUnitObservable().Share();
    }

    //현재 진행 중인 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator)
    {
        return Observable.EveryUpdate().AsUnitObservable().FirstOrDefault(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration);
    }

    //지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, string animationName)
    {
        return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)).Where(
                _ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=
                     1.0f - animator.GetAnimatorTransitionInfo(0).duration)
            .AsUnitObservable().FirstOrDefault();
    }

    //지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, int animationHash)
    {
        return Observable.EveryUpdate()
            .Where(_ => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animationHash).Where(_ =>
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=
                1.0f - animator.GetAnimatorTransitionInfo(0).duration)
            .AsUnitObservable().FirstOrDefault();
    }

    //사운드 재생이 종료 시 이벤트를 호출하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnAudioCompleteAsObservable(this AudioSource audioSource)
    {
        return Observable.EveryUpdate().Where(_ => !audioSource.isPlaying).AsUnitObservable().FirstOrDefault();
    }

    //파티클 재생 종료 시 이벤트를 호출하는 기능을 UniRx에 추가
    public static IObservable<Unit> OnParticleCompleteAsObservable(this ParticleSystem particleSystem)
    {
        return Observable.EveryUpdate().Where(_ => !particleSystem.IsAlive()).AsUnitObservable().FirstOrDefault();
    }

    public static void AddTo(this IDisposable disposable) => disposable.AddTo(SceneLife.Instance);

    #endregion

    #region UniTask Extensions

    public static async UniTask WaitAnimationCompleteAsync(this Animator animator, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration, cancellationToken: cancellationToken);
    }

    public static async UniTask WaitAnimationCompleteAsync(this Animator animator, string animationName, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration, cancellationToken: cancellationToken);
    }

    public static async UniTask WaitAnimationCompleteAsync(this Animator animator, int animationHash, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animationHash && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration, cancellationToken: cancellationToken);
    }

    public static async UniTask WaitAnimationCompleteAsync(this Animation animation, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => !animation.isPlaying, cancellationToken: cancellationToken);
    }

    public static async UniTask WaitAudioCompleteAsync(this AudioSource audioSource, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => !audioSource.isPlaying, cancellationToken: cancellationToken);
    }

    public static async UniTask WaitParticleCompleteAsync(this ParticleSystem particleSystem, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => !particleSystem.IsAlive(), cancellationToken: cancellationToken);
    }

    public static async UniTask PlayAsync(this PlayableDirector director, CancellationToken cancellationToken = default)
    {
        try
        {
            if(ConfigModel.Instance.Setting.debugMode) director.IsValid();
            director.Play();
            await UniTask.WaitUntil(() => director.state == PlayState.Paused, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static async UniTask WaitPauseAsync(this PlayableDirector director, CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => director.state == PlayState.Paused, cancellationToken: cancellationToken);
    }
    
    public static void IsValid(this PlayableDirector director)
    {
        var playableAsset = director.playableAsset;

        // Loop through the bindings
        foreach (var output in playableAsset.outputs)
        {
            if(output.sourceObject)
            {
                // Get the binding
                var binding = director.GetGenericBinding(output.sourceObject);
                if (!binding && output.sourceObject is not AudioTrack) Debug.LogError($"{director.name} Binding {output.sourceObject.name} is null");
            }
        }
    }

    public static UniTask AddTo(this UniTask task) => task.AttachExternalCancellation(SceneLife.Instance.GetCancellationTokenOnDestroy());
    public static UniTask AddTo(this UniTask task, GameObject gameObject) => task.AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
    public static UniTask<T> AddTo<T>(this UniTask<T> task) => task.AttachExternalCancellation(SceneLife.Instance.GetCancellationTokenOnDestroy());
    public static UniTask<T> AddTo<T>(this UniTask<T> task, GameObject gameObject) => task.AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());

    public static CancellationToken GetCancellationTokenOnDisable(this GameObject gameObject)
    {
        var cts = new CancellationTokenSource();
        gameObject.OnDisableAsObservable().Merge(gameObject.OnDestroyAsObservable()).FirstOrDefault().Subscribe(_ =>
        {
            cts.Cancel();
            cts.Dispose();
        });

        return cts.Token;
    }

    public static CancellationToken GetCancellationTokenOnDisable(this MonoBehaviour mono) => GetCancellationTokenOnDisable(mono.gameObject);

    #endregion

    #region PlayableDirector Extensions

    #endregion

    #region String Extension

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        => enumerable == null || !enumerable.Any();

    #endregion
}