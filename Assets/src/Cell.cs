using System.Collections.Generic;
using src.EnumMessenger;
using UnityEngine;

namespace src
{
    public class Cell : MonoBehaviour
    {
        private static Dictionary<CellContent, Color> _colors;
        private CellContent _content = CellContent.Empty;

        public Vector3Int Pos { get; set; }

        public Renderer Renderer { get; private set; }

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
            Renderer = GetComponent<Renderer>();
        }

        private void InitColors()
        {
            if (_colors == null)
                _colors = new Dictionary<CellContent, Color>
                {
                    [CellContent.Empty] = Color.white,
                    [CellContent.Player] = Color.blue,
                    [CellContent.Wall] = Color.black,
                    [CellContent.Goal] = Color.green
                };
        }

        public bool CanMakeMove()
        {
            return Content == CellContent.Empty;
        }

        public void UpdateColor()
        {
            if (Renderer.material.color != _colors[_content]) Renderer.material.color = _colors[_content];
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0)) Content = CellContent.Wall;
            if (Input.GetMouseButtonDown(1)) Messenger<Vector3Int>.Broadcast(GameEvent.GoalChanged, Pos);
        }
    }
}