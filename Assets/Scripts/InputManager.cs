using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class InputManager : MonoBehaviour
{
    private List<Cell> selectedList = new();

    private bool mouseDown;
    private bool lockInput;

    private void OnEnable()
    {
        GameManager.AddDataUpdateListener(OnDataUpdate);
    }

    private void OnDisable()
    {
        GameManager.RemoveDataUpdateListener(OnDataUpdate);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) QuitApplication();

        if (lockInput) return;

        MouseDragUpdate();

        if (Input.GetMouseButtonDown(0))
        {
            var clickedCell = GameManager.Instance?.Cells.FirstOrDefault(cell => cell.IsOverlapping(Input.mousePosition));

            if (clickedCell != null) selectedList.Add(clickedCell);

            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            selectedList.ForEach(cell => cell.SetArrowTo(null));

            if (selectedList.Count > 2)
            {
                GameManager.Instance?.Collect(selectedList.ToArray());
            }

            selectedList.Clear();
            mouseDown = false;
        }
    }

    private void QuitApplication()
    {
        if (Application.isMobilePlatform)
        {
            Application.Unload();
        }
        else
        {
            Application.Quit();
        }
    }

    private void MouseDragUpdate()
    {
        if (!mouseDown && selectedList.Count == 0) return;

        var last = selectedList[^1];

        if (last.IsOverlapping(Input.mousePosition)) return;

        var cell = GameManager.Instance?.Cells.FirstOrDefault(x => x.IsOverlapping(Input.mousePosition));

        if (cell == null || !last.IsNeighbor(cell)) return;

        var index = selectedList.IndexOf(cell);

        if (index == -1)
        {
            if (cell.Type == last.Type)
            {
                selectedList.Add(cell);
                last.SetArrowTo(cell);
            }
        }
        else if (index == selectedList.Count - 2)
        {
            selectedList[index].SetArrowTo(null);
            selectedList.Remove(last);
        }
    }

    public void OnDataUpdate()
    {
        lockInput = GameManager.Instance?.State != GameManager.GameState.Round;
    }

}
