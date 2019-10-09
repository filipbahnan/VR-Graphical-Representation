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
    public Material rootMaterial;
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
    private float smallestSize = 0;
    private float biggestSize = 0;
    private Child previousNode;
    private RootObject myJsonObject;
    public string thePath;
    private bool firstRead = true;

    // Start is called before the first frame update
    public void startFunction()
    {
        if (firstRead == false)
        {
            resetEverything();
            destroyObjects();
        }
        read();
        setNodes();
        positionNodes();
        spawnLines();
        setCenter();
        spawnPlatform();
        setSpawnPosition();
        firstRead = false;
    }

    public void read()
    {
        jsonString = File.ReadAllText(thePath);
        myJsonObject = JsonConvert.DeserializeObject<RootObject>(jsonString);
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
        GameObject theRoot = Instantiate(prefab, spawnPosition, Quaternion.identity);
        theRoot.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = theRoot.GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 6; i++)
            textObject[i].text = myJsonObject.name;

        for (int i = 6; i < 24; i++)
            textObject[i].text = "";
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

    public void chooseNode(GameObject chosenNode)
    {
        //kolla om barnen till noden inte har barn, om do inte har det måste if i create children ändras till att kunna visa alla noder!
        if (chosenNode.GetComponent<ChildNode>().jsonData.children.Count != 0)
        {
            resetEverything();
            int limit;
            if (previousNode == chosenNode.GetComponent<ChildNode>().jsonData)
            {
                limit = 1;
            }
            else
            {
                limit = 2;
            }
            createChildren(chosenNode.GetComponent<ChildNode>().jsonData, chosenNode.GetComponent<ChildNode>().depth + limit, chosenNode.GetComponent<ChildNode>().depth);
            destroyObjects();
            setNodes();
            positionNodes();
            spawnLines();
            setCenter();
            spawnPlatform();
            setSpawnPosition();
            previousNode = chosenNode.GetComponent<ChildNode>().jsonData;
            Destroy(chosenNode);
        }
        else
        {
            createPhysicalObject(chosenNode);
        }

    }

    void createPhysicalObject(GameObject chosenNode)
    {
        GameObject player = GameObject.Find("Player");
        Vector3 playerPosition = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z);
        GameObject newObj = Instantiate(chosenNode, playerPosition, Quaternion.identity);
        newObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        newObj.GetComponent<Rigidbody>().useGravity = true;
        //newObj.AddComponent<Interactable>();
        //newObj.AddComponent<Throwable>();
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
        smallestSize = 0;
        biggestSize = 0;
}
    void destroyObjects()
    {
        Destroy(centerObject);
        Destroy(platform);
        Destroy(teleportPlatform);
    }

    public void reset()
    {
        resetEverything();
        destroyObjects();
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
        GameObject theRoot = Instantiate(prefab, spawnPosition, Quaternion.identity);
        theRoot.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = theRoot.GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 6; i++)
            textObject[i].text = myJsonObject.name;

        for (int i = 6; i < 18; i++)
            textObject[i].text = "";
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
        setNodes();
        positionNodes();
        spawnLines();
        setCenter();
        spawnPlatform();
        setSpawnPosition();
    }

    GameObject createChildren(Child theNodeData, int depthLimit, int currentDepth)
    {   
        currentDepth++;
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
        GameObject theChild = Instantiate(prefab, spawnPosition, Quaternion.identity);
        theChild.transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
        TextMesh[] textObject = theChild.GetComponentsInChildren<TextMesh>();
        string secondText = "";
        string firstText = "";
        for (int i = 0; i < 6; i++)
        {
            if (theNodeData.name.Length < 20)
            {
                textObject[i].text = theNodeData.name;
            }
            else
            {
                for(int x = 0; x < 19; x++)
                {
                    firstText += theNodeData.name[x];
                }
                firstText += "-";
                for(int x = 19; x < theNodeData.name.Length; x++)
                {
                    secondText += theNodeData.name[x];
                }
                break;
            }
        }
        if(firstText != "")
        {
            for (int i = 0; i < 6; i++)
            {
                textObject[i].text = firstText;
            }
        }

        for (int i = 18; i < 24; i++)
        {
            textObject[i].text = secondText;
        }

        for (int i = 6; i < 12; i++)
        {
            if (theNodeData.size == null)
            {
                textObject[i].text = "";
            }
            else
            {
                textObject[i].text = theNodeData.size.Trim();
            }
        }
        for (int i = 12; i < 18; i++)
        {
            if (theNodeData.weight == null)
            {
                textObject[i].text = "";
            }
            else
            {
                textObject[i].text = theNodeData.weight.ToString();
            }
        }
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
            if (theChild.GetComponent<ChildNode>().getSize() < smallestSize || smallestSize == 0)
            {
                smallestSize = theChild.GetComponent<ChildNode>().getSize();
            }
            if (theChild.GetComponent<ChildNode>().getSize() > biggestSize)
            {
                biggestSize = theChild.GetComponent<ChildNode>().getSize();
            }
        }
        if (currentDepth <= depthLimit)
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

        mapOfNodes = new bool[mapLength, mapLength, mapLength];

        for (int x = 0; x < nodes.Count; x++)
        {
            int j = 0;
            while (freeSlots[j] == 0)
            {
                j++;
            }
            nodes[x].transform.position = getPosition(j);
            if (x < 8)
            {
                firstObjects[x] = nodes[x];
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
    {
        for (int i = 0; i < nodes.Count; i++)
        {   if(nodes[i].GetComponent<ChildNode>() != null)
            {
                if (nodes[i].GetComponent<ChildNode>().getHasChildren() == false)
                {
                    nodes[i].GetComponent<Renderer>().material = noChildrenMaterial;

                    float sizePercentage = (nodes[i].GetComponent<ChildNode>().getSize() - smallestSize) / (biggestSize - smallestSize);
                    float nodeSize = 1f + (2f * sizePercentage);
                    nodes[i].transform.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
                    float theWeight = (float)nodes[i].GetComponent<ChildNode>().getWeight();
                    if(theWeight == 0)
                    {
                        //do nothing
                    }
                    else if (theWeight < 0.1f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(255f/255f, 170f/255f, 0f/255f); 
                    }
                    else if (theWeight < 0.2f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(0f / 255f, 102f / 255f, 51f / 255f);
                    }
                    else if (theWeight < 0.3f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(255f / 255f, 255f / 255f, 0f / 255f);
                    }
                    else if (theWeight < 0.4f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(0f / 255f, 0f / 255f, 255f / 255f);
                    }
                    else if (theWeight < 0.5f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(255f / 255f, 0f / 255f, 255f / 255f);
                    }
                    else if (theWeight < 0.6f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(96f / 255f, 96f / 255f, 96f / 255f);
                    }
                    else if (theWeight < 0.7f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(255f / 255f, 0f / 255f, 0f / 255f);
                    }
                    else if (theWeight < 0.8f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(0f / 255f, 255f / 255f, 0f / 255f);
                    }
                    else if (theWeight < 0.9f)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(255f / 255f, 204f / 255f, 255f / 255f);
                    }
                    else
                    {
                        nodes[i].GetComponent<Renderer>().material.color = new Color(255f / 255f, 255 / 255f, 255f / 255f);
                    }
                }
                if(i == 0)
                {
                    nodes[i].GetComponent<Renderer>().material = rootMaterial;
                }
            }
            else
            {
                nodes[i].GetComponent<Renderer>().material = rootMaterial;
            }
        }
        if (nodes[0].GetComponent<ChildNode>() != null)
        {
            nodes.Sort((a, b) => b.GetComponent<ChildNode>().children.Count.CompareTo(a.GetComponent<ChildNode>().children.Count));
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
                    relation.widthMultiplier = 0.1f;
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
                    relation.widthMultiplier = 0.1f;
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
        teleportPlatform.GetComponent<MeshRenderer>().enabled = false;
    }

    void setSpawnPosition()
    {
        GameObject thePlayer = GameObject.Find("Player");
        thePlayer.transform.position = new Vector3(middle.x + platformSize, middle.y, middle.z);
    }
}
