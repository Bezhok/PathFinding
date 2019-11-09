using System.Collections;
using System.Collections.Generic;
using src;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int _columnsNumber;

    public int ColumnsNumber => _columnsNumber;

    public int RowsNumber => _rowsNumber;

    [SerializeField]
    private int _rowsNumber;
    [SerializeField]
    private GameObject _cell;
    [SerializeField]
    private float xStart;
    [SerializeField]
    private float yStart;

    private Cell[,] _map;

    public Cell[,] Map
    {
        get => _map;
        set => _map = value;
    }

    void Start()
    {
        _map = new Cell[_columnsNumber, _rowsNumber];
        for (int i = 0; i < _columnsNumber; i++)
        {
            for (int j = 0; j < _rowsNumber; j++)
            {
                GameObject obj = Instantiate(_cell, new Vector3(xStart + i*1.1f, yStart-j*1.1f), Quaternion.identity);
                _map[i, j] = obj.GetComponent<Cell>();
                _map[i, j].Pos = new Vector3Int(i,j, 0);
            }
        }

        _map[0, 0].Renderer.material.color = Color.blue;

        for (int i = 0; i < _rowsNumber-1; i++)
        {
            _map[2, i].Content = CellContent.Wall;
        }
        
        for (int i = 1; i < _rowsNumber; i++)
        {
            _map[5, i].Content = CellContent.Wall;
        }
        
        for (int i = 1; i < _rowsNumber; i++)
        {
            _map[10, i].Content = CellContent.Wall;
        }
        
        for (int i = 1; i < _rowsNumber; i++)
        {
            _map[13, i].Content = CellContent.Wall;
        }
//        _map[_columnsNumber-1, _rowsNumber-1].Content = CellContent.Goal;

        var p = new PathFinder(this);
        Cell.p = p;
        p.Find(new Vector3Int(0,0, 0), new Vector3Int(_columnsNumber-1, _rowsNumber-1, 0));
    }

    public bool CanMakeMove(int x, int y, int z)
    {
        bool isValidRange = x >= 0 && y >= 0 && x < ColumnsNumber && y < RowsNumber;
        return isValidRange && _map[x, y].CanMakeMove();
    }
    void Update()
    {
        
    }
}
