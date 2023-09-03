using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Vector2Int Pos;

    [SerializeField] private float _moveTime;
    [SerializeField] private AnimationCurve _speedCurve;

    public void Init(Vector2Int start)
    {
        Pos = start;
        transform.position = new Vector3(Pos.y, Pos.x, 0f);
    }

    public IEnumerator Move(Vector2Int offset, int distance)
    {
        Pos += offset;
        float totalTime = Mathf.Abs(distance) * _moveTime;
        float speed = 1 / totalTime;
        float timeElapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(Pos.y, Pos.x, 0f);
        Vector3 offsetPos = endPos - startPos;
        while (timeElapsed < 1f)
        {
            transform.position = startPos + offsetPos * _speedCurve.Evaluate(timeElapsed);
            int x = Mathf.FloorToInt(transform.position.x + 0.5f);
            int y = Mathf.FloorToInt(transform.position.y + 0.5f);
            GameManager.Instance.HighLightBlock(x, y);
            timeElapsed += speed * Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        GameManager.Instance.CanClick = true;
        GameManager.Instance.CheckWin();
    }
}
