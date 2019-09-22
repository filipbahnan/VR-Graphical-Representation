using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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
        spawnLines();
        positionNodes();
    }

    void spawnNodes(string[] row)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObj.transform.localScale += new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = newObj.GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 6; i++)
            textObject[i].text = row[0];

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

    void positionNodes()
    {
        int arraySize = 3;
        bool[, ,] mapOfNodes = new bool[arraySize, arraySize, arraySize];

        int x = Random.Range(0, arraySize - 1);
        int y = Random.Range(0, arraySize - 1);
        int z = Random.Range(0, arraySize - 1);


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
                int size = nodes[i].GetComponent<Node>().getAmountOfChildren();
                nodes[i].transform.localScale += new Vector3(2*size, 2 * size, 2 * size);
                nodes[i].GetComponent<Renderer>().material = abstractMaterial;
            }
        }
    }

    void spawnLines()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetComponent<Node>().getParent() != "" && nodes.First(Node => Node.name == nodes[i].GetComponent<Node>().getParent()).GetComponent<Node>().getisNodeAbstract() != true) 
            {
                LineRenderer relation = nodes[i].AddComponent<LineRenderer>();
                relation.material = new Material(Shader.Find("Sprites/Default"));
                Vector3[] positions = new Vector3[2];
                positions[0] = nodes[i].transform.position;
                positions[1] = nodes.First(Node => Node.name == nodes[i].GetComponent<Node>().getParent()).transform.position;

                relation.widthMultiplier = 0.2f;
                relation.material = new Material(Shader.Find("Sprites/Default"));
                relation.SetPositions(positions);

            }
            
        }
            
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
