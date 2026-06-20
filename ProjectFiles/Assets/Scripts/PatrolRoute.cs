using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public Transform[] GetPoints()
    {
        // 自身の子要素を全て返す（自分自身は含めない）
        Transform[] children = GetComponentsInChildren<Transform>();
        Transform[] points = new Transform[children.Length - 1];
        for (int i = 1; i < children.Length; i++)
        {
            points[i - 1] = children[i];
        }
        return points;
    }
}
