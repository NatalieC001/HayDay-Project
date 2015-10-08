using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float speed = 10;
	public bool watchPlayer;
	public bool followPlayer;
	public GameObject player;

    float startTime;
    float journeyLength;

    Vector3 difVec;
    Vector3 startingAngle;
    Vector3 endingAngle;
    Vector3 startPositon;
    Vector3 endPosition;

	public bool transitioning;


    float playerY = 0;
    void Start()
    {
        player = GameObject.Find("Player");
        difVec = transform.position - player.transform.position;
        startPositon = transform.position;
        StartCoroutine(average());
    }
    IEnumerator average()
    {
        for(int i = 0; i < 10; i++)
        {       
            playerY += player.transform.position.y;
            yield return new WaitForSeconds(.01f);
        }
        playerY = playerY / 10; 
    }
    public void MoveToLookAt(Vector3 positon, Vector3 target)
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, positon);

        startPositon = transform.position;
        startingAngle = transform.forward;

        endPosition = positon;
        endingAngle = (target - positon).normalized;

        transitioning = true;
        watchPlayer = false;
        followPlayer = false;
    }

    public void WatchPlayer()
    {
        Vector3 playerLoc =  player.transform.position;
        playerLoc.y = playerY;
        MoveToLookAt(startPositon, playerLoc);
        watchPlayer = true;
    }

    public void FollowPlayer()
    {
        Vector3 playerLoc = player.transform.position;
        playerLoc.y = playerY;
        MoveToLookAt(startPositon, playerLoc);
        followPlayer = true;
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
        else if (watchPlayer)
        {
            transform.LookAt(player.transform.position);
        }
        else if (followPlayer)
        {
            transform.position = player.transform.position + difVec;
        }
    }
}