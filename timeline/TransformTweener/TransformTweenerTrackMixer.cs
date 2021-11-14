using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class TransformTweenerTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform ThisTr = playerData as Transform;
        if (ThisTr is null) return;

        //---------------- vars --------------------//
        Vector3? WeightPos = null;
        Quaternion? WeightRot = null;
        Vector3? WeightLocalScale = null;

        Vector3? lastPos  = null;
        //------------------------------------------//
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            //per clip
            ScriptPlayable<TransformTweenerBehaviour> inputPlayable = (ScriptPlayable<TransformTweenerBehaviour>)playable.GetInput(i);
            TransformTweenerBehaviour input = inputPlayable.GetBehaviour();

            TransformTweenerClip.LocatorTRS inputLocatorTrS = input.GetLocatorTRS();

            float perc = (float)i / inputCount;

            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0)
            {
                //pos
                if (WeightPos is null) WeightPos = inputLocatorTrS.pos; //first weighted
                else WeightPos = Vector3.Lerp((Vector3)WeightPos, inputLocatorTrS.pos, inputWeight); //lerp
                //rot
                if (WeightRot is null)
                {
                    WeightRot = inputLocatorTrS.rot; //first weighted
                }
                else
                {
                    WeightRot = NormalizeQuaternion((Quaternion)WeightRot);
                    WeightRot = Quaternion.Lerp((Quaternion)WeightRot, inputLocatorTrS.rot, inputWeight); //lerp
                }
                //localscale
                if (WeightLocalScale is null) WeightLocalScale = inputLocatorTrS.localScale; //first weighted
                else WeightLocalScale = Vector3.Lerp((Vector3)WeightLocalScale, inputLocatorTrS.localScale, inputWeight); //lerp
            }
            ///--------debug------------///
            //draw all pos rails
            if (lastPos != null)
            {
                Debug.DrawLine((Vector3)lastPos, inputLocatorTrS.pos,  Color.Lerp(Color.black, Color.white, perc) * input.debugColor);
            }
            lastPos = inputLocatorTrS.pos;
            //draw all facing
            Debug.DrawRay(inputLocatorTrS.pos, (inputLocatorTrS.rot * Vector3.forward) /2 , Color.red);
            ///--------------------------///
        }
 
        if (WeightPos != null && WeightRot != null)
        {
            //------------ set ----------------//
            ThisTr.position = (Vector3)WeightPos;
            ThisTr.rotation = (Quaternion)WeightRot;
            ThisTr.localScale = (Vector3)WeightLocalScale;
            //draw current pos/ facing
            Debug.DrawRay((Vector3)WeightPos, ((Quaternion)WeightRot * Vector3.forward)/2, Color.blue);
            Debug.DrawRay((Vector3)WeightPos, ((Quaternion)WeightRot * Vector3.up)/2, Color.green);
        }
    }





    //Quaternion check
    static float QuaternionMagnitude(Quaternion rotation)
    {
        return Mathf.Sqrt((Quaternion.Dot(rotation, rotation)));
    }

    static Quaternion NormalizeQuaternion(Quaternion rotation)
    {
        float magnitude = QuaternionMagnitude(rotation);

        if (magnitude > 0f) return rotation;
           // return ScaleQuaternion(rotation, 1f / magnitude);

            //Debug.LogWarning("Cannot normalize a quaternion with zero magnitude.");
        return Quaternion.identity;
    }
}
