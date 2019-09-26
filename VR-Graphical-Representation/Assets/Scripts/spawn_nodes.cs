using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Valve.VR.InteractionSystem;


public class Spawn_nodes : MonoBehaviour
{
    public GameObject prefab;
    public GameObject platformPrefab;
    public GameObject centerPrefab;
    private GameObject centerObject;
    public float nodeSize;
    private List<GameObject> nodes = new List<GameObject>();
    public Material abstractMaterial;
    private string jsonString;
    private int amountOfLayers = 0;
    private bool[,,] mapOfNodes;
    private int[] freeSlots;
    private int mapLength = 0;
    public float spacing;
    private bool odd;
    private GameObject[] firstObjects = new GameObject[8];
    private Vector3 middle;
    private float platformSize = 14f;


    // Start is called before the first frame update
    void Start()
    {
        read();
        setNodes();
        positionNodes();
        spawnLines();
        setCenter();
        spawnPlatform();
        setSpawnPosition();
        Debug.Log(nodes.Count);

    }

    void spawnNodes(Node theNode)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObj.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
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
        bool keepLooping = true;
        int i = 3;
        while(keepLooping == true)
        {
            if(nodes.Count > (i*i*i))
            {
                i++;
            }
            else
            {
                mapLength = i;
                keepLooping = false;
            }
        }
        int innerLength = mapLength - 2;
        amountOfLayers = countLayers(innerLength, amountOfLayers) - 1;

        freeSlots = new int[amountOfLayers + 1];
        if((mapLength % 2) == 1)
        {
            odd = true;
            freeSlots[0] = 1;
            for (int x = 1; x < amountOfLayers + 1; x++)
            {
                freeSlots[x] = getOddSlots(x, freeSlots[x - 1]);
            }
        }
        else
        {
            odd = false;
            freeSlots[0] = 8;
            for (int x = 1; x < amountOfLayers + 1; x++)
            {
                freeSlots[x] = getEvenSlots(x, freeSlots[x - 1]);
            }
        }
        for (int x = 0; x < amountOfLayers + 1; x++)
        {
            Debug.Log(freeSlots[x]);
        }


        mapOfNodes = new bool[mapLength, mapLength, mapLength];
        List<GameObject> listOfRed = new List<GameObject>();
        List<GameObject> listOfOrange = new List<GameObject>();
        List<GameObject> listOfYellow = new List<GameObject>();

