using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;
using SFB;

public class ActionScript : MonoBehaviour
{
    // a reference to the action
    public SteamVR_Action_Boolean clickNodeOnOff;
    public SteamVR_Action_Boolean resetOnOff;
    public SteamVR_Action_Boolean fileExplorerOnOFf;
    public SteamVR_Action_Vector2 rotateNodes;
    public SteamVR_Action_Boolean switchMode;
    public SteamVR_Action_Boolean layer2;
    public SteamVR_Action_Boolean layer4;

    // a reference to the hand
    public SteamVR_Input_Sources rightController;
    public SteamVR_Input_Sources leftController;
    private static string thePath;
    private bool currentModeIsOverView = false;



    void Start()
    {
        clickNodeOnOff.AddOnStateDownListener(TriggerDown, rightController);
        clickNodeOnOff.AddOnStateUpListener(TriggerUp, rightController);

        rotateNodes.AddOnAxisListener(TouchTouchPad, rightController);

        resetOnOff.AddOnStateDownListener(resetDown, leftController);
        fileExplorerOnOFf.AddOnStateDownListener(fileDown, rightController);

        switchMode.AddOnStateDownListener(modeSwitchDown, rightController);
        layer2.AddOnStateDownListener(layer2Down, leftController);
        layer4.AddOnStateDownListener(layer4Down, rightController);


    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            var anotherPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
            GameObject spawner = GameObject.Find("Spawner");
            spawner.GetComponent<Spawn_nodes>().thePath = anotherPath[0];
            spawner.GetComponent<Spawn_nodes>().startFunction();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (currentModeIsOverView == false)
            {
                GameObject spawner = GameObject.Find("Spawner");
                spawner.GetComponent<Spawn_nodes>().reset("overview");
                currentModeIsOverView = true;
            }
            else
            {
                GameObject spawner = GameObject.Find("Spawner");
                spawner.GetComponent<Spawn_nodes>().reset("standard");
                currentModeIsOverView = false;
            }

        }
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
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().reset("standard");

    }
    public void fileDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        var anotherPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().thePath = anotherPath[0];
        spawner.GetComponent<Spawn_nodes>().startFunction();
    }

    private void TouchTouchPad(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 theAxis, Vector2 theDelta)
    {
        GameObject center = GameObject.Find("center(Clone)");
        Vector3 theRotaion = new Vector3(theAxis.x, theAxis.y, 0);
        center.transform.Rotate(theRotaion);
    }

    public void modeSwitchDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (currentModeIsOverView == false)
        {
            GameObject spawner = GameObject.Find("Spawner");
            spawner.GetComponent<Spawn_nodes>().reset("overview");
            currentModeIsOverView = true;
        }
        else
        {
            GameObject spawner = GameObject.Find("Spawner");
            spawner.GetComponent<Spawn_nodes>().reset("standard");
            currentModeIsOverView = false;
        }
    }

    public void layer2Down(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().chosenLimit = 2;
    }
    public void layer4Down(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().chosenLimit = 4;
    }
}
