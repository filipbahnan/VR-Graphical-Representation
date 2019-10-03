using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;

public class ActionScript : MonoBehaviour
{

    
    // a reference to the action
    public SteamVR_Action_Boolean clickNodeOnOff;
    public SteamVR_Action_Boolean resetOnOff;
    // a reference to the hand
    public SteamVR_Input_Sources rightController;
    public SteamVR_Input_Sources leftController;

    void Start()
    {
        clickNodeOnOff.AddOnStateDownListener(TriggerDown, rightController);
        clickNodeOnOff.AddOnStateUpListener(TriggerUp, rightController);

        resetOnOff.AddOnStateDownListener(resetDown, leftController);

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

    public void resetDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Keeeeeeeeeeeeeeeeeeeeeeeevin");
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().reset();
        /*
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().*/
    }
}
