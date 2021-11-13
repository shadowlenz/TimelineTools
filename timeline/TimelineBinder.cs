using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineBinder : MonoBehaviour
{
    [Header("Works properly if PlayableDirector's playOnAwake is set -false-")]
    [Space()]
    public PlayableDirector director;

    public enum AutoGetObj {ThisObj=0, MainCam=1, MainCamCinemaBrain=2, CurrentPlayableActorTr =3}
    [System.Serializable]
    public struct TrackRebinder
    {
        public string trackName;
        public AutoGetObj autoGetObj;
        [Tooltip("obj to bind with")]
        public Object obj;

    }
    public TrackRebinder[] trackRebinders;

    public enum DirectorState { isStopped=0, isPlaying=1, isPaused=2 }
    [Header("debug")]
    [DisplayWithoutEdit] [SerializeField] DirectorState directorState;

    private void OnValidate()
    {
        director = this.GetComponent<PlayableDirector>();
    }

    private void Awake()
    {
        director.played += OnPlay;
        director.paused += OnPause;
        director.stopped += OnStop;
    }

    private void OnDestroy()
    {
        director.played -= OnPlay;
        director.paused -= OnPause;
        director.stopped -= OnStop;
    }


    void OnPlay(PlayableDirector _director)
    {
        if (directorState != DirectorState.isPaused)
        {
            SetTrackBinding();
        }

        directorState = DirectorState.isPlaying;
    }
    void OnPause(PlayableDirector _director)
    {
        directorState = DirectorState.isPaused;
    }
    void OnStop(PlayableDirector _director)
    {
        directorState = DirectorState.isStopped;
    }


    public void SetTrackBinding()
    {
        var timeline = director.playableAsset as TimelineAsset;

        for (int i = 0; i < trackRebinders.Length; i++)
        {
            TrackRebinder ThisRebinder = trackRebinders[i];

            TrackAsset[] GetTrackAssets = timeline.GetOutputTracks().Where(x=> string.Equals(x.name, ThisRebinder.trackName)).ToArray();
            for (int x = 0; x < GetTrackAssets.Length; x++)
            {
                TrackAsset ThisTrack = GetTrackAssets[x];
                Object thisRebindObj = GetRebinderObj(ThisRebinder);

                Debug.Log("Timeline // rebinds : " + ThisTrack.name +" w/ "+ thisRebindObj.name);
                director.SetGenericBinding(ThisTrack, thisRebindObj);
            }
        }
    }


    Object GetRebinderObj(TrackRebinder ThisRebinder)
    {
        /// <summary>
        /// Add custom obj to bind here
        /// </summary>
        switch (ThisRebinder.autoGetObj)
        {
            case AutoGetObj.ThisObj:
                return ThisRebinder.obj;
            case AutoGetObj.MainCam:
                return Camera.main;
            case AutoGetObj.MainCamCinemaBrain:
                return Camera.main.GetComponent<CinemachineBrain>();
            /*
            case AutoGetObj.CurrentPlayableActorTr:
            return GameGlobal.instance.CurrentPlayableActor.sceneActor.transform;
            */
            default:
                return null;
        }
    }
}
