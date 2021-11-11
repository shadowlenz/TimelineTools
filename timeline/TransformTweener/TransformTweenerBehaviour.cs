using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TransformTweenerBehaviour : PlayableBehaviour
{
    //from PlayableAsset
    public Transform locationTr;
    public Transform lookAtTr;
    public LocatorTransform manualTr =
        new LocatorTransform()
        {
            pos = Vector3.zero,
            rot = Quaternion.identity
        };

    //from TrackAsset
    [System.NonSerialized]
    public Transform localOffsetTr;
    [System.NonSerialized]
    public Color debugColor = Color.white;
    //

    [System.Serializable]
    public struct LocatorTransform
    {
        public Vector3 pos;
        public Quaternion rot;
    }
    /// <summary>
    /// get pos and rot of this behaviour locator
    /// </summary>
    public LocatorTransform GetLocatorTransform()
    {
        LocatorTransform posRot;

        if (locationTr != null)
        {
            //use locationTr
            posRot.pos =  locationTr.position;
            posRot.rot =  locationTr.rotation;
        }
        else
        {
            //use manualTr and add local offset
            Vector3 offsetPos = (localOffsetTr != null) ? localOffsetTr.position : Vector3.zero;
            Quaternion offsetRot = (localOffsetTr != null) ? localOffsetTr.rotation : Quaternion.identity;

            posRot.pos =  (manualTr.pos + offsetPos);
            posRot.rot =  (manualTr.rot * offsetRot);
        }

        if (lookAtTr != null)
        {
            //override rot to face lookAtTr 
            posRot.rot = Quaternion.LookRotation((lookAtTr.position- posRot.pos).normalized);
        }

        return posRot;
    }

}
