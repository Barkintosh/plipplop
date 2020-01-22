﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

[TrackColor(0.4448276f, 0f, 1f)]
[TrackClipType(typeof(SpielbergClip))]
//[TrackBindingType(typeof(GameObject))]
public class SpielbergTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<SpielbergClipBehaviour>.Create(graph, inputCount);
        UpdateClipNames();
        return scriptPlayable;
    }

    void UpdateClipNames()
    {
        foreach (var clip in GetClips()) {
            var ev = clip.asset as SpielbergClip;

            clip.displayName = "{0} {1}".Format(ev.behaviour.action, string.Join(" ",
                ev.behaviour.args
                .Where(o => { return o.Length > 0;})
                /*
                .Select(o => { return (object)o; })
                .Concat(ev.behaviour.refArgs)
                .Where(o => { return o != null; })
                */
            ));
        }
    }
}