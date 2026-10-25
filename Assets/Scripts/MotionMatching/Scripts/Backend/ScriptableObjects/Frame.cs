using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Frame
{
    public int AnimationClipNumber;
    //public int FrameNumber;
    public Vector3 RootPosition;
    public Tags Tag;
    public Quaternion CurrentDirection;
    public List<Vector3> FutureTrejectoryPositions = new List<Vector3>();
    public List<Quaternion> FutureTrejectoryRotations = new List<Quaternion>();
    public List<Bone> BoneData = new List<Bone>();
}
public enum Tags { Walk, Run };