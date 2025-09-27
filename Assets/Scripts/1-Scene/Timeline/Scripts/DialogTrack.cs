using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

// 加入特性，这个轨道上只允许放 DialogClip
[TrackClipType(typeof(DialogClip))]
public class DialogTrack : TrackAsset { }
