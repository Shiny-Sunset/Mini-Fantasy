using UnityEngine;

// Editor上で実行：NavMesh上に配置
[ExecuteInEditMode]
public class SnapToNavMesh : MonoBehaviour
{
    void Update()
    {
        if (!Application.isPlaying)
        {
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out var hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
        }
    }
}
