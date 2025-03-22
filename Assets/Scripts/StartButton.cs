using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Listener);
    }

    private void OnEnable()
    {
        GameManager.AddDataUpdateListener(OnDataUpdate);
    }

    private void OnDisable()
    {
        GameManager.RemoveDataUpdateListener(OnDataUpdate);
    }

    private void OnDataUpdate()
    {
        switch (GameManager.Instance?.State)
        {
            case GameManager.GameState.Round:
            case GameManager.GameState.Collect:
                button.interactable = false;
                break;
            case GameManager.GameState.Start:
            case GameManager.GameState.End:
                button.interactable = true;
                break;
        }
    }

    private void Listener()
    {
        GameManager.Instance?.StartGame();
    }
}
