using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TransformTweenerBehaviour : PlayableBehaviour
{
    //from PlayableAsset
    public Transform locationTr;
    public Transform lookAtTr;
    public TransformTweenerClip.LocatorTRS manualTRS = TransformTweenerClip.DefaultVal();

    //from TrackAsset
    [System.NonSerialized]
    public Transform localOffsetTr;
    [System.NonSerialized]
    public Color debugColor = Color.white;
    //


    /// <summary>
    /// get pos and rot of this behaviour locator
    /// </summary>
    public TransformTweenerClip.LocatorTRS GetLocatorTRS()
    {
        TransformTweenerClip.LocatorTRS posRot;

        if (locationTr != null)
        {
            //use locationTr
            posRot.pos =  locationTr.position;
            posRot.rot =  locationTr.rotation;
            posRot.localScale = locationTr.localScale;
        }
        else
        {
            //use manualTr and add local offset
            Vector3 offsetPos = (localOffsetTr != null) ? localOffsetTr.position : Vector3.zero;
            Quaternion offsetRot = (localOffsetTr != null) ? localOffsetTr.rotation : Quaternion.identity;
            Vector3 offsetScale = (localOffsetTr != null) ? localOffsetTr.localScale : Vector3.one;

            posRot.pos =  (manualTRS.pos + offsetPos);
            posRot.rot =  (manualTRS.rot * offsetRot);
            posRot.localScale = Vector3.Scale(manualTRS.localScale , offsetScale);
        }

        if (lookAtTr != null)
        {
            //override rot to face lookAtTr 
            posRot.rot = Quaternion.LookRotation((lookAtTr.position- posRot.pos).normalized);
        }

        return posRot;
    }

}
