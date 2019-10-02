using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private decimal? weight;
    public int depth;
    private string color = "red";
    private bool hasChildren = false;
    private Guid id;
    public Child jsonData;

    public void setId(Guid id)
    {
        this.id = id;
    }
    public Guid getId()
    {
        return id;
    }

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

    public int getSize()
    {
        return size;
    }
    public void setSize(int size)
    {
        this.size = size;
    }

    public decimal? getWeight()
    {
        return weight;
    }
    public void setWeight(decimal? weight)
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
    public decimal? weight { get; set; }
    //public Guid id = Guid.NewGuid();
}

public class RootObject 
{
    public string name { get; set; }
    public List<Child> children { get; set; }
}
