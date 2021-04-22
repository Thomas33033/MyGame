using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGridMesh : MonoBehaviour
{
    private bool m_showGrid = true;

    public int GridCountH;
    public int GridCountV;
    public int GridSize = 32;
    Transform _showGrid = null;

    public Dictionary<int, NodeRender> nodeGraphMap;


    

    // Update is called once per frame
    public void UpdateShowGrid()
    {
        MeshRenderer meshRenderer = null;
        MeshFilter meshFilter = null;
        if (_showGrid == null)
        {
            var _obj = new GameObject();
            _showGrid = _obj.transform;
            _showGrid.transform.SetParent(this.transform);

            _showGrid.localPosition = new Vector3(
                -GridSize * GridCountH / 2f - GridSize / 2f, 
                0, 
                -GridSize * GridCountV / 2f - GridSize / 2f);
            _showGrid.localScale = new Vector3(1, 1, 1);
            _showGrid.gameObject.name = "ShowGrid";

            meshRenderer = _showGrid.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("VertexColor"));
            meshFilter = _showGrid.gameObject.AddComponent<MeshFilter>();
        }
        else
        {
            meshRenderer = _showGrid.gameObject.GetComponent<MeshRenderer>();
            meshFilter = _showGrid.gameObject.GetComponent<MeshFilter>();
        }

        Mesh mesh = new Mesh();

        int gridWidth = GridSize;
        int gridHeight = GridSize;

        int width = GridSize * GridCountH;
        int height = GridSize * GridCountV;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        for (int row = 0; row < GridCountV; ++row)
        {
            for (int col = 0; col < GridCountH; ++col)
            {
                int _tileIndex = row * GridCountH + col;

                int _verIndex = vertices.Count;

                float _left = col * gridWidth;
                float _top = row * gridHeight;

                Vector3[] _vers = new Vector3[4]
                {
                    new Vector3((_left + 0),0, (_top + 0)),
                    new Vector3((_left + gridWidth), 0, (_top + 0)),
                    new Vector3((_left + 0), 0, (_top + gridHeight)),
                    new Vector3((_left + gridWidth), 0, (_top + gridHeight))
                };
                Color _gridColor = Color.clear;
                // _gridColor = row == 0 ? Color.green : Color.red;

                if (this.nodeGraphMap != null && this.nodeGraphMap.ContainsKey(_tileIndex))
                {
                   // _gridColor = this.nodeGraphMap[_tileIndex].GetColor();
                }

                Color[] _colors = new Color[4]
                {
                    _gridColor, _gridColor, _gridColor, _gridColor
                };

                int[] _tris = new int[6]
                {
                    // lower left triangle
                    0, 2, 1,
                    // upper right triangle
                    2, 3, 1
                };

                for (int _i = 0; _i < _tris.Length; ++_i)
                {
                    _tris[_i] = _tris[_i] + _verIndex;
                }

                vertices.AddRange(_vers);
                colors.AddRange(_colors);
                triangles.AddRange(_tris);
                
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.colors = colors.ToArray();
        mesh.triangles = triangles.ToArray();
        meshFilter.mesh = mesh;
    }
}