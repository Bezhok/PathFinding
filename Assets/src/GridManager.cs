using System.Collections.Generic;
using System.Linq;
using src.EnumMessenger;
using src.Path;
using UnityEngine;

namespace src
{
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

            float vertExtent = 2 * Camera.main.orthographicSize;
            float horizExtent = vertExtent * Screen.width / Screen.height;

            float offsetScale = 1.04f;
            float scale = (horizExtent - 0.04f) / (ColumnsNumber * offsetScale);
            xStart -= (1 - scale) / 2;
            yStart += (1 - scale) / 2;
            for (int i = 0; i < ColumnsNumber; i++)
            for (int j = 0; j < RowsNumber; j++)
            {
                GameObject obj = Instantiate(_cell,
                    new Vector3(xStart + i * offsetScale * scale, yStart - j * offsetScale * scale),
                    Quaternion.identity);
                obj.name = $"x:{i}, y:{j}";

                obj.transform.localScale = new Vector3(scale, scale, scale);
                Map[i, j] = obj.GetComponent<Cell>();
                Map[i, j].Pos = new Vector3Int(i, j, 0);
            }
        }

        private void Start()
        {
            Messenger<Vector3Int>.AddListener(GameEvent.GoalChanged, OnGoalChanged);

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
            for (int y = startY; y <= endY; y++) Map[x, y].Content = CellContent.Wall;
        }

        private void HorizontalGenerator(int y, int startX, int endX)
        {
            for (int x = startX; x <= endX; x++) Map[x, y].Content = CellContent.Wall;
        }

        public bool CanMakeMove(int x, int y, int z)
        {
            bool isValidRange = x >= 0 && y >= 0 && x < ColumnsNumber && y < RowsNumber;
            return isValidRange && Map[x, y].CanMakeMove();
        }

        public void ResetColors()
        {
            for (int i = 0; i < ColumnsNumber; i++)
            for (int j = 0; j < RowsNumber; j++)
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
                int moves = 0;
                _timeSummer += Time.deltaTime;
                while (_timeSummer >= TimePerMove)
                {
                    _timeSummer -= TimePerMove;
                    moves++;
                }

                while (moves != 0 && _path.Any())
                {
                    Vector3M pos = _path[_path.Count - 1];
                    Map[pos.V.x, pos.V.y].Renderer.material.color = Color.green;
                    _path.RemoveAt(_path.Count - 1);
                    moves--;
                }
            }
        }
    }
}