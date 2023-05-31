using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MoveTweenClip : PlayableAsset, ITimelineClipAsset
{
    [NonSerialized] public TimelineClip clipPassthrough = null;
    public MoveTweenBehaviour template = new MoveTweenBehaviour ();
    public ExposedReference<MoveTweenModel> model;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        template.clip = clipPassthrough;
        var playable = ScriptPlayable<MoveTweenBehaviour>.Create (graph, template);
        MoveTweenBehaviour clone = playable.GetBehaviour ();
        clone.model = model.Resolve (graph.GetResolver ());
        return playable;
    }
}
