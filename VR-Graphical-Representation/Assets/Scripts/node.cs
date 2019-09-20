using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isNodeAbstract;
    public string parent;

    public bool getisNodeAbstract()
    {
        return isNodeAbstract;
    }
    public void setisNodeAbstract(bool isNodeAbstract)
    {
        this.isNodeAbstract = isNodeAbstract;
    }

    public string getParent()
    {
        return parent;
    }
    public void setParent(string parent)
    {
        this.parent = parent;
    }
}
