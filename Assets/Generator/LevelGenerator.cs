using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int _row, _col;
    [SerializeField] private Level _level;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private Player _player;

    private Block[,] blocks;

    private void Awake()
    {
        CreateLevel();
        SpawnLevel();
    }

    private void CreateLevel()
    {
        if (_level.Row == _row && _level.Col == _col) return;
        _level.Row = _row;
        _level.Col = _col;
        _level.Start = Vector2Int.zero;
        _level.Data = new List<int>();
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _level.Col; j++)
            {
                _level.Data.Add(1);
            }
        }
        EditorUtility.SetDirty(_level);
    }

    private void SpawnLevel()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.x = _level.Col * 0.5f - 0.5f;
        camPos.y = _level.Row * 0.5f - 0.5f;
        Camera.main.transform.position = camPos;
        Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Col) + 2f;

        _player.Init(_level.Start);

        blocks = new Block[_level.Row, _level.Col];

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Col; j++)
            {
                blocks[i, j] = Instantiate(_blockPrefab, new Vector3(j, i, 0), Quaternion.identity);
                blocks[i, j].Init(_level.Data[i * _level.Col + j]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(mousePos.y + 0.5f),
                Mathf.FloorToInt(mousePos.x + 0.5f)
                );
            if (!IsValid(gridPos)) return;
            int currentFill = _level.Data[gridPos.x * _col + gridPos.y];
            currentFill = currentFill == 1 ? 0 : 1;
            _level.Data[gridPos.x * _col + gridPos.y] = currentFill;
            int i = gridPos.x;
            int j = gridPos.y;
            blocks[i, j].Init(_level.Data[i * _level.Col + j]);
            EditorUtility.SetDirty(_level);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(mousePos.y + 0.5f),
                Mathf.FloorToInt(mousePos.x + 0.5f)
                );
            if (!IsValid(gridPos)) return;
            _level.Start = gridPos;
            _player.Init(gridPos);
            EditorUtility.SetDirty(_level);
        }

    }

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Row && pos.y < _level.Col;
    }
}
