using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Bone
{
    public int BoneID; //make sure to remeber what number each bone is.
    public Vector3 BonePositions;
    public Quaternion BoneRotations;
    public Vector3 BoneVelocities;
}
