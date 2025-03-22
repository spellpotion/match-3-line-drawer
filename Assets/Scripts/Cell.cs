using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static GameSettings;

[RequireComponent(typeof(CircleCollider2D))]
public class Cell : MonoBehaviour
{
    [SerializeField] private Transform arrowContainer;
    [SerializeField] private Image icon;

    [SerializeField] private List<Sprite> commonList = new List<Sprite>();
    [SerializeField] private List<Sprite> premiumList = new List<Sprite>();

    public int X { get; set; }
    public int Type { get; set; }
    public bool Bonus { get; set; }

    private CircleCollider2D circleCollider;

    private Coroutine coroutine;
    private bool spawned;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Initialize(int x, int type, bool bonus)
    {
        arrowContainer.gameObject.SetActive(false);

        icon.gameObject.SetActive(false);
        icon.sprite = bonus ? premiumList[type] : commonList[type];

        X = x;
        Type = type;
        Bonus = bonus;
    }

    public void Position(int index)
    {
        var localPosition = new Vector2(X * CellSizeX, index * CellSizeY + X % 2f * (CellSizeY * .5f));
        localPosition += originOffset;

        if (!spawned)
        {
            transform.localPosition = localPosition;

            coroutine = StartCoroutine(AnimationZoomIn());
        }
        else if (localPosition != (Vector2) transform.localPosition)
        {
            StartCoroutine(AnimationSlideTo(localPosition));
        }
    }

    public void Pop()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(AnimationZoomOut());
    }

    private IEnumerator AnimationZoomIn()
    {
        icon.transform.localScale = Vector3.zero;
        icon.gameObject.SetActive(true);

        var start = DateTime.Now;
        var progress = 0f;

        while (progress < 1f)
        {
            var duration = DateTime.Now - start;
            progress = (float)duration.TotalSeconds / durationAnimation;
            var value = Mathf.SmoothStep(0f, 1f, progress);

            icon.transform.localScale = new Vector3(value, value, value);

            yield return new WaitForEndOfFrame();
        }

        icon.transform.localScale = Vector3.one;

        spawned = true;
        coroutine = null;
    }

    private IEnumerator AnimationZoomOut()
    {
        icon.transform.localScale = Vector3.one;
        icon.gameObject.SetActive(true);

        var start = DateTime.Now;
        var progress = 0f;

        while (progress < 1f)
        {
            var duration = DateTime.Now - start;
            progress = (float)duration.TotalSeconds / (durationAnimation * .5f);
            var value = Mathf.SmoothStep(1f, 0f, progress);

            icon.transform.localScale = new Vector3(value, value, value);

            yield return new WaitForEndOfFrame();
        }

        icon.transform.localScale = Vector3.zero;

        Destroy(this.gameObject);
    }

    private IEnumerator AnimationSlideTo(Vector3 localPositionEnd)
    {
        var localPositionStart = transform.localPosition;
        var localPositionDifference = localPositionEnd - localPositionStart;

        var start = DateTime.Now;
        var progress = 0f;

        while (progress < 1f)
        {
            var duration = DateTime.Now - start;
            progress = (float)duration.TotalSeconds / (durationAnimation * .5f);
            var value = Mathf.SmoothStep(0f, 1f, progress);

            transform.localPosition = localPositionStart + (localPositionDifference * value);

            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = localPositionEnd;
    }

    public bool IsOverlapping(Vector2 point)
    {
        return circleCollider.OverlapPoint(point);
    }

    public bool IsNeighbor(Cell other)
    {
        return (Vector2.Distance(transform.localPosition, other.transform.localPosition) < circleCollider.radius * 3f);
    }

    public void SetArrowTo(Cell other)
    {
        if (other == null)
        {
            arrowContainer.gameObject.SetActive(false);
            return;
        }

        other.transform.SetAsFirstSibling();
        arrowContainer.gameObject.SetActive(true);
        var angle = Vector3.SignedAngle(other.transform.position - transform.position, Vector3.up, Vector3.back);
        arrowContainer.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnValidate()
    {
        Debug.Assert(arrowContainer != null);
        Debug.Assert(icon != null);
        Debug.Assert(commonList.Count == CellTypeCount);
        Debug.Assert(premiumList.Count == CellTypeCount);
    }
}
