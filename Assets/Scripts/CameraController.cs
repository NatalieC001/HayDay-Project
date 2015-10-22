using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float speed = 10;
	public bool watchTarget;
	public bool followTarget;
	public GameObject target;

    float startTime;
    float journeyLength;

    Vector3 difVec;
    Vector3 startingAngle;
    Vector3 endingAngle;
    Vector3 defaultPosition;
    Vector3 startPositon;
    Vector3 endPosition;

	public bool transitioning;

    float playerY = 0;

    void Start()
    {
        target = GameObject.Find("Player");
        difVec = transform.position - target.transform.position;
        defaultPosition = transform.position;
        StartCoroutine(average());
    }

    IEnumerator average()
    {
        for(int i = 0; i < 10; i++)
        {       
            playerY += target.transform.position.y;
            yield return new WaitForSeconds(.01f);
        }
        playerY = playerY / 10; 
    }

    public void MoveToLookAt(Vector3 position, Vector3 target)
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, position);

        startPositon = transform.position;
        startingAngle = transform.forward;

        endPosition = position;
        endingAngle = (target - position).normalized;

        transitioning = true;
        watchTarget = false;
        followTarget = false;
    }

    public void LookAt(Vector3 target)
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(target -transform.position, transform.forward)/4;

        startPositon = transform.position;
        startingAngle = transform.forward;
        endingAngle = (target - transform.position).normalized;

        transitioning = true;
        watchTarget = false;
        followTarget = false;
    }



    public void WatchTarget(GameObject target)
    {
        this.target = target;
        LookAt(target.transform.position);
        watchTarget = true;
    }

    public void WatchPlayer()
    {
        target = GameObject.Find("Player");
        Vector3 playerLoc = target.transform.position;
        playerLoc.y = playerY;
        MoveToLookAt(defaultPosition, playerLoc);
        watchTarget = true;
    }



    public void FollowPlayer()
    {
        target = GameObject.Find("Player");
        Vector3 playerLoc = target.transform.position;
        playerLoc.y = playerY;
        MoveToLookAt(playerLoc+difVec, playerLoc);
        followTarget = true;
    }

    void LateUpdate()
    {   
        if (transitioning)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracComplete = distCovered / journeyLength;

            if (fracComplete <= 1)
            {
                transform.position = Vector3.Lerp(startPositon, endPosition, fracComplete);
                transform.forward = Vector3.Slerp(startingAngle, endingAngle, fracComplete);
            }
            else
            {
                transform.position = Vector3.Lerp(startPositon, endPosition, 1);
                transform.forward = Vector3.Slerp(startingAngle, endingAngle, 1);
                transitioning = false;
            }
        }
        else if (watchTarget)
        {
            transform.LookAt(target.transform.position);
        }
        else if (followTarget)
        {
            transform.position = target.transform.position + difVec;
        }
    }
}