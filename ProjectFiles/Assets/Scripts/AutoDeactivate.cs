using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    [Header("何秒後に非アクティブにするか")]
    public float deactivateDelay = 1f;

    private void OnEnable()
    {
        Invoke(nameof(DeactivateSelf), deactivateDelay);
    }

    private void OnDisable()
    {
        // 二重にInvokeしないようにキャンセル
        CancelInvoke(nameof(DeactivateSelf));
    }

    private void DeactivateSelf()
    {
        gameObject.SetActive(false);
    }
}
