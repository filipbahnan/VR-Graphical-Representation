using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;
using SimpleFileBrowser;
public class ActionScript : MonoBehaviour
{

    
    // a reference to the action
    public SteamVR_Action_Boolean clickNodeOnOff;
    public SteamVR_Action_Boolean resetOnOff;
    public SteamVR_Action_Boolean fileExplorerOnOFf;
    public SteamVR_Action_Vector2 rotateNodes;
    // a reference to the hand
    public SteamVR_Input_Sources rightController;
    public SteamVR_Input_Sources leftController;
    private static string thePath;



    void Start()
    {
        clickNodeOnOff.AddOnStateDownListener(TriggerDown, rightController);
        clickNodeOnOff.AddOnStateUpListener(TriggerUp, rightController);

        rotateNodes.AddOnAxisListener(TouchTouchPad, rightController);

        resetOnOff.AddOnStateDownListener(resetDown, leftController);
        fileExplorerOnOFf.AddOnStateDownListener(fileDown, rightController);


    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");
        GameObject spawner = GameObject.Find("Spawner");
        spawner.GetComponent<Spawn_nodes>().thePath = FileBrowser.Result;
        spawner.GetComponent<Spawn_nodes>().startFunction();
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
        spawner.GetComponent<Spawn_nodes>().reset();

    }
    public void fileDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        FileBrowser.SetDefaultFilter("json");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    private void TouchTouchPad(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 theAxis, Vector2 theDelta)
    {
        Debug.Log("fungerar det???");
        GameObject center = GameObject.Find("center(Clone)");
        Vector3 theRotaion = new Vector3(theAxis.x, theAxis.y, 0);
        center.transform.Rotate(theRotaion);
    }
}
