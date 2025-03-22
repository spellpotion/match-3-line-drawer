using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextUpdater : MonoBehaviour
{
    private enum DataType { ScoreTotal, RoundsRemaining }

    [SerializeField] private DataType dataType;

    private Text text;

    private void OnEnable()
    {
        GameManager.AddDataUpdateListener(OnDataUpdate);
    }

    private void OnDisable()
    {
        GameManager.RemoveDataUpdateListener(OnDataUpdate);
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void OnDataUpdate()
    {
        switch (dataType)
        {
            case DataType.ScoreTotal:
                text.text = GameManager.Instance?.ScoreTotal.ToString();
                break;
            case DataType.RoundsRemaining:
                text.text = GameManager.Instance?.RoundRemaining.ToString();
                break;
        }
    }
}
