using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider hpSlider;
    public PlayerStatus playerStatus;

    void Start()
    {
        hpSlider.maxValue = playerStatus.maxHP;
        hpSlider.value = playerStatus.currentHP;
    }

    void Update()
    {
        hpSlider.value = playerStatus.currentHP;
        hpSlider.fillRect.gameObject.SetActive(hpSlider.value > 0f);
    }
}
