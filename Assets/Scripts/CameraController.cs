using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Vector3 position;
    public Vector3 defaultLookat;
    public bool followPlayer;
    GameObject player;
    public GameObject target;
    Vector3 difVec;

	void Start () {
        defaultLookat = transform.forward;
        followPlayer = true;
        player = GameObject.Find("Player");
        difVec = transform.position - player.transform.position;

	}


    void LateUpdate()
    {
	
        if(followPlayer)
            transform.position = player.transform.position + difVec;
                  
        else
            if (target != null)
            {
                transform.position = position;
                transform.LookAt(target.transform);
            }  
            else
            {
                transform.forward = defaultLookat;
                followPlayer = true;
                
            }
	}
}
