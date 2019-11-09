using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using src;
using src.EnumMessenger;
using src.Path;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int ColumnsNumber
    {
        get { return _map.GetLength(0); }
    }

    public int RowsNumber
    {
        get { return _map.GetLength(1); }
    }

    [SerializeField] private GameObject _cell;
    [SerializeField] private float xStart;
    [SerializeField] private float yStart;

    private Cell[,] _map;

    public Cell[,] Map
    {
        get => _map;
        set => _map = value;
    }

    private Vector3Int _startPos;
    void Start()
    {
        Messenger<Vector3Int>.AddListener(GameEvent.GOAL_CHANGED, OnGoalChanged);
        
        _map = new Cell[16*2, 9*2];
        
        float scaller = 16.0f / ColumnsNumber;
        xStart -= (1 - scaller)/2;
        yStart += (1 - scaller)/2;
        for (int i = 0; i < ColumnsNumber; i++)
        {
            for (int j = 0; j < RowsNumber; j++)
            {
                GameObject obj = Instantiate(_cell, new Vector3(xStart + i * 1.1f * scaller, yStart - j * 1.1f * scaller),
                    Quaternion.identity);
                obj.transform.localScale = new Vector3(scaller, scaller, scaller);
                _map[i, j] = obj.GetComponent<Cell>();
                _map[i, j].Pos = new Vector3Int(i, j, 0);
            }
        }

        _map[0, 0].Renderer.material.color = Color.blue;

        for (int i = 0; i < RowsNumber - 1; i++)
        {
            _map[2, i].Content = CellContent.Wall;
        }

        for (int i = 1; i < RowsNumber; i++)
        {
            _map[5, i].Content = CellContent.Wall;
        }

        for (int i = 1; i < RowsNumber; i++)
        {
            _map[10, i].Content = CellContent.Wall;
        }

        for (int i = 1; i < RowsNumber; i++)
        {
            _map[13, i].Content = CellContent.Wall;
        }

        _pathFinder = new PathFinder(this);
//        _pathFinder.Find(new Vector3Int(0, 0, 0), new Vector3Int(ColumnsNumber - 1, RowsNumber - 1, 0));
    }

    private PathFinder _pathFinder;
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
                if (_map[i, j].Content.Equals(CellContent.Empty))
                {
                    _map[i, j].Renderer.material.color = Color.white;
                }
            }
        }
    }

    private List<Vector3M> _path;
    private void OnGoalChanged(Vector3Int pos)
    {
        ResetColors();
        _path = _pathFinder.Find(_startPos, pos);
        _startPos = pos;
    }

    private float _timeSummer = 0.0f;
    private const float timePerMove = 0.02f;
    private void Update()
    {
        if (_path == null || !_path.Any())
        {
            _path = null;
        }
        
        if (_path != null )
        {
            int moves = 0;
            _timeSummer += Time.deltaTime;
            while (_timeSummer >= timePerMove)
            {
                _timeSummer -= timePerMove;
                moves++;
            }

            while (moves != 0 && _path.Any())
            {
                var pos = _path[_path.Count - 1];
                _map[pos.v.x, pos.v.y].Renderer.material.color = Color.yellow;
                _path.RemoveAt(_path.Count - 1);
                moves--;
            }
        }
    }
}