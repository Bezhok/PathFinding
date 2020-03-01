using System.Collections.Generic;
using System.Linq;
using src;
using src.EnumMessenger;
using src.Path;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private const float TimePerMove = 0.02f;

    [SerializeField] private GameObject _cell;
    private List<Vector3M> _path;


    private PathFinder _pathFinder;

    private Vector3Int _startPlayerPos;

    private float _timeSummer;
    [SerializeField] private int xCells = 32;
    [SerializeField] private float xStart;
    [SerializeField] private int yCells = 16;
    [SerializeField] private float yStart;
    public int ColumnsNumber => Map.GetLength(0);

    public int RowsNumber => Map.GetLength(1);

    public Cell[,] Map { get; set; }

    private void InstantiateMap()
    {
        Map = new Cell[xCells, yCells];

        var vertExtent = 2 * Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        var offsetScaller = 1.04f;
        var scaller = (horzExtent - 0.04f) / (ColumnsNumber * offsetScaller);
        xStart -= (1 - scaller) / 2;
        yStart += (1 - scaller) / 2;
        for (var i = 0; i < ColumnsNumber; i++)
        for (var j = 0; j < RowsNumber; j++)
        {
            var obj = Instantiate(_cell,
                new Vector3(xStart + i * offsetScaller * scaller, yStart - j * offsetScaller * scaller),
                Quaternion.identity);
            obj.name = $"x:{i}, y:{j}";

            obj.transform.localScale = new Vector3(scaller, scaller, scaller);
            Map[i, j] = obj.GetComponent<Cell>();
            Map[i, j].Pos = new Vector3Int(i, j, 0);
        }
    }

    private void Start()
    {
        Messenger<Vector3Int>.AddListener(GameEvent.GOAL_CHANGED, OnGoalChanged);

        InstantiateMap();
        GenerateLevel();
        
        _pathFinder = new PathFinder(this);
    }

    private void GenerateLevel()
    {
        VerticalGenerator(2, 0, RowsNumber - 2);
        VerticalGenerator(5, 1, RowsNumber - 1);
        VerticalGenerator(10, 1, RowsNumber - 1);
        VerticalGenerator(13, 1, RowsNumber - 1);

        if (ColumnsNumber >= 20)
        {
            HorizontalGenerator(5, 15, 26);
            HorizontalGenerator(10, 15, 26);
        }

        VerticalGenerator(15, 5, 10);
        VerticalGenerator(26, 5, 10);

        Map[16, 10].Content = CellContent.Empty;
    }

    private void VerticalGenerator(int x, int startY, int endY)
    {
        for (var y = startY; y <= endY; y++) Map[x, y].Content = CellContent.Wall;
    }

    private void HorizontalGenerator(int y, int startX, int endX)
    {
        for (var x = startX; x <= endX; x++) Map[x, y].Content = CellContent.Wall;
    }

    public bool CanMakeMove(int x, int y, int z)
    {
        var isValidRange = x >= 0 && y >= 0 && x < ColumnsNumber && y < RowsNumber;
        return isValidRange && Map[x, y].CanMakeMove();
    }

    public void ResetColors()
    {
        for (var i = 0; i < ColumnsNumber; i++)
        for (var j = 0; j < RowsNumber; j++)
            Map[i, j].UpdateColor();
    }

    private void OnGoalChanged(Vector3Int pos)
    {
        ResetColors();
        _path = _pathFinder.Find(_startPlayerPos, pos);
        _startPlayerPos = pos;
    }

    private void Update()
    {
        if (_path == null || !_path.Any()) _path = null;

        if (_path != null)
        {
            var moves = 0;
            _timeSummer += Time.deltaTime;
            while (_timeSummer >= TimePerMove)
            {
                _timeSummer -= TimePerMove;
                moves++;
            }

            while (moves != 0 && _path.Any())
            {
                var pos = _path[_path.Count - 1];
                Map[pos.v.x, pos.v.y].Renderer.material.color = Color.green;
                _path.RemoveAt(_path.Count - 1);
                moves--;
            }
        }
    }
}