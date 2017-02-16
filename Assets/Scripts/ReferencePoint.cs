using UnityEngine;

/// <summary>
/// Implements a non-collidable point that sits slightly above the user's gaze vector
/// </summary>
public class ReferencePoint : MonoBehaviour
{
	private MeshRenderer meshRenderer;

    public float forward_offset = 1f;
    public float upwards_offset = 0.1f;

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
		Vector3 gazeDirection = Vector3.Normalize(Camera.main.transform.forward);
 
		Vector3 offset = new Vector3 (0f, upwards_offset, 0f);
		meshRenderer.transform.position = headPosition + gazeDirection*forward_offset + offset;
	}
}
