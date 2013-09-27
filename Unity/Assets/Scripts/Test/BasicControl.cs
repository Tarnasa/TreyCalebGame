using UnityEngine;
using System.Collections;

public class BasicControl : MonoBehaviour {
	
	public const float moveSpeed = 0.1F;
	public const float decel = 0.05F;
	public const float accel = 0.07F;
	public const float gravity = 9.8F;
	
	public Vector3 velocity = Vector3.zero;
	
	private Vector3 inputVector;
	private float rotate;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		// Get WASD
		inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis ("Vertical"));
		// Get mouse rotation
		rotate = Input.GetAxis("Mouse X") * 5.0F;
		// Rotate
		transform.Rotate (0, rotate, 0);
		
		inputVector = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * inputVector;
		// Apply acceleration
		velocity += inputVector * accel;
		velocity *= 1 - decel;
		// Cap speed
		if (velocity.magnitude > moveSpeed)
		{
			velocity = velocity.normalized * moveSpeed;
		}
		// Move
		controller.Move(velocity);
	}
}
