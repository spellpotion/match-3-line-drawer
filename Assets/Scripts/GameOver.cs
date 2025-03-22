using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using static GameSettings;

[RequireComponent(typeof(Text))]
public class GameOver : MonoBehaviour
{
    private Text text;
    private Color original;
    private Coroutine coroutine;

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
        original = text.color;
    }

    private void Start()
    {
        Clear();
    }

    private void Clear()
    {
        text.color = new Color(original.r, original.g, original.b, 0f);
    }

    private void OnDataUpdate()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        if (GameManager.GameState.End == GameManager.Instance?.State)
        {
            coroutine = StartCoroutine(AnimationFadeIn());
        }
        else
        {
            Clear();
        }
    }

    private IEnumerator AnimationFadeIn()
    {
        var start = DateTime.Now;
        var progress = 0f;

        while (progress < 1f)
        {
            var duration = DateTime.Now - start;
            progress = (float) duration.TotalSeconds / (durationAnimation * 3f);
            var value = Mathf.SmoothStep(0f, 1f, progress);

            text.color = new Color(original.r, original.g, original.b, original.a * value);

            yield return new WaitForEndOfFrame();
        }

        text.color = original;
    }
}
