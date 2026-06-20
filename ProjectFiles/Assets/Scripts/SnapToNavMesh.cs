using UnityEngine;

// Editor��Ŏ��s�FNavMesh��ɔz�u
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
