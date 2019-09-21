using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawn_nodes : MonoBehaviour
{
    public GameObject prefab;
    public float nodeSize;
    private List<GameObject> nodes = new List<GameObject>();
    public Material abstractMaterial;

    // Start is called before the first frame update
    void Start()
    {
        read();
        setNodes();
    }

    void spawnNodes(string[] row)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObj.transform.localScale += new Vector3(nodeSize, nodeSize, nodeSize);

        newObj.AddComponent<Node>();
        newObj.name = row[0].Trim();
        newObj.GetComponent<Node>().setParent(row[1].Trim());

        if (row[2].Trim() == "yes")
        {
            newObj.GetComponent<Node>().setisNodeAbstract(true);
        }
        else
        {
            newObj.GetComponent<Node>().setisNodeAbstract(false);
        }

        nodes.Add(newObj);

    }

    void setNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if(nodes[i].GetComponent<Node>().getParent() != "")
            {
                GameObject theParent = nodes.First(Node => Node.name == nodes[i].GetComponent<Node>().getParent());
                theParent.GetComponent<Node>().addChild();
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetComponent<Node>().getisNodeAbstract() == true)
            {
                Debug.Log("bajs");
                int size = nodes[i].GetComponent<Node>().getAmountOfChildren();
                nodes[i].transform.localScale += new Vector3(2*size, 2 * size, 2 * size);
                nodes[i].GetComponent<Renderer>().material = abstractMaterial;
            }
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
