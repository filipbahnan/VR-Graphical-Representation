using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

public class Spawn_nodes : MonoBehaviour
{
    public GameObject prefab;
    public float nodeSize;
    private List<GameObject> nodes = new List<GameObject>();
    public Material abstractMaterial;
    private string jsonString;

    // Start is called before the first frame update
    void Start()
    {
        read();
        setNodes();
        spawnLines();
        positionNodes();
    }

    void spawnNodes(Node theNode)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObj.transform.localScale += new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = newObj.GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 6; i++)
            textObject[i].text = theNode.@class;

        newObj.AddComponent<NodeGameObject>();
        newObj.name = theNode.@class.Trim();
        newObj.GetComponent<NodeGameObject>().setParent(theNode.parent.Trim());

        if (theNode.@abstract.Trim() == "yes")
        {
            newObj.GetComponent<NodeGameObject>().setisNodeAbstract(true);
        }
        else
        {
            newObj.GetComponent<NodeGameObject>().setisNodeAbstract(false);
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
            if(nodes[i].GetComponent<NodeGameObject>().getParent() != "")
            {
                GameObject theParent = nodes.First(NodeGameObject => NodeGameObject.name == nodes[i].GetComponent<NodeGameObject>().getParent());
                theParent.GetComponent<NodeGameObject>().addChild();
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetComponent<NodeGameObject>().getisNodeAbstract() == true)
            {
                int size = nodes[i].GetComponent<NodeGameObject>().getAmountOfChildren();
                nodes[i].transform.localScale += new Vector3(2*size, 2 * size, 2 * size);
                nodes[i].GetComponent<Renderer>().material = abstractMaterial;
            }
        }
    }

    void spawnLines()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetComponent<NodeGameObject>().getParent() != "" && nodes.First(NodeGameObject => NodeGameObject.name == nodes[i].GetComponent<NodeGameObject>().getParent()).GetComponent<NodeGameObject>().getisNodeAbstract() != true) 
            {
                LineRenderer relation = nodes[i].AddComponent<LineRenderer>();
                relation.material = new Material(Shader.Find("Sprites/Default"));
                Vector3[] positions = new Vector3[2];
                //positions[0] = nodes[i].transform.position;
                positions[0] = new Vector3(0,0,0);
                positions[1] = nodes.First(NodeGameObject => NodeGameObject.name == nodes[i].GetComponent<NodeGameObject>().getParent()).transform.position;
                Transform transformOfChild = nodes[i].transform;
                positions[1] = transformOfChild.InverseTransformPoint(positions[1]);
                relation.widthMultiplier = 0.2f;
                relation.SetPositions(positions);
                relation.useWorldSpace = false;

            }
            
        }
            
    }

    public void read()
    {
        string filename = "json_data.json";
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + filename);
        RootObject myJsonObject = JsonConvert.DeserializeObject<RootObject>(jsonString);
        Debug.Log(myJsonObject.jsonNodes[1].parent);

        /*
        TextAsset softwareData = Resources.Load<TextAsset>("test_data");
        string[] data = softwareData.text.Split('\n');
        */

        for (int i = 0; i < myJsonObject.jsonNodes.Count; i++)
        {
            //string[] row = data[i].Split(',');
            spawnNodes(myJsonObject.jsonNodes[i]);
        }
        
    }
}
