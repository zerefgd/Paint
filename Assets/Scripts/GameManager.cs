using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public bool CanClick;

    [SerializeField] private Level _level;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private Player _player;

    private Block[,] blocks;
    private Vector2 start, end;

    private bool hasGameFinished;

    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        CanClick = true;
        SpawnLevel();
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
        if (hasGameFinished || !CanClick) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            start = new Vector2(mousePos.x, mousePos.y);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            end = new Vector2(mousePos.x, mousePos.y);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2Int direction = GetDirection();
            Vector2Int offset = GetOffsetPos(direction);
            if (offset == Vector2Int.zero) return;
            StartCoroutine(_player.Move(offset, offset.x == 0 ? offset.y : offset.x));
            CanClick = false;
        }
    }

    private Vector2Int GetDirection()
    {
        Vector2 offset = end - start;
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            return offset.x > 0 ? Vector2Int.up : Vector2Int.down;
        }
        return offset.y > 0 ? Vector2Int.right : Vector2Int.left;
    }

    private Vector2Int GetOffsetPos(Vector2Int direction)
    {
        Vector2Int result = direction;
        Vector2Int checkPos = _player.Pos + result;
        while (IsValid(checkPos) && !blocks[checkPos.x, checkPos.y].Blocked)
        {
            result += direction;
            checkPos += direction;
        }
        return result - direction;
    }

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Row && pos.y < _level.Col;
    }

    public void HighLightBlock(int x, int y)
    {
        blocks[y, x].Add();
    }

    public void CheckWin()
    {
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Row; j++)
            {
                if (!blocks[i, j].Filled)
                    return;
            }
        }

        hasGameFinished = true;
        StartCoroutine(GameFinished());
    }

    private IEnumerator GameFinished()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
