using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameSettings;

public class GameManager : MonoBehaviour
{
    public enum GameState { Start, Round, Collect, End }

    public static GameManager Instance { get; private set; }

    private static Action DataUpdate;

    public GameState State { get; private set; }
    public int ScoreTotal { get; private set; }
    public int RoundRemaining { get; private set; }
    public Cell[] Cells { get; private set; } = new Cell[0];

    [SerializeField] private Transform gameBoard;
    [SerializeField] private GameObject cellPrefab;

    private List<Cell> cellList = new List<Cell>();
    private int[] columnCellCount = new int[CellCountX];

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void Start()
    {
        State = GameState.Start;
        DataUpdate?.Invoke();
    }

    public void StartGame()
    {
        if (State == GameState.End)
        {
            Clear();
        }

        Populate();
        PositionCells();

        ScoreTotal = 0;
        RoundRemaining = turnMax;

        State = GameState.Round;
        Cells = cellList.ToArray();
        DataUpdate?.Invoke();
    }

    private void Clear()
    {
        foreach (var cell in cellList)
        {
            cell.Pop();
        }
        cellList.Clear();
        Array.Clear(columnCellCount, 0, CellCountX);
    }

    public void Collect(Cell[] cells)
    {
        int scoreRound = 0;

        foreach(var cell in cells)
        {
            scoreRound += scoreCell;
            if (cell.Bonus)
            {
                scoreRound *= scoreBonusMultiplier;
            }

            cellList.Remove(cell);
            columnCellCount[cell.X]--;
            cell.Pop();
        }

        Populate();
        PositionCells();

        State = GameState.Collect;
        ScoreTotal += scoreRound;

        DataUpdate?.Invoke();
        StartCoroutine(EndRound());
    }

    private void Populate()
    {
        for (var iX = 0; iX < CellCountX; iX++)
        {
            for (var iY = columnCellCount[iX]; iY < CellCountY; iY++)
            {
                var instance = Instantiate(cellPrefab, gameBoard);
                var cell = instance.GetComponent<Cell>();
                var type = UnityEngine.Random.Range(0, CellTypeCount);
                var bonus = UnityEngine.Random.Range(0, 4) == 0;
                cell.Initialize(iX, type, bonus);

                cellList.Add(cell);
                columnCellCount[iX]++;
            }
        }
    }

    private void PositionCells()
    {
        int[] columnCellCount = new int[CellCountX];

        foreach (var cell in cellList)
        {
            cell.Position(columnCellCount[cell.X]);
            columnCellCount[cell.X]++;
        }
    }

    private IEnumerator EndRound()
    {
        yield return new WaitForSeconds(durationAnimation);

        State = --RoundRemaining > 0 ? GameState.Round : GameState.End;
        Cells = cellList.ToArray();

        DataUpdate?.Invoke();
    }

    public static void AddDataUpdateListener(Action onDataUpdate)
    {
        DataUpdate += onDataUpdate;
    }
    public static void RemoveDataUpdateListener(Action onDataUpdate)
    {
        DataUpdate -= onDataUpdate;
    }

    private void OnValidate()
    {
        Debug.Assert(gameBoard != null);
        Debug.Assert(cellPrefab != null);
    }
}
