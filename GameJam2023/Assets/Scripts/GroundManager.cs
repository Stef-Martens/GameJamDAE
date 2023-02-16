using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public GameObject prefab;
    public int numRows = 3;
    public int numColumns = 3;
    public float spacing = 3f;

    void Start()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                Vector3 position = new Vector3(col * spacing, 0f, row * -spacing) + transform.position;
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                instance.transform.SetParent(transform);
            }
        }
    }
}
