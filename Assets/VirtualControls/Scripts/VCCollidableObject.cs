//----------------------------------------
// Virtual Controls Suite for Unity
// Â© 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// An object that contains a Collider which may be hit tested against.
/// </summary>
public class VCCollideableObject : MonoBehaviour 
{
	protected Camera _colliderCamera;
	protected Collider _collider;
	
	// cached vector for AABB hit test
	private Vector3 _tempVec;
	
	// Causes this object to use the specified gameObject for colliison detection.
	protected void InitCollider (GameObject colliderGo) 
	{
		_collider = colliderGo.GetComponent<Collider>();
		_colliderCamera = VCUtils.GetCamera(colliderGo);
	}
	
	// Axis Aligned Bounding Box Hit Test
	public bool AABBContains (Vector2 pos)
	{
		// if we have no collider, we never collide
		if (_collider == null)
			return false;
			
		// test min extents
		_tempVec = _colliderCamera.WorldToScreenPoint(_collider.bounds.min);
		if (pos.x < _tempVec.x || pos.y < _tempVec.y)
			return false;
		
		// test max extents
		_tempVec = _colliderCamera.WorldToScreenPoint(_collider.bounds.max);
		if (pos.x > _tempVec.x || pos.y > _tempVec.y)
			return false;
		
		return true;
	}
}
