using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public int Row;
    public int Col;
    public Vector2Int Start;
    public List<int> Data;
}
