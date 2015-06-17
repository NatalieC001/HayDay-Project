using UnityEngine;
using System.Collections;

public class  CowMovement : MonoBehaviour {

    Transform player;
    Animation _animation;

    Animation anim;

    public Rigidbody rb;
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();

        player = GameObject.Find("Player").transform;
    
        anim = GetComponent<Animation>();
        anim.Play("walk");
    }
	


	// Update is called once per frame
	void Update () {

      
        Vector3 position = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(position);
        transform.position += transform.forward*4*Time.deltaTime;

	}



}
