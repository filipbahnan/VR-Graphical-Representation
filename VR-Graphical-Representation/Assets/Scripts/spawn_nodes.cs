using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_nodes : MonoBehaviour
{
    public data softwareArchitecture;
    public GameObject prefab;
    public float nodeSize;
    // Start is called before the first frame update
    void Start()
    {
        spawnNodes();
    }

    void spawnNodes()
    {
        for (int i = 0; i < 10000; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
            newObj.transform.localScale += new Vector3(nodeSize, nodeSize, nodeSize);
        }
    }

    void spawnLines()
    {

    }

}
