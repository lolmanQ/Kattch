using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class JumpNode : MonoBehaviour
{
	public Vector3 pos => transform.position;

	[SerializeField]
	private Light2D platformLight;

	[SerializeField]
	private Light2D lampLight;

	[SerializeField]
	private Color defaultLampColor, defaultPlatformColor;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void FixedUpdate()
	{
		platformLight.color = Color.Lerp(defaultPlatformColor, platformLight.color, 0.98f);
		lampLight.color = Color.Lerp(defaultLampColor, lampLight.color, 0.98f);
	}

	public void SetPlatColor(Color color)
	{
		platformLight.color = color;
	}
	public void SetLampColor(Color color)
	{
		lampLight.color = color;
	}


	private void OnDrawGizmos()
	{
		Gizmos.DrawCube(pos, Vector3.one * 0.2f);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, 4);
		Gizmos.DrawWireSphere(transform.position, 5.5f);
	}
}
