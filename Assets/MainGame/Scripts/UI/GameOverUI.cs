using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static event System.Action OnPlayPressed;
    [SerializeField] private Image gameOverBox;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TMPro.TMP_Text score;
    [SerializeField] private TMPro.TMP_Text coins;

    private void Awake()
    {
        Debug.Log("duoc clone");
        if(gameOverBox)
            gameOverBox.gameObject.SetActive(true);
        this.closeBtn.onClick.AddListener(HandleCloseButton);
    }

    private void Start()
    {
        EventManager.Instance.OnPlayerCollided += Open;
    }
    public void Open(GameObject gameObject)
    {
        UIManager.Instance.ShowUI(UIName.GameOver);
    }

    public void HandleCloseButton()
    {
        OnPlayPressed?.Invoke();
        UIManager.Instance.HideUI(UIName.GameOver);
        UIManager.Instance.ShowUI(UIName.GameplayUI);
    }
}
