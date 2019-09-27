using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class Node : MonoBehaviour
{
    private bool isNodeAbstract ;
    public List<GameObject> children;
    private int amountOfChildren = 0;
    private string state;
    private string color = "red";
    private int size;
    private double weight;

    public string getColor()
    {
        return color;
    }

    public string getState()
    {
        return state;
    }
    public void setisNodeAbstract(string state)
    {
        this.state = state;
    }

    public int getSize()
    {
        return size;
    }
    public void setSize(int size)
    {
        this.size = size;
    }

    public double getWeight()
    {
        return weight;
    }
    public void setWeight(double weight)
    {
        this.weight = weight;
    }
}*/

/*
public class Node
{
    public string @class { get; set; }
    public string parent { get; set; }
    public string @abstract { get; set; }
}

public class RootObject
{
    public List<Node> jsonNodes { get; set; }
}*/

public class RootNode : MonoBehaviour
{
    private string rootName;
    public List<GameObject> children = new List<GameObject>();
    private string color = "red";


    public string getRootName()
    {
        return rootName;
    }
    public void setRootName(string rootName)
    {
        this.rootName = rootName;
    }

    public string getColor()
    {
        return color;
    }
}

public class ChildNode : MonoBehaviour
{
    private string nodeName;
    public List<GameObject> children = new List<GameObject>();
    private int size;
    private double? weight;
    public int depth;
    private string color = "red";
    private bool hasChildren = false;

    public void setHasChildren(bool hasChildren)
    {
        this.hasChildren = hasChildren;
    }
    public bool getHasChildren()
    {
        return hasChildren;
    }

    public string getNodeName()
    {
        return nodeName;
    }
    public void setNodeName(string nodeName)
    {
        this.nodeName = nodeName;
    }

    public int? getSize()
    {
        return size;
    }
    public void setSize(int size)
    {
        this.size = size;
    }

    public double? getWeight()
    {
        return weight;
    }
    public void setWeight(double? weight)
    {
        this.weight = weight;
    }

    public string getColor()
    {
        return color;
    }

}

public class Child 
{
    public string name { get; set; }
    public List<Child> children { get; set; }
    public string size { get; set; }
    public double? weight { get; set; }
}

public class RootObject 
{
    public string name { get; set; }
    public List<Child> children { get; set; }
}
