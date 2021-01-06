using UnityEngine;

public class AddVelocity : MonoBehaviour
{
	[SerializeField] Vector3 velocity;
	[SerializeField] bool update;

	Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		rb.AddForce(velocity, ForceMode.Force);
	}

	private void Update()
	{
		if(update)
			rb.AddForce(velocity, ForceMode.Force);
	}
}
