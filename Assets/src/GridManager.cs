using System.Collections.Generic;
using System.Linq;
using src;
using src.EnumMessenger;
using src.Path;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int ColumnsNumber => _map.GetLength(0);

    public int RowsNumber => _map.GetLength(1);

    [SerializeField] private GameObject _cell;
    [SerializeField] private float xStart;
    [SerializeField] private float yStart;   
    [SerializeField] private int xCells = 32;
    [SerializeField] private int yCells = 16;
    
    
    private PathFinder _pathFinder;
    private Cell[,] _map;
    private List<Vector3M> _path;

    public Cell[,] Map
    {
        get => _map;
        set => _map = value;
    }

    private Vector3Int _startPlayerPos;

    void Start()
    {
        Messenger<Vector3Int>.AddListener(GameEvent.GOAL_CHANGED, OnGoalChanged);

        _map = new Cell[xCells, yCells];

        var vertExtent = 2*Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        float offsetScaller = 1.04f;
        float scaller = (horzExtent-0.04f) / (ColumnsNumber * offsetScaller);
        xStart -= (1 - scaller) / 2;
        yStart += (1 - scaller) / 2;
        for (int i = 0; i < ColumnsNumber; i++)
        {
            for (int j = 0; j < RowsNumber; j++)
            {
                GameObject obj = Instantiate(_cell,
                    new Vector3(xStart + i * offsetScaller * scaller, yStart - j * offsetScaller * scaller),
                    Quaternion.identity);
                obj.name = $"x:{i}, y:{j}";
                
                obj.transform.localScale = new Vector3(scaller, scaller, scaller);
                _map[i, j] = obj.GetComponent<Cell>();
                _map[i, j].Pos = new Vector3Int(i, j, 0);
            }
        }

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
        
        _map[16, 10].Content = CellContent.Empty;
    }

    void VerticalGenerator(int x, int startY, int endY)
    {
        for (int y = startY; y <= endY; y++)
        {
            _map[x, y].Content = CellContent.Wall;
        }
    }
    
    void HorizontalGenerator(int y, int startX, int endX)
    {
        for (int x = startX; x <= endX; x++)
        {
            _map[x, y].Content = CellContent.Wall;
        }
    }
    
    public bool CanMakeMove(int x, int y, int z)
    {
        bool isValidRange = x >= 0 && y >= 0 && x < ColumnsNumber && y < RowsNumber;
        return isValidRange && _map[x, y].CanMakeMove();
    }

    public void ResetColors()
    {
        for (int i = 0; i < ColumnsNumber; i++)
        {
            for (int j = 0; j < RowsNumber; j++)
            {
                _map[i, j].UpdateColor();
            }
        }
    }
    
    private void OnGoalChanged(Vector3Int pos)
    {
        ResetColors();
        _path = _pathFinder.Find(_startPlayerPos, pos);
        _startPlayerPos = pos;
    }

    private float _timeSummer;
    private const float TimePerMove = 0.02f;

    private void Update()
    {
        if (_path == null || !_path.Any())
        {
            _path = null;
        }

        if (_path != null)
        {
            int moves = 0;
            _timeSummer += Time.deltaTime;
            while (_timeSummer >= TimePerMove)
            {
                _timeSummer -= TimePerMove;
                moves++;
            }

            while (moves != 0 && _path.Any())
            {
                var pos = _path[_path.Count - 1];
                _map[pos.v.x, pos.v.y].Renderer.material.color = Color.green;
                _path.RemoveAt(_path.Count - 1);
                moves--;
            }
        }
    }
}