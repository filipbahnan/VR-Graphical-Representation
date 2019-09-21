﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private bool isNodeAbstract;
    private string parent;
    private int amountOfChildren = 0;

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

    public void addChild()
    {
        amountOfChildren++;
    }
    public int getAmountOfChildren()
    {
        return amountOfChildren;
    }
}
