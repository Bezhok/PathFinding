using System;
using System.Collections.Generic;
using src.EnumMessenger;
using UnityEngine;

namespace src
{
    public class Cell : MonoBehaviour
    {
        private static Dictionary<CellContent, Color> _colors;
        private CellContent _content = CellContent.Empty;
        private Renderer _renderer;
        private Vector3Int _pos;

        public Vector3Int Pos
        {
            get => _pos;
            set => _pos = value;
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

        public void UpdateColor()
        {
            if (Renderer.material.color != _colors[_content])
            {
                Renderer.material.color = _colors[_content];
            }
        }
        
        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Content = CellContent.Wall;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Messenger<Vector3Int>.Broadcast(GameEvent.GOAL_CHANGED, _pos);
            }
        }
    }
}