using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_nodes : MonoBehaviour
{
    public GameObject prefab;
    public float nodeSize;
    private List<Node> nodes = new List<Node>();

    // Start is called before the first frame update
    void Start()
    {
        read();
    }

    void spawnNodes(string[] row)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObj.transform.localScale += new Vector3(nodeSize, nodeSize, nodeSize);

        newObj.AddComponent<Node>();
        newObj.name = row[0];
        newObj.GetComponent<Node>().setParent(row[1]);

        if (row[2] == "yes")
        {
            newObj.GetComponent<Node>().setisNodeAbstract(true);
        }
        else
        {
            newObj.GetComponent<Node>().setisNodeAbstract(false);
        }

    }

    void spawnLines()
    {

    }

    public void read()
    {
        TextAsset softwareData = Resources.Load<TextAsset>("test_data");
        string[] data = softwareData.text.Split('\n');

        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(',');
            spawnNodes(row);
        }
    }
}
