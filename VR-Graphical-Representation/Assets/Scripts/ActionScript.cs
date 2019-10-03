﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;

public class ActionScript : MonoBehaviour
{

    
    // a reference to the action
    public SteamVR_Action_Boolean clickNodeOnOff;
    // a reference to the hand
    public SteamVR_Input_Sources handType;

    void Start()
    {
        clickNodeOnOff.AddOnStateDownListener(TriggerDown, handType);
        clickNodeOnOff.AddOnStateUpListener(TriggerUp, handType);
    }
    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is up");
    }
    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        GameObject rightHand = GameObject.Find("RightHand");
        rightHand.GetComponent<LaserInput>().rayCastLaser();
    }
}
