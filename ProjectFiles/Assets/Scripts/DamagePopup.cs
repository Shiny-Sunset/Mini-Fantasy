using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveUpSpeed = 20f;
    public float disappearSpeed = 2f;

    private TextMeshProUGUI textMesh;
    private Color textColor;
    private float disappearTimer;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textColor = textMesh.color;
    }

    public void Setup(float damageAmount)
    {
        textMesh.text = damageAmount.ToString();
        disappearTimer = 1f;
    }

    void Update()
    {
        transform.position += new Vector3(0, moveUpSpeed * Time.deltaTime, 0);

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0f)
                Destroy(gameObject);
        }
    }
}
