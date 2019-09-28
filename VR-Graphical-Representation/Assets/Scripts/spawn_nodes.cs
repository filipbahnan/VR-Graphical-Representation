using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Valve.VR.InteractionSystem;
using System;


public class Spawn_nodes : MonoBehaviour
{
    public GameObject prefab;
    public GameObject platformPrefab;
    public GameObject centerPrefab;
    private GameObject centerObject;
    private GameObject platform;
    GameObject teleportPlatform;
    public float nodeSize;
    private List<GameObject> nodes = new List<GameObject>();
    public Material noChildrenMaterial;
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
    public GameObject chosenNode;


    // Start is called before the first frame update
    void Start()
    {
        GameObject bajs = new GameObject();
        read();
        setNodes();
        positionNodes();
        spawnLines();
        setCenter();
        spawnPlatform();
        setSpawnPosition();
        Debug.Log(nodes.Count);

    }

    private void Update()
    {
        chooseNode(chosenNode);
    }

    public void read()
    {
        string filename = "hib_hotspot_proto.json";
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + filename);
        RootObject myJsonObject = JsonConvert.DeserializeObject<RootObject>(jsonString);
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
        GameObject theRoot = Instantiate(prefab, spawnPosition, Quaternion.identity);
        theRoot.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = theRoot.GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 6; i++)
            textObject[i].text = myJsonObject.name;
        theRoot.AddComponent<RootNode>();
        theRoot.GetComponent<RootNode>().setRootName(myJsonObject.name);
        nodes.Add(theRoot);
        for (int i = 0; i < myJsonObject.children.Count; i++)
        {
            //setId(myJsonObject.children[i]);
            int depthLimit = 0;
            int currentDepth = 0;
            theRoot.GetComponent<RootNode>().children.Add(createChildren(myJsonObject.children[i], depthLimit, currentDepth));
        }
    }
    /*
    public void setId(Child node)
    {
        node.id = Guid.NewGuid();
        Debug.Log(node.id);
        for(int i = 0; i < node.children.Count; i++)
        {
            setId(node.children[i]);
        }
    }*/

    void chooseNode(GameObject chosenNode)
    {
        //kolla om barnen till noden inte har barn, om do inte har det måste if i create children ändras till att kunna visa alla noder!
        resetEverything();
        createChildren(chosenNode.GetComponent<ChildNode>().jsonData, chosenNode.GetComponent<ChildNode>().depth + 3, chosenNode.GetComponent<ChildNode>().depth);
        destroyObjects();
        setNodes();
        positionNodes();
        spawnLines();
        setCenter();
        spawnPlatform();
        setSpawnPosition();
        Destroy(chosenNode);
    }
    void resetEverything()
    {
        nodes.Clear();
        Array.Clear(mapOfNodes, 0, mapOfNodes.Length);
        Array.Clear(freeSlots, 0, freeSlots.Length);
        Array.Clear(firstObjects, 0, firstObjects.Length);
        firstObjects = new GameObject[8];
        mapLength = 0;
        amountOfLayers = 0;
        platformSize = 14f;
    }
    void destroyObjects()
    {
        Destroy(centerObject);
        Destroy(platform);
        Destroy(teleportPlatform);
    }
    
    GameObject createChildren(Child theNodeData, int depthLimit, int currentDepth)
    {
        currentDepth++;
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
        GameObject theChild = Instantiate(prefab, spawnPosition, Quaternion.identity);
        theChild.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = theChild.GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 6; i++)
            textObject[i].text = theNodeData.name;
        theChild.AddComponent<ChildNode>();
        theChild.GetComponent<ChildNode>().depth = currentDepth;
        theChild.GetComponent<ChildNode>().setNodeName(theNodeData.name);
        nodes.Add(theChild);
        theChild.GetComponent<ChildNode>().setHasChildren(checkIfHasChildren(theNodeData));
        theChild.GetComponent<ChildNode>().jsonData = theNodeData;
        //theChild.GetComponent<ChildNode>().setId(theNodeData.id);
        //Debug.Log(theNodeData.id);

        if (theNodeData.children.Count == 0)
        {
            int parsedInt = 0;
            if (int.TryParse(theNodeData.size, out parsedInt))
            {
                // Code for if the string was valid
                theChild.GetComponent<ChildNode>().setSize(parsedInt);
            }
            else
            {
                // Code for if the string was invalid
                theChild.GetComponent<ChildNode>().setSize(0);
            }
            theChild.GetComponent<ChildNode>().setWeight(theNodeData.weight);
        }
        if (/*theNodeData.children.Count < 4 &&*/ currentDepth <= depthLimit)
        {
            for (int i = 0; i < theNodeData.children.Count; i++)
            {
                theChild.GetComponent<ChildNode>().children.Add(createChildren(theNodeData.children[i],depthLimit, currentDepth));
            }
        }
        return theChild;
    }

    bool checkIfHasChildren(Child theNodeData)
    {
        if(theNodeData.children.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /*
    Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    GameObject newObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
    newObj.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
    TextMesh[] textObject = newObj.GetComponentsInChildren<TextMesh>();

    for (int i = 0; i < 6; i++)
        textObject[i].text = theNode.name;

    newObj.AddComponent<Node>();
    newObj.name = theNode.name.Trim();
    for(int i = 0; i < theNode)
    if()
    for(int i = 0; i < theNode.children.Count; i++)
    {
        newObj.GetComponent<Node>().children.Add(theNode.children[i]);
    }

    int parsedInt = 0;
    if (int.TryParse(theNode.size, out parsedInt))
    {
        // Code for if the string was valid
        newObj.GetComponent<Node>().setSize(parsedInt);
    }
    else
    {
        // Code for if the string was invalid
        newObj.GetComponent<Node>().setSize(0);
    }

    nodes.Add(newObj);
    */



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
            string color;
            if (nodes[x].GetComponent<ChildNode>() == null)
            {
                color = nodes[x].GetComponent<RootNode>().getColor();
            }
            else
            {
                color = nodes[x].GetComponent<ChildNode>().getColor();
            }
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
        int x = UnityEngine.Random.Range(0, mapLength );
        int y = UnityEngine.Random.Range(0, mapLength );
        int z = UnityEngine.Random.Range(0, mapLength );
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
        return UnityEngine.Random.Range(0, mapLength);
    }

    void setNodes()
    {/*
        for (int i = 0; i < nodes.Count; i++)
        {
            if(nodes[i].GetComponent<ChildNode>().getParent() != "")
            {
                GameObject theParent = nodes.First(NodeGameObject => NodeGameObject.name == nodes[i].GetComponent<Node>().getParent());
                theParent.GetComponent<Node>().addChild();
            }
        }
        */
        for (int i = 0; i < nodes.Count; i++)
        {   if(nodes[i].GetComponent<ChildNode>() != null)
            {
                if (nodes[i].GetComponent<ChildNode>().getHasChildren() == false)
                {
                    nodes[i].GetComponent<Renderer>().material = noChildrenMaterial;
                }
            }
        }
        
    }

    void spawnLines()
    {
        for (int i = 0; i < nodes.Count; i++)
        {   
            if(nodes[i].GetComponent<ChildNode>() == null)
            {     
                for (int x = 0; x < nodes[i].GetComponent<RootNode>().children.Count; x++)
                {
                    GameObject lineObj = new GameObject();
                    lineObj.transform.position = nodes[i].transform.position;
                    LineRenderer relation = lineObj.AddComponent<LineRenderer>();
                    relation.material = new Material(Shader.Find("Sprites/Default"));
                    Vector3[] positions = new Vector3[2];
                    positions[0] = new Vector3(0, 0, 0);
                    positions[1] = nodes[i].GetComponent<RootNode>().children[x].transform.position;
                    Transform transformOfChild = nodes[i].transform;
                    positions[1] = transformOfChild.InverseTransformPoint(positions[1]);
                    relation.widthMultiplier = 0.2f;
                    relation.SetPositions(positions);
                    relation.useWorldSpace = false;
                    lineObj.transform.SetParent(nodes[i].transform);
                    lineObj.transform.localScale = new Vector3(1, 1, 1);

                }
            }
            else 
            {
                for (int x = 0; x < nodes[i].GetComponent<ChildNode>().children.Count; x++)
                {
                    GameObject lineObj = new GameObject();
                    lineObj.transform.position = nodes[i].transform.position;
                    LineRenderer relation = lineObj.AddComponent<LineRenderer>();
                    relation.material = new Material(Shader.Find("Sprites/Default"));
                    Vector3[] positions = new Vector3[2];
                    positions[0] = new Vector3(0, 0, 0);
                    positions[1] = nodes[i].GetComponent<ChildNode>().children[x].transform.position;
                    Transform transformOfChild = nodes[i].transform;
                    positions[1] = transformOfChild.InverseTransformPoint(positions[1]);
                    relation.widthMultiplier = 0.2f;
                    relation.SetPositions(positions);
                    relation.useWorldSpace = false;
                    lineObj.transform.SetParent(nodes[i].transform);
                    lineObj.transform.localScale = new Vector3(1, 1, 1);
                }

            }
            
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
        platform = Instantiate(platformPrefab, middle, Quaternion.identity);
        platform.transform.localScale = new Vector3(platformSize, platformSize, platformSize);
        platform.AddComponent<MeshCollider>();

        Vector3 teleportOffset = new Vector3(middle.x, middle.y + 0.06f, middle.z);
        teleportPlatform = Instantiate(platformPrefab, teleportOffset, Quaternion.identity);
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
