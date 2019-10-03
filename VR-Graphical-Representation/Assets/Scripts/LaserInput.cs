using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserInput : MonoBehaviour
{

    public static GameObject currentObject;
    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
    }

    // Update is called once per frame
    public void rayCastLaser()
    {
        //Sends out a Raycast and returns an array filled with everything in it
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        //ray.origin.Set(transform.position.x, transform.position.y, transform.position.z);
        //hit = Physics.RaycastAll(transform.position, transform.forward, 100.0f);

        //Goes through all the hit objects and checks if any of them were our button
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Raycast hit");
            Debug.DrawRay(ray.origin, ray.direction);
            //I use the object id to determine if I have already run the code for this object;
            //int id = hit.collider.gameObject.GetInstanceID();

            //If I havent then I will run it again but if I have it is unnecessary to keep running it
            //if (currentID != id)
            //{
                //currentID = id;
                currentObject = hit.collider.gameObject;

                //Checks based off the name
                string name = currentObject.name;
                Debug.Log("Name: " + name);
                if (name == "Node(Clone)")
                {
                    Debug.Log("antal gånger");

                    GameObject spawner = GameObject.Find("Spawner");
                    spawner.GetComponent<Spawn_nodes>().chooseNode(currentObject);
                }
            //}
        }




        ////Sends out a Raycast and returns an array filled with everything in it
        //RaycastHit[] hit = new RaycastHit[1];
        //hit = Physics.RaycastAll(transform.position, transform.forward, 100.0f);

        ////Goes through all the hit objects and checks if any of them were our button


        ////I use the object id to determine if I have already run the code for this object;
        //int id = hit[0].collider.gameObject.GetInstanceID();

        ////If I havent then I will run it again but if I have it is unnecessary to keep running it
        //if (currentID != id)
        //{
        //    currentID = id;
        //    currentObject = hit[0].collider.gameObject;

        //    //Checks based off the name
        //    string name = currentObject.name;
        //    if (name == "Node(Clone)")
        //    {
        //        Debug.Log("antal gånger");

        //        GameObject spawner = GameObject.Find("Spawner");
        //        spawner.GetComponent<Spawn_nodes>().chooseNode(currentObject);
        //    }
        //}


    }
}