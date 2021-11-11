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

        Vector3? lastPos  = null;
        //------------------------------------------//
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            //per clip
            ScriptPlayable<TransformTweenerBehaviour> inputPlayable = (ScriptPlayable<TransformTweenerBehaviour>)playable.GetInput(i);
            TransformTweenerBehaviour input = inputPlayable.GetBehaviour();

            TransformTweenerBehaviour.LocatorTransform inputLocatorTr = input.GetLocatorTransform();

            float perc = (float)i / inputCount;

            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0)
            {
                if (WeightPos is null) WeightPos = inputLocatorTr.pos; //first weighted
                else WeightPos = Vector3.Lerp((Vector3)WeightPos, inputLocatorTr.pos, inputWeight); //lerp
                if (WeightRot is null)
                {
                    WeightRot = inputLocatorTr.rot; //first weighted
                }
                else
                {
                    WeightRot = NormalizeQuaternion((Quaternion)WeightRot);
                    WeightRot = Quaternion.Lerp((Quaternion)WeightRot, inputLocatorTr.rot, inputWeight); //lerp
                }
            }
            //draw all pos rails
            if (lastPos != null)
            {
                Debug.DrawLine((Vector3)lastPos, inputLocatorTr.pos,  Color.Lerp(Color.black, Color.white, perc) * input.debugColor);
            }
            lastPos = inputLocatorTr.pos;
            //draw all facing
            Debug.DrawRay(inputLocatorTr.pos, (inputLocatorTr.rot * Vector3.forward) * 0.2f, Color.red);
        }
 
        if (WeightPos != null && WeightRot != null)
        {
            //------------ set ----------------//
            ThisTr.position = (Vector3)WeightPos;
            ThisTr.rotation = (Quaternion)WeightRot;
            //draw current pos/ facing
            Debug.DrawRay((Vector3)WeightPos, (Quaternion)WeightRot * Vector3.forward * 0.2f, Color.blue);
            Debug.DrawRay((Vector3)WeightPos, (Quaternion)WeightRot * Vector3.up * 0.2f, Color.green);
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
