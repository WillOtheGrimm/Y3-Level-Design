using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMode : MonoBehaviour
{
    [Tooltip("When holding this key down, player will be teleported to where you click with the left mouse button!")]
    public KeyCode HotKey;
    Vector3 teleportLocation;
    [Tooltip("What to add to the indicator location so player doesn't get splinched")]
    public Vector3 teleportOffset = new Vector3(0, 0.5f, 0);
    bool isRayCasting;
    public GameObject indicator;
    GameObject player;

    private void Start()
    {
        indicator.SetActive(false);
        player = GameObject.FindWithTag("Player");
        if(player == null)
        {
            Debug.Log("No player ! God Mode disabled...");
        }
    }

    private void Update()
    {
        if (player == null) return;
        if (Input.GetKey(HotKey))
        {
            indicator.SetActive(true);
            isRayCasting = true;
        }
        else
        {
            indicator.SetActive(false);
            isRayCasting= false;
        }

        if (Input.GetMouseButtonUp(0) && player != null)
        {
            player.transform.position = teleportLocation;
        }

        if (isRayCasting)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                indicator.transform.position = hit.point;
                teleportLocation = hit.point + teleportOffset;
            }
        }

    }
}
