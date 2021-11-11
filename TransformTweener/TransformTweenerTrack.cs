using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[TrackBindingType(typeof(Transform))]
[TrackClipType(typeof(TransformTweenerClip))]

public class TransformTweenerTrack : TrackAsset
{
    public Color debugColor = Color.white;
    public bool isLocal;
    [Tooltip("uses this playable transform if empty. (isLocal) needs to be set true.")]
    public ExposedReference<Transform> localOffsetTr;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        IEnumerable<TimelineClip> clips = GetClips();
        foreach (TimelineClip clip in clips)
        {
            TransformTweenerClip ThisTransformClip = (TransformTweenerClip)clip.asset;
            ThisTransformClip.debugColor = debugColor;
            if (isLocal)
            {
                ThisTransformClip.localTr = localOffsetTr.Resolve(graph.GetResolver()) != null ? localOffsetTr.Resolve(graph.GetResolver()) : go.transform;
            }
            else
            {
                ThisTransformClip.localTr = null;
            }
        }

        return ScriptPlayable<TransformTweenerTrackMixer>.Create(graph, inputCount);
    }
    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        //cause of remembering Transforms after deselecting timeline in editor
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
