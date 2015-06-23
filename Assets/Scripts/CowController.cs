using UnityEngine;
using System.Collections;

public class CowController : MonoBehaviour {

    Transform player;
    Animation _animation;
    Animation anim;
    CameraController camera;
    Vector3 cameraPosition;
    UI Userinterface;


    public string state;

    void Start()
    {
        state = "standing";
        player = GameObject.Find("Player").transform;
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

            case "standing":
                anim.Play("idle1");
                break;

            case "following":
                Vector3 position = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.LookAt(position);
                transform.position += transform.forward * 4 * Time.deltaTime;
                anim.Play("walk");
                break;
        }


    }



    void OnMouseDown()
    {
        state = "standing";

        Userinterface = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();


        camera.position = transform.position + transform.forward * 5 + new Vector3(0f, 3f, 0f);
        camera.target = this.gameObject;
        camera.followPlayer = false;


        //Finds the cow with the same instance Id as the parent gameobject
        Userinterface.cow = GameControl.game.cows.Find(cow => cow.gameObjectID == this.gameObject.GetInstanceID());
        Userinterface.cowGameObject = this.gameObject;

        Userinterface.cowUI = true;
        Userinterface.playerUI = false;     

    }
}
