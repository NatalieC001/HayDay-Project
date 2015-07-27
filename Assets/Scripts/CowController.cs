using UnityEngine;
using System.Collections;

public class CowController : MonoBehaviour
{
	// Farm Vars
    GameObject player;
    Animation _animation;
    Animation anim;
    CameraController camera;
    Vector3 cameraPosition;
    UI Userinterface;
    bool idleRunning;
	Vector3 targetDestination;
    Movement movement;
    public float speed = 3;
    public float rotationSpeed = 10;
	public string state;

	// Mart Vars
	bool inMart = false;
	bool cowInRing = false;
	string martState;

    void Start()
    {
        state = "wander";

        player = GameObject.Find("Player");
        movement = player.GetComponent<Movement>();
        anim = GetComponent<Animation>();

		if (Application.loadedLevelName.Equals ("Mart"))
			inMart = true;
    }

    // Update is called once per frame
    void Update()
    {
		if(!inMart)
		{
	        switch (state)
	        {
	            case "moving":
					MoveTo(targetDestination);
	            break;
	            case "wander":
	                Wander();
	            break;
	            case "idle":
	                if (!idleRunning)
	                    StartCoroutine(Idle(Random.Range(5, 40)));
	            break;
	            case "lookingAtPlayer":
	                if (movement.currentFocus != this.gameObject)
	                {
	                    state = "wander";
	                }
	                anim.Play("idle3");
	            break;
	            case "following":
	                Follow(player.transform.position);
	            break;
	        }
		}
		else
		{
			if(!cowInRing)
			{
				// Set location for cow to spawn in mart
				Vector3 spawnLocation = new Vector3(0f, 0, 0f);
				CowMaker.GenerateCow(spawnLocation);
				cowInRing = true;
				martState = "enterCow";
			}

			MartControl(martState);
		}
    }

    IEnumerator Idle(int seconds)
    {
        idleRunning = true;
        anim.Play("idle2");
        yield return new WaitForSeconds(seconds);
        state = "wander";
        idleRunning = false;
    }

	IEnumerator IdleOnly(int seconds)
	{
		idleRunning = true;
		anim.Play("idle2");
		yield return new WaitForSeconds(seconds);
		anim.Play("idle3");
		idleRunning = false;
	}

	public void Wander()
    {
		anim.Play("walk");
		targetDestination = new Vector3(transform.position.x + Random.Range(-10, 10), 0f, transform.position.z + Random.Range(-10, 10));
		targetDestination.y = Terrain.activeTerrain.SampleHeight(targetDestination);
        state = "moving";   
    }

	public void MoveTo(Vector3 targetDestinationInput)
	{
		if (Vector3.Distance(transform.position, targetDestinationInput) < 2)
		{
			state = "idle";
			martState = "stopCow";
		}
		else if (targetDestinationInput != Vector3.zero)
		{
			Quaternion lookRotation;
			Vector3 direction;
			
			direction = (targetDestinationInput - transform.position).normalized;
			lookRotation = Quaternion.LookRotation(direction);
			
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
			Vector3 yVec = Vector3.zero;
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
			yVec.y += 1;
			
			if (!Physics.Raycast(transform.position, transform.forward, 2))
			{
				transform.position += transform.forward * speed * Time.deltaTime;
			}
			else
			{
				state = "idle";
				martState = "stopCow";
			}
		}
	}

    public void Follow(Vector3 goToPosition)
    {
        anim.Play("walk");
		Vector3 targetPosition = goToPosition;

        targetPosition += player.transform.forward * 7;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        transform.position += transform.forward * 4 * Time.deltaTime;

        if (Vector3.Distance(transform.position,targetPosition) < 1)
        {
            transform.LookAt(player.transform);
            state = "lookingAtPlayer";
        }
    }

	public void MartControl(string martState)
	{
		switch(martState)
		{
			case "enterCow":
				// Cow will spawn somewhere off screen, then
				// Move cow towards center of the ring
				state = "moving";
				Vector3 bidArea = new Vector3(0f, 0f, 0f);
				MoveTo(bidArea);
			break;
			case "stopCow":
				// Stop cow in center of ring
				// After cow stops, display start bid of cow on screen
				// Stats of the cow will appear somewhere on screen etc
				if(!idleRunning)
					IdleOnly(Random.Range(15, 45));
			break;
			case "exitCow":
				// Bidding has ended, moving cow from the bidding area
				// If player has bought this cow, add it to a list to bring back to farm

				// De-spawn cow after moving cow out of sight
				cowInRing = false;
			break;
			case "biddingCow":
				// Logic for bidding on cow maybe here, bidding time of 1 minute maybe?
			break;
			case "sellingCow":
				// Logic for selling player cow here, bidding time of 1 minute maybe?
			break;
		}
	}

    void OnMouseDown()
    {
        state = "following";

        movement.lookAt(this.gameObject);
        Userinterface = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        camera.lookAt(this.gameObject);
      
        //Finds the cow with the same instance Id as the parent gameobject
        Userinterface.cow = GameController.game.cows.Find(cow => cow.gameObjectID == this.gameObject.GetInstanceID());
        Userinterface.cowGameObject = this.gameObject;

        Userinterface.cowUI = true;
        Userinterface.playerUI = false;
    }
}