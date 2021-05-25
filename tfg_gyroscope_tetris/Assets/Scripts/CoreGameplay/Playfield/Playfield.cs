using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    [SerializeField, Tooltip("Playfield grid data")]
    private PlayfieldGrid _grid = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(_grid != null)
        {
            Gizmos.color = Color.red;
            
            for(int i = 0; i < _grid.columns; i++)
            {
                for(int j = 0; j < _grid.rows; j++)
                {
                    float x = this.transform.position.x - (_grid.worldSizeX / 2f) + (i * _grid.cellSize) + (_grid.cellSize / 2f);
                    float y = this.transform.position.y - (_grid.worldSizeY / 2f) + (j * _grid.cellSize) + (_grid.cellSize / 2f);

                    Gizmos.DrawWireCube(new Vector3(x, y, transform.position.z), Vector3.one * _grid.cellSize);
                }
            }
        }
    }
}
