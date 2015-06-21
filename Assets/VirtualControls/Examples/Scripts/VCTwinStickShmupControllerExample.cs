using UnityEngine;
using System.Collections;

public class VCTwinStickShmupControllerExample : MonoBehaviour 
{
	public VCAnalogJoystickBase moveJoystick;
	public VCAnalogJoystickBase rotateJoystick;
	
	public GameObject movingGo; 	// the gameobject that moves (e.g. the player)
	public GameObject rotatingGo;	// the gameobject that rotates (e.g. the gun, top half of body, etc.)
	
	public Transform bulletContainer; // gameobject in which we nest bullet children (to prevent scene from getting cluttered)
	public Transform bulletSpawnPoint; // where the bullets appear from
	
	public float moveSpeed = 10.0f; // scalar to apply for movement speed
	
	private float _lastShotTime = 0.0f; // the time when we last shot
	public float shotInterval = .5f; // how often we fire
	public float shotSpeed = 200.0f; // how fast bullets travel
	
	// Use this for initialization
	void Start () 
	{
		if (moveJoystick == null || rotateJoystick == null)
		{
			Debug.LogError("Please assign both moveJoystick and rotateJoystick via the inspector before using.  Disabling for now.");
			this.enabled = false;
			return;
		}
		
		if (movingGo == null || rotatingGo == null)
		{
			Debug.LogError("Please assign both movingGo and rotatingGo via the inspector before using.  Disabling for now.");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// move the moving object
		movingGo.transform.Translate(moveJoystick.AxisX * moveSpeed * Time.deltaTime, 0.0f, moveJoystick.AxisY * moveSpeed * Time.deltaTime);
		
		// set the rotation of the rotating part
		rotatingGo.transform.localEulerAngles = new Vector3(0.0f, Mathf.Atan2(rotateJoystick.AxisX, rotateJoystick.AxisY) * Mathf.Rad2Deg, 0.0f);
		
		// if the rotateJoystick is being used, and enough time has passed
		if (rotateJoystick.MagnitudeSqr > .01f && Time.timeSinceLevelLoad >= _lastShotTime + shotInterval)
		{
			_lastShotTime = Time.timeSinceLevelLoad;
			
			// fire a shot
			
			// bxb ses note: this is not the best way to spawn a rapid amount of projectiles.  You'd be better off spawning them ahead
			// of time and then pooling them for reuse instead of creating a new one each time, but this is good enough for this example.
			GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			bullet.AddComponent<Rigidbody>();
			bullet.transform.localScale = new Vector3(.25f, .25f, .25f);
			bullet.transform.parent = bulletContainer;
			bullet.transform.position = bulletSpawnPoint.position;
			
			Vector2 direction = new Vector2(rotateJoystick.AxisX, rotateJoystick.AxisY).normalized;
			direction *= shotSpeed;
			
			bullet.GetComponent<Rigidbody>().AddForce(direction.x, 0.0f, direction.y);
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 70, 600, 20), "Left side of screen handles movement, right side of screen handles rotation + firing.");
		GUI.Label(new Rect(10, 90, 600, 20), "Check the \"Hide Moving Part\" Option of a Joystick to set it completely invisible.");
	}
}
