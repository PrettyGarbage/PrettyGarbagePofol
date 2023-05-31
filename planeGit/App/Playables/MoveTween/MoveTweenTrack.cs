using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.2775454f, 0.5208482f, 0.764151f)]
[TrackClipType(typeof(MoveTweenClip))]
[TrackBindingType(typeof(Transform))]
public class MoveTweenTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var clips = GetClips();
        foreach (var clip in clips)
        {
            var moveTweenClip = clip.asset as MoveTweenClip;
            moveTweenClip.clipPassthrough = clip;
        }
        
        return ScriptPlayable<MoveTweenMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        var comp = director.GetGenericBinding(this) as Transform;
        if (comp == null)
            return;
        var so = new UnityEditor.SerializedObject(comp);
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.hasVisibleChildren)
                continue;
            driver.AddFromName<Transform>(comp.gameObject, iter.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}
