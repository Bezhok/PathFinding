using System;
using System.Collections.Generic;
using UnityEngine;

namespace src
{
    public class Cell : MonoBehaviour
    {
        private static Dictionary<CellContent, Color> _colors;
        private CellContent _content = CellContent.Empty;
        private Renderer _renderer;

        private Vector3Int pos;

        public Vector3Int Pos
        {
            get => pos;
            set => pos = value;
        }

        public Renderer Renderer => _renderer;

        public CellContent Content
        {
            get => _content;
            set
            {
                _content = value;
                Renderer.material.color = _colors[_content];
            }
        }

        private void Awake()
        {
            InitColors();
            _renderer = GetComponent<Renderer>();
        }

        private void InitColors()
        {
            if (_colors == null)
            {
                _colors = new Dictionary<CellContent, Color>();
                _colors[CellContent.Empty] = Color.white;
                _colors[CellContent.Player] = Color.blue;
                _colors[CellContent.Wall] = Color.black;
                _colors[CellContent.Goal] = Color.green;
            } 
        }

        public bool CanMakeMove()
        {
            return Content == CellContent.Empty;
        }
        
        public static PathFinder p;
        private void OnMouseOver()
        {
            if(Input.GetMouseButtonDown(0))
                Content = CellContent.Wall;
            if(Input.GetMouseButtonDown(1))
            {
                p.Find(new Vector3Int(0,0, 0), pos);
            }
        }
    }
}