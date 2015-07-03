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


        
    void Start()
    {
        state = "idle";

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
            case "wandering":
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
        state = "wandering";
        anim.Play("walk");
        Vector3 targetLocation  = new Vector3(Random.Range(50f, 100f), 0f, Random.Range(223f, 263f));
        
        targetLocation.y = Terrain.activeTerrain.SampleHeight(targetLocation);
        transform.LookAt(targetLocation * Time.deltaTime);
        transform.position += transform.forward * 4 * Time.deltaTime;

        if (transform.position == targetLocation)
        {
            state = "idle";
        }
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