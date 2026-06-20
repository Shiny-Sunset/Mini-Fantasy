using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gameClearUI;

    private bool isCleared = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCleared) return;

        if (other.CompareTag("Player"))
        {
            isCleared = true;
            if (gameClearUI != null)
                gameClearUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