        for (int x = 0; x < nodes.Count; x++)
        {
            string color = nodes[x].GetComponent<NodeGameObject>().getColor();
            if(color == "red")
            {
                listOfRed.Add(nodes[x]);
            }
            else if(color == "orange")
            {
                listOfOrange.Add(nodes[x]);
            }
            else if(color == "yellow")
            {
                listOfYellow.Add(nodes[x]);
            }
        }
        
        
        for (int x = 0; x < listOfRed.Count; x++)
        {
            int j = 0;
            while (freeSlots[j] == 0)
            {
                j++;
            }
            listOfRed[x].transform.position = getPosition(j);
            if (x < 8)
            {
                firstObjects[x] = listOfRed[x];
            }
        }
        for (int x = 0; x < listOfOrange.Count; x++)
        {
            int j = 0;
            while (freeSlots[j] == 0)
            {
                j++;
            }
            listOfOrange[x].transform.position = getPosition(j);
            if(firstObjects.Length < 8)
            {
                firstObjects[firstObjects.Length] = listOfOrange[x];
            }
        }
        for (int x = 0; x < listOfYellow.Count; x++)
        {
            int j = 0;
            while (freeSlots[j] == 0)
            {
                j++;
            }
            listOfYellow[x].transform.position = getPosition(j);
            if (firstObjects.Length < 8)
            {
                firstObjects[firstObjects.Length] = listOfYellow[x];
            }

        }


    }

    int countLayers(int innerLength, int amountOfLayers)
    {
        if (innerLength > 2)
        {
            amountOfLayers = countLayers(innerLength - 2, amountOfLayers + 1);
        }
        else
        {
            return amountOfLayers + 2;
        }

        return amountOfLayers;

    }

    int getEvenSlots(int layer, int previousAmountOfSlots)
    {
        int length = (layer * 2) + 2;
        int previousLength = ((layer - 1) * 2) + 2;
        return (length * length * length) - (previousLength * previousLength * previousLength);
    }

    int getOddSlots(int layer, int previousAmountOfSlots)
    {
        int length = (layer * 2) + 1;
        int previousLength = ((layer - 1) * 2) + 1;
        return (length * length * length) - (previousLength * previousLength * previousLength);
    }

    Vector3 getPosition(int layer)
    {
        int x = Random.Range(0, mapLength );
        int y = Random.Range(0, mapLength );
        int z = Random.Range(0, mapLength );
        int left = 0 + amountOfLayers - layer;
        int right = mapLength - 1 - amountOfLayers + layer;

        while (true)
        {
            if (x == left || x == right || y == left || y == right || z == left || z == right)
            {
                if (left <= x && x <= right && left <= y && y <= right && left <= z && z <= right)
                {
                    if (mapOfNodes[x, y, z] == false)
                    {
                        mapOfNodes[x, y, z] = true;
                        freeSlots[layer] -= 1;
                        return new Vector3(x * spacing, y * spacing, z * spacing);
                    }
                    else
                    {
                        x = rollPosition();
                        y = rollPosition();
                        z = rollPosition();
                    }
                }
                else
                {
                    if (left <= x && x <= right)
                    {
                        x = rollPosition();
                    }
                    if (left <= y && y <= right)
                    {
                        y = rollPosition();
                    }
                    if (left <= z && z <= right)
                    {
                        z = rollPosition();
                    }
                }
            }
            else
            {
                x = rollPosition();
                y = rollPosition();
                z = rollPosition();
            }
        }
        

    }

    int rollPosition()
    {
        return Random.Range(0, mapLength);
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
                //nodes[i].transform.localScale += new Vector3(2*size, 2 * size, 2 * size);
                nodes[i].GetComponent<Renderer>().material = abstractMaterial;
            }
        }
    }

    void spawnLines()
    {
        for (int i = 0; i < nodes.Count; i++)
        {   //nodes.First(NodeGameObject => NodeGameObject.name == nodes[i].GetComponent<NodeGameObject>().getParent()).GetComponent<NodeGameObject>().getisNodeAbstract() != true
            if (nodes[i].GetComponent<NodeGameObject>().getParent() != "") 
            {
                //LineRenderer removeLine = nodes[i].GetComponent<LineRenderer>();
                //Destroy(removeLine);
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

    void setCenter()
    {
        if (odd == true)
        {
            middle = firstObjects[0].transform.position;
        }
        else
        {
            float x = 0;
            float y = 0;
            float z = 0;
            for(int i = 0; i < 8; i++)
            {
                x += firstObjects[i].transform.position.x;
                y += firstObjects[i].transform.position.y;
                z += firstObjects[i].transform.position.z;
            }
            x = x / 8;
            y = y / 8;
            z = z / 8;
            middle = new Vector3(x, y, z);


        }
        centerObject = Instantiate(centerPrefab, middle, Quaternion.identity);
        Transform centerTransform = centerObject.transform;
        for(int i = 0; i < nodes.Count; i++)
        {
            nodes[i].transform.SetParent(centerTransform);
        }
    }

    void spawnPlatform()
    {
        for (int i = 3; i < mapLength + 1; i++)
        {
            platformSize += 4.5f;
        }
        GameObject platform = Instantiate(platformPrefab, middle, Quaternion.identity);
        platform.transform.localScale = new Vector3(platformSize, platformSize, platformSize);
        platform.AddComponent<MeshCollider>();

        Vector3 teleportOffset = new Vector3(middle.x, middle.y + 0.06f, middle.z);
        GameObject teleportPlatform = Instantiate(platformPrefab, teleportOffset, Quaternion.identity);
        teleportPlatform.transform.localScale = new Vector3(platformSize, platformSize, platformSize);
        teleportPlatform.AddComponent<MeshCollider>();
        teleportPlatform.AddComponent<Valve.VR.InteractionSystem.TeleportArea>();
    }

    void setSpawnPosition()
    {
        GameObject thePlayer = GameObject.Find("Player");
        thePlayer.transform.position = new Vector3(middle.x + platformSize, middle.y, middle.z);
    }
}
