using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Cinemachine.Timeline
{
    #pragma warning disable CS0618 // Тип или член устарел
    [TrackMediaType(TimelineAsset.MediaType.Script)]
    #pragma warning restore CS0618 // Тип или член устарел
    [Serializable]
    [TrackClipType(typeof(CinemachineShot))]
    [TrackBindingType(typeof(CinemachineBrain))]
    [TrackColor(0.53f, 0.0f, 0.08f)]
    public class CinemachineTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(
            PlayableGraph graph, GameObject go, int inputCount)
        {
            // Hack to set the display name of the clip to match the vcam
            foreach (var c in GetClips())
            {
                CinemachineShot shot = (CinemachineShot)c.asset;
                CinemachineVirtualCameraBase vcam = shot.VirtualCamera.Resolve(graph.GetResolver());
                if (vcam != null)
                    c.displayName = vcam.Name;
            }

            var mixer = ScriptPlayable<CinemachineMixer>.Create(graph);
            mixer.SetInputCount(inputCount);
            return mixer;
        }
    }
}
