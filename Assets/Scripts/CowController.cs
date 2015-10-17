using UnityEngine;
using System.Collections;

[System.Serializable]
public class CowController : GameController
{
    private GameObject playerGO;
	private Animation anim;
	private CameraController cameraControl;
	private Movement movement;
	private VCAnalogJoystickBase joyStick;

	private float width;
	private float heigth;

	private bool cowSelected;
	private string currScene;
	private bool idleRunning;

	public Vector3 targetDest;
	public Vector3 finalDest;

    public float speed = 6;
    public float rotationSpeed = 50f;
    public string state;
    public AudioClip cowSound;

	public Cow cow;

	public UIFarm userInterface;

    void Start()
    {
        width = this.gameObject.GetComponent<Collider>().bounds.size.x;
        heigth = this.gameObject.GetComponent<Collider>().bounds.size.x;
        anim = GetComponent<Animation>();
        userInterface = GameObject.FindGameObjectWithTag("UI").GetComponent<UIFarm>();
        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        joyStick = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<VCAnalogJoystickBase>();
        playerGO = GameObject.Find("Player");
        movement = playerGO.GetComponent<Movement>();
		currScene = Application.loadedLevelName;
    }

    void Update()
    {
        switch (state)
        {
            case "waiting":
                anim.Play("idle2");
                break;
            case "moving":
                Moving();
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
                Follow(playerGO.transform.position);
                break;
			case "":
				state = "idle";
			break;
        }
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    IEnumerator Idle(int seconds)
    {
		idleRunning = true;
		anim.Play("idle2");
		
		yield return new WaitForSeconds(seconds);
		
		if (state == "idle" || state == "idle2")
		{
			state = "wander";
			idleRunning = false;
		}
	}
	
	public void Wait()
	{
		state = "waiting";
    }

    public void Wander()
    {
        anim.Play("walk");

		int playSound = Random.Range (1, 5);
		
		switch(playSound)
		{
			case 1:
				StartCoroutine(CowMoo(Random.Range(12, 60)));
				break;
			case 2:
				StartCoroutine(CowMoo(Random.Range(16, 60)));
				break;
			case 3:
				StartCoroutine(CowMoo(Random.Range(20, 60)));
				break;
			case 4:
				StartCoroutine(CowMoo(Random.Range(24, 60)));
				break;
			case 5:
				StartCoroutine(CowMoo(Random.Range(28, 60)));
				break;
		}

        finalDest = new Vector3(transform.position.x + Random.Range(-10, 10), 0f, transform.position.z + Random.Range(-10, 10));
        finalDest.y = Terrain.activeTerrain.SampleHeight(targetDest);

        state = "moving";
    }

    public void MoveTo(Vector3 Destination)
    {
        finalDest = Destination;
        targetDest = Destination;
        state = "moving";
    }

    Vector3 lastDirection;
    Vector3 oldAngle = Vector3.zero;

    Vector3 findPath(Vector3 position, Vector3 direction)
    {
        int angle = (int)(Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
        float[] distArray = new float[360];

        RaycastHit hit;   
        Collider rBL;
        Collider rBR;

        Physics.Raycast(position, direction, out hit);
        float lastDistLeft = hit.distance;
        float lastDistRight = hit.distance;

        Vector3 closestDirection = direction;
        float smalestDist = Vector3.Distance(hit.point, finalDest);

        Vector3 leftSide = transform.position;
        Vector3 rightSide = transform.position;

        leftSide.z += width / 2;
        rightSide.z -= width / 2;

        rBL = hit.collider;
        rBR = hit.collider;
      
        direction.y = 0;
		
        for (int i = 0; i < 180; i++)
        {
            direction.x = Mathf.Cos((angle + i) * Mathf.Deg2Rad);
            direction.z = Mathf.Sin((angle + i) * Mathf.Deg2Rad);

            if (oldAngle == Vector3.zero || Vector3.Distance(oldAngle, direction) > .8)
            {
            	Physics.Raycast(rightSide, direction, out hit);
                              
                if (hit.collider != rBL)
                {
                    if (hit.distance > lastDistLeft + width)
                    {                          
                        direction.x = Mathf.Cos((angle + i +5) * Mathf.Deg2Rad);
                        direction.z = Mathf.Sin((angle + i + 5) * Mathf.Deg2Rad);
                        lastDistLeft += width*2;
                 
                        oldAngle = -direction;
                        return direction * lastDistLeft;
                    }
                    rBL = hit.collider;
                }
                          
                lastDistLeft = hit.distance;
            }

            if(Vector3.Distance(hit.point,finalDest) < smalestDist)
            {
                smalestDist = Vector3.Distance(hit.point, finalDest);
                closestDirection = direction;
            }

            direction.x = Mathf.Cos((angle - i) * Mathf.Deg2Rad);
            direction.y = 0;
            direction.z = Mathf.Sin((angle - i) * Mathf.Deg2Rad);

            if (oldAngle == Vector3.zero || Vector3.Distance(oldAngle, direction) > .8)
            {
            	Physics.Raycast(leftSide, direction, out hit);

                if (hit.collider != rBR)
                {
                    if (hit.distance > lastDistRight + width)
                    {                    
                        direction.x = Mathf.Cos((angle - i - 5) * Mathf.Deg2Rad);
                        direction.z = Mathf.Sin((angle - i -5) * Mathf.Deg2Rad);
                        oldAngle = -direction;
                        lastDistRight += width * 2;
                        return direction * lastDistRight;
                    }
                    rBR = hit.collider;
                }
                
                lastDistRight = hit.distance;
            }

            if (Vector3.Distance(hit.point, finalDest) < smalestDist)
            {
                smalestDist = Vector3.Distance(hit.point, finalDest);
                closestDirection = direction;
            }
        }
        return closestDirection * (smalestDist -2);
    }

    public void Moving()
    {
		if(currScene.Equals("Farm"))
		{
			idleRunning = false;

	        if (Vector3.Distance(transform.position, targetDest) < 2)
	        {
	            targetDest = finalDest;
				state = "idle";
	        }

	        if (Physics.Raycast(transform.position, (targetDest - transform.position).normalized, Vector3.Distance(transform.position, targetDest)) || Physics.Raycast(transform.position, transform.forward, 2))
	        {
	            targetDest = transform.position + findPath(transform.position, (finalDest - transform.position).normalized);
	        }
			
			if (targetDest != Vector3.zero)
	        {
	            anim.Play("walk");
	            Quaternion lookRotation;
	            Vector3 direction;

	            direction = (targetDest - transform.position).normalized;

	            lookRotation = Quaternion.LookRotation(direction);

	            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
	            Vector3 yVec = Vector3.zero;
	            Vector3 fwd = transform.TransformDirection(Vector3.forward);
	            yVec.y += 1;

	            transform.position += transform.forward * speed * Time.deltaTime;
	        }
		}
		else if(currScene.Equals("Mart"))
		{
			if (Vector3.Distance(transform.position, finalDest) < 2)
			{
				state = "idle";
				Wait ();
				return;
			}

			if (Vector3.Distance(transform.position, targetDest) < 2)
			{
				targetDest = finalDest;
			}
			
			if (Physics.Raycast(transform.position, (targetDest - transform.position).normalized, Vector3.Distance(transform.position, targetDest)) || Physics.Raycast(transform.position, transform.forward, 2))
			{
				targetDest = transform.position + findPath(transform.position, (finalDest - transform.position).normalized);
			}
			
			if (targetDest != Vector3.zero)
			{
				anim.Play("walk");
				Quaternion lookRotation;
				Vector3 direction;
				
				direction = (targetDest - transform.position).normalized;
				
				lookRotation = Quaternion.LookRotation(direction);
				
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
				Vector3 yVec = Vector3.zero;
				Vector3 fwd = transform.TransformDirection(Vector3.forward);
				yVec.y += 1;
				
				transform.position += transform.forward * speed * Time.deltaTime;
			}
		}
    }

    public void Follow(Vector3 goToPosition)
    {
        anim.Play("walk");
        Vector3 targetPosition = goToPosition;

        targetPosition += playerGO.transform.forward * 7;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        transform.position += transform.forward * 4 * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 1)
        {
            transform.LookAt(playerGO.transform);
            state = "lookingAtPlayer";
			StartCoroutine(WaitFor(20f, 1));
        }
    }

    public Vector3 ReturnPosition()
    {
        return this.gameObject.transform.position;
    }

    void OnMouseDown()
    {
		if (!currScene.Equals("Farm") || GlobalVars.cowSelected)
			return;
		
		Vector3 position;
		Vector3 target;
		
		GlobalVars.cowSelected = true;
		
		joyStick.gameObject.SetActive(false);
		state = "following";
		
		movement.lookAt(this.gameObject);
		
		position = playerGO.transform.position - transform.position;
		position = position.normalized * 6;
		target = position;
		position += playerGO.transform.position;
		target = playerGO.transform.position - target;
		target.y = transform.position.y;
		position.y = transform.position.y + 6;
		
		cameraControl.MoveToLookAt(position, target);
		
		userInterface.cow = cow;
		userInterface.cowGameObject = this.gameObject;

		if (userInterface.cowGameObject == null)
			Debug.Log("User Interface is null!");
		
		if (userInterface.cow == null)
			Debug.Log("Cow is null!");
		
		GlobalVars.cowUI = true;
		GlobalVars.playerUI = false;
    }

    IEnumerator WaitFor(float seconds, int option)
    {
        yield return new WaitForSeconds(seconds);

        switch (option)
        {
            case 0:
                state = "idle";
                break;
            case 1:
				state = "wander";
                break;
        }
    }

	IEnumerator CowMoo(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		
		GetComponent<AudioSource>().PlayOneShot(cowSound, Random.Range(0.4f, 0.7f));
	}
}