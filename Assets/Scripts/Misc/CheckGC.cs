using UnityEngine;

namespace HayDay
{
	public class CheckGC : MonoBehaviour {

		void Start()
		{
			checkGCExists("GameController");
		}
		
		void Awake()
		{
			checkGCExists("GameController");
		}

		private void checkGCExists(string gcName)
		{
			if(GameObject.Find(gcName) == null)
			{
				GameObject cowGameObject = Instantiate(Resources.Load(gcName) as GameObject);
				cowGameObject.name = gcName;
			}
		}
	}
}