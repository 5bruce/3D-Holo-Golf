using UnityEngine;

/// <summary>
/// Implements a non-collidable point that sits slightly above the user's gaze vector
/// </summary>
public class ReferencePoint : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start()
	{
		// Grab the mesh renderer that's on the same object as this script.
		meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
		meshRenderer.enabled = true;
	}

	// Update is called once per frame
	void Update()
	{
		// Do a raycast into the world based on the user's
		// head position and orientation.
		Vector3 headPosition = Camera.main.transform.position;
		Vector3 gazeDirection = Camera.main.transform.forward;

		// percent offset
		const float forward_offset = 1f;  
		Vector3 offset = new Vector3 (0f, 0.10f, 0f);
		meshRenderer.transform.position = headPosition + gazeDirection*forward_offset + offset;
	}
}
