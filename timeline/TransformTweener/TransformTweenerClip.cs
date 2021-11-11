using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[System.Serializable]
public class TransformTweenerClip : PlayableAsset
{
    public TransformTweenerBehaviour template = new TransformTweenerBehaviour();

    public ExposedReference<Transform> locationTr;
    public ExposedReference<Transform> lookAtTr;
    public TransformTweenerBehaviour.LocatorTransform manualTr;

    //from TrackAsset
    [System.NonSerialized]
    public Color debugColor = Color.white;
    [System.NonSerialized]
    public Transform localTr;
    //

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<TransformTweenerBehaviour>.Create(graph, template);

        TransformTweenerBehaviour transformMoveBehaviour = playable.GetBehaviour();
        transformMoveBehaviour.locationTr =  locationTr.Resolve(graph.GetResolver());
        transformMoveBehaviour.lookAtTr = lookAtTr.Resolve(graph.GetResolver());
        transformMoveBehaviour.manualTr = manualTr;
        transformMoveBehaviour.localOffsetTr = localTr;
  

        transformMoveBehaviour.debugColor = debugColor;


        return playable;
    }

}
