using UnityEngine;

public class HandController : MonoBehaviour
{
    public SkinnedMeshRenderer handMesh;
    public string blendShapeName = "GrabPose_R";

    private int blendShapeIndex;

    void Start()
    {
        Mesh mesh = handMesh.sharedMesh;
        blendShapeIndex = -1;

        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            if (mesh.GetBlendShapeName(i) == blendShapeName)
            {
                blendShapeIndex = i;
                break;
            }
        }

        if (blendShapeIndex == -1)
        {
            Debug.LogError("BlendShape not found: " + blendShapeName);
        }
    }

    public void Grab()
    {
        if (blendShapeIndex != -1)
            handMesh.SetBlendShapeWeight(blendShapeIndex, 100f);
    }

    public void Release()
    {
        if (blendShapeIndex != -1)
            handMesh.SetBlendShapeWeight(blendShapeIndex, 0f);
    }
}
