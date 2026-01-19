using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private string prefix = "x ";

    private void Awake()
    {
        if (livesText == null)
            livesText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (LifeManager.Instance == null) return;

        livesText.text = prefix + LifeManager.Instance.Lives;
    }
}
