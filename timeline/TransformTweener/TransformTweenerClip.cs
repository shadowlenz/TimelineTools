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

    //----------------------LocatorTRS-----------------------//
    [System.Serializable]
    public struct LocatorTRS
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 localScale;
        public static LocatorTRS DefaultVal()
        {
            return new LocatorTRS()
            {
                pos = Vector3.zero,
                rot = Quaternion.identity,
                localScale = Vector3.one
            };
        }
    }

    public LocatorTRS manualTRS = LocatorTRS.DefaultVal();
    //--------------------------------------------------------//
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
        transformMoveBehaviour.manualTRS = manualTRS;
        transformMoveBehaviour.localOffsetTr = localTr;
  

        transformMoveBehaviour.debugColor = debugColor;


        return playable;
    }

}
