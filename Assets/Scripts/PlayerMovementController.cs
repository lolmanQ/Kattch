using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
	public JumpNode currentNode;

	public JumpNode nextNode;

	[SerializeField]
	private float speed = 2f;

	[SerializeField]
	private AnimationCurve speedCurve;

	[SerializeField]
	private float extraHeight = 1f, maxDistance = 1;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private bool isMoving = false;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0) && !isMoving)
		{
			Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D raycastHit = Physics2D.CircleCast(mPos, 0.2f, Vector3.up);
			if(raycastHit.collider.gameObject.TryGetComponent(out JumpNode jumpNode))
			{
				if(Vector3.Distance(currentNode.pos, jumpNode.pos) < maxDistance)
				{
					nextNode = jumpNode;
					StartCoroutine(MoveCoroutine(currentNode, nextNode));
				}
			}
		}
	}

	public IEnumerator MoveCoroutine(JumpNode from, JumpNode to)
	{
		isMoving = true;

		float dist = Vector3.Distance(from.pos, to.pos);
		float progress = 0;

		Vector3 p1 = from.pos;
		Vector3 p2 = from.pos + Vector3.up * extraHeight;
		Vector3 p3 = to.pos + Vector3.up * extraHeight;
		Vector3 p4 = to.pos;

		spriteRenderer.flipX = p1.x - p4.x > 0;

		animator.SetTrigger("StartJump");

		bool landAnimTriggerd = false;

		while (progress <= dist)
		{
			if(dist-progress < 1f && !landAnimTriggerd)
			{
				animator.SetTrigger("StopJump");
				landAnimTriggerd = true;
			}

			float realT = progress / dist;

			float t = speedCurve.Evaluate(realT);
			//transform.position = Vector3.Lerp(from.pos, to.pos, t);

			Vector3 q1 = Vector3.Lerp(p1, p2, t);
			Vector3 q2 = Vector3.Lerp(p2, p3, t);
			Vector3 q3 = Vector3.Lerp(p3, p4, t);
			
			Vector3 z1 = Vector3.Lerp(q1, q2, t);
			Vector3 z2 = Vector3.Lerp(q2, q3, t);

			Vector3 point = Vector3.Lerp(z1, z2, t);
			transform.position = point;

			progress += Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}
		transform.position = to.pos;
		currentNode = to;
		isMoving = false;
	}

	private void OnDrawGizmos()
	{
		
	}
}
