using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DataSet", menuName = "Scriptable Objects/DefaultDataSet")]
public class DataSet : ScriptableObject
{
    public List<AnimationClipInfo> AnimationClips = new List<AnimationClipInfo>();
    public List<BoneObjectInfo> Bones = new List<BoneObjectInfo>();
    public List<Frame> Frames = new List<Frame>();
}

[System.Serializable]
public class BoneObjectInfo
{
    public string BoneName;
    [HideInInspector]public int BoneID;
    //public GameObject BoneObject;
}

[System.Serializable]
public class AnimationClipInfo
{
    public AnimationClip Clip;
    public int FrameAmount;
}