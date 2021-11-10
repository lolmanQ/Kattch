using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawCube(transform.position, Vector3.one * 0.4f);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
	}
}
