using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserInput : MonoBehaviour
{

    public static GameObject currentObject;
    private int currentID;
    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        currentID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Sends out a Raycast and returns an array filled with everything in it
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0f);

        //Goes through all the hit objects and checks if any of them were our button
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            //I use the object id to determine if I have already run the code for this object;
            int id = hit.collider.gameObject.GetInstanceID();

            //If I havent then I will run it again but if I have it is unnecessary to keep running it
            if (currentID != id)
            {
                currentID = id;
                currentObject = hit.collider.gameObject;

                //Checks based off the name
                string name = currentObject.name;
                if (name == "Node")
                {
                    GameObject spawner = GameObject.Find("Spawner");
                    spawner.GetComponent<Spawn_nodes>().chooseNode(currentObject);
                }
            }

        }
    }
}