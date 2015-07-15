using UnityEngine;
using System.Collections;

public class CowController : MonoBehaviour
{
    GameObject player;
    Animation _animation;
    Animation anim;
    CameraController camera;
    Vector3 cameraPosition;
    UI Userinterface;
    bool idleRunning;
    public string state;
    Movement movement;

    public float speed = 3;
    public float rotationSpeed = 10;
    Vector3 targetDestination;
        
    void Start()
    {
        state = "wander";

        player = GameObject.Find("Player");
        movement = player.GetComponent<Movement>();

        anim = GetComponent<Animation>();
        anim.Play("walk");
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case "moving":
                if (Vector3.Distance(transform.position, targetDestination) < 2)
                {
                    state = "idle";
                }
                else if (targetDestination != Vector3.zero)
                {
                    Quaternion lookRotation;
                    Vector3 direction;

                    direction = (targetDestination - transform.position).normalized;
                    lookRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    //transform.LookAt(targetDestination);
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
                    }  
                }
            break;
            case "wander":
                wander();
            break;
            case "idle":
                if (!idleRunning)
                    StartCoroutine(idle(Random.Range(5, 40)));
            break;
            case "lookingAtPlayer":
                if (movement.currentFocus != this.gameObject)
                {
                    state = "wander";
                }
                anim.Play("idle3");
            break;
            case "following":
                follow();
            break;
        }
    }

    IEnumerator idle(int seconds)
    {
        idleRunning = true;
        anim.Play("idle2");
        yield return new WaitForSeconds(seconds);
        state = "wander";
        idleRunning = false;
    }

    public void wander()
    {
        state = "moving";
        anim.Play("walk");
        targetDestination = new Vector3(transform.position.x + Random.Range(-10, 10), 0f, transform.position.z + Random.Range(-10, 10));
        targetDestination.y = Terrain.activeTerrain.SampleHeight(targetDestination);
    }

    public void follow()
    {
        anim.Play("walk");
        Vector3 targetPosition = player.transform.position;

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