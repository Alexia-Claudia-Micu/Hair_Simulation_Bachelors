using UnityEngine;

public class BoneFollower : MonoBehaviour
{
    public Transform targetBone;
    public Transform[] followers;

    private Vector3[] initialOffsets;
    private Quaternion[] initialRotations;

    void Start()
    {
        initialOffsets = new Vector3[followers.Length];
        initialRotations = new Quaternion[followers.Length];

        for (int i = 0; i < followers.Length; i++)
        {
            initialOffsets[i] = followers[i].position - targetBone.position;
            initialRotations[i] = Quaternion.Inverse(targetBone.rotation) * followers[i].rotation;
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < followers.Length; i++)
        {
            followers[i].position = targetBone.position + targetBone.rotation * initialOffsets[i];
            followers[i].rotation = targetBone.rotation * initialRotations[i];
        }
    }
}
