using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Vector3 position;

    public bool closeUpView;
    GameObject player;


    public GameObject currentFocus;


    Vector3 difVec;


    Vector3 startingFocus;
    Vector3 endingFocus;

    Vector3 startingPositon;
    Vector3 endingPosition;

   
    float speed = 10;
    public float startTime;

    public float journeyLength;
   

	void Start () {
     
        player = GameObject.Find("Player");
        transform.LookAt(player.transform.position);
        difVec = transform.position - player.transform.position;
	}





    public void lookAt(GameObject target)
    {
        //just used to check if the gameobject still exists
       // if (!closeUpView)
        //{
            currentFocus = target;
            startTime = Time.time;

            startingPositon = transform.position;
            startingFocus = player.transform.position;
            

            journeyLength = Vector3.Distance(startingPositon, player.transform.position - player.transform.forward * 4 + new Vector3(0f, 5f, 0f));   

            closeUpView = true;
        //}
        //else
        //{
        //    if (currentFocus != null && currentFocus != target)
        //    {
        //        startTime = Time.time;
        //        translationTime = Vector3.Distance(currentFocus.tr, transform.position);



        //    }

        //}
    }


    void resetCamera()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, player.transform.position + difVec);

        startingPositon = transform.position;
        closeUpView = false;
    }

    void LateUpdate()
    {
        if(!closeUpView && Vector3.Distance(transform.position, player.transform.position + difVec)<1)
		{          
            transform.position = player.transform.position + difVec;
		}
        else if (closeUpView)
		{
            if (currentFocus != null)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracComplete = distCovered / journeyLength;

                if (fracComplete < 1)
                {

                    transform.position = Vector3.Lerp(startingPositon, player.transform.position - player.transform.forward * 4 + new Vector3(0f, 5f, 0f), fracComplete);
                    transform.LookAt(Vector3.Slerp(startingFocus, currentFocus.transform.position, fracComplete));

                }
                else
                {
                    startingFocus = currentFocus.transform.position;
                }


                
            }  
            else
            {
                resetCamera();
				HealthBar.SetHealth(0f);
				HappinessBar.SetHappiness(0f);
            }
		}
        else
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(startingPositon, player.transform.position + difVec, fracJourney);
            transform.LookAt(Vector3.Slerp(startingFocus, player.transform.position, fracJourney));
        }
	}

   

}