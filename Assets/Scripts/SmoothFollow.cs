using UnityEngine;

public class SmoothFollow : MonoBehaviour 
{
	public Transform target;
	public float smoothSpeed = 0.2f;
	public Vector3 offset;
 
    void FixedUpdate()
	{
		Vector3 desiredPos = target.position + offset;

		Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
		transform.position = smoothedPos;
	}
}