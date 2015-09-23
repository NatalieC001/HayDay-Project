using UnityEngine;
using System.Collections;

[System.Serializable]
public class CowController : GameController
{
    public Cow cow;
	new GameObject playerGO;
    Animation anim;
    CameraController cameraControl;
    UIFarm userInterface;
    Vector3 targetDestination;
    Movement movement;
    VCAnalogJoystickBase joyStick;

	bool idleRunning;
    bool inMart = false;

	public float speed = 6;
	public float rotationSpeed = 5;
	public string state;
	public AudioClip cowSound;

    void Start()
    {
        anim = GetComponent<Animation>();
        userInterface = GameObject.FindGameObjectWithTag("UI").GetComponent<UIFarm>();
		cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        joyStick = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<VCAnalogJoystickBase>();

        if (!(Application.loadedLevelName.Equals("Mart")))
        {
            state = "wander";
            playerGO = GameObject.Find("Player");
            movement = playerGO.GetComponent<Movement>();
        }
        else
        {
            state = "wander";
            inMart = true;
        }
    }
    
    void Update()
    {
        print(state);
        switch (state)
        {
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
            
		}
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    IEnumerator Idle(int seconds)
    {
        idleRunning = true;
        anim.Play("idle2");
        yield return new WaitForSeconds(seconds);
        if (state == "idle")
        {
            state = "wander";
            idleRunning = false;
        }
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

        GetComponent<AudioSource>().PlayOneShot(cowSound, 0.9f);

        targetDestination = new Vector3(transform.position.x + Random.Range(-10, 10), 0f, transform.position.z + Random.Range(-10, 10));
        targetDestination.y = Terrain.activeTerrain.SampleHeight(targetDestination);
		
        state = "moving";
    }

    public void MoveTo(Vector3 newDestination)
    {
        targetDestination = newDestination;
        state = "moving";
        idleRunning = false;
    }

    public void Moving()
    {
        if (Vector3.Distance(transform.position, targetDestination) < 1)
        {
            state = "idle";
        }
        else if (targetDestination != Vector3.zero)
        {

            if (!Physics.Raycast(transform.position, (transform.position - targetDestination).normalized, 3))
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }

            anim.Play("walk");
            Quaternion lookRotation;
            Vector3 direction;

            direction = (targetDestination - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            Vector3 yVec = Vector3.zero;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            yVec.y += 1;

            if (!Physics.Raycast(transform.position, transform.forward, 3))
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                state = "idle";
            }
        }
    }


    Vector3 findPath(Vector3 position,Vector3 targetDestination, int count){

        if (!Physics.Raycast(position, targetDestination, 3))
        {
            return targetDestination;
        }
        else if (count>5)
        {
            targetDestination.y /= 2;
            targetDestination.x /= 2;

            findPath(position, targetDestination, count);
            print("New heading" + targetDestination);
        }

        return Vector3.zero;
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
        }
    }

    public Vector3 ReturnPosition()
    {
        return this.gameObject.transform.position;
    }

    void OnMouseDown()
    {

        Vector3 position;
        Vector3 target;

        joyStick.gameObject.SetActive(false);
		if (inMart)
			return;

        state = "following";

        movement.lookAt(this.gameObject);
   
        position =  playerGO.transform.position - transform.position;
        position = position.normalized * 6;
        target = position;
        position += playerGO.transform.position;
        target = playerGO.transform.position - target;
        position.y = transform.position.y + 6;
 
        findPath(transform.position, (transform.position - playerGO.transform.position).normalized, 0);

        cameraControl.MoveToLookAt(position, target);

        userInterface.cow = cow;
		userInterface.cowGameObject = this.gameObject;


		if(userInterface.cowGameObject == null)
			Debug.Log ("User Interface is null!");

		if(userInterface.cow == null)
			Debug.Log ("Cow is null!");

		userInterface.cowUI = true;
		userInterface.playerUI = false;
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
                break;
        }
    }
}