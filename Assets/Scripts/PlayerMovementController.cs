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
	private float extraHeight = 1f, destBend = 1f;

	[SerializeField]
	private Vector2 maxDistance;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float inputBufferLength = 0.2f;

	private float inputBufferTimer = 0;
	private Vector3 bufferdMousePos;

	private bool isMoving = false;

	[SerializeField]
	private bool debugMovement = false;

	[SerializeField]
	private TargetManager targetManager;

	[SerializeField]
	private Gradient gradient;

	private AudioSource jumpAudioSource;

	// Start is called before the first frame update
	void Start()
	{
		jumpAudioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		inputBufferTimer -= Time.unscaledDeltaTime;
		if (Input.GetMouseButtonDown(0))
		{
			inputBufferTimer = inputBufferLength;
			bufferdMousePos = Input.mousePosition;
		}

		if (inputBufferTimer > 0 && !isMoving)
		{
			Vector2 mPos = Camera.main.ScreenToWorldPoint(bufferdMousePos);
			RaycastHit2D raycastHit = Physics2D.CircleCast(mPos, 0.01f, Vector3.up, 0.01f);
			if(raycastHit.collider != null)
			{
				if (raycastHit.collider.gameObject.TryGetComponent(out JumpNode jumpNode))
				{
					if(jumpNode != currentNode)
					{
						float isInsideNum = Mathf.Pow(jumpNode.pos.x - currentNode.pos.x, 2) / Mathf.Pow(maxDistance.x, 2) + Mathf.Pow(jumpNode.pos.y - currentNode.pos.y, 2) / Mathf.Pow(maxDistance.y, 2);

						//Debug.Log(isInsideNum);

						if (isInsideNum <= 1)
						{
							nextNode = jumpNode;
							StartCoroutine(MoveCoroutine(currentNode, nextNode));
						}

						//Vector3 diff = currentNode.pos - jumpNode.pos;
						//if (Mathf.Abs(diff.x) <= maxDistance.x && Mathf.Abs(diff.y) <= maxDistance.y)
						//{
						//	nextNode = jumpNode;
						//	StartCoroutine(MoveCoroutine(currentNode, nextNode));
						//}
					}
				}
			}
			inputBufferTimer = 0;
		}

		if (inputBufferTimer < 0 && !isMoving)
		{
			Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			spriteRenderer.flipX = transform.position.x - mPos.x > 0;
		}
	}

	public IEnumerator MoveCoroutine(JumpNode from, JumpNode to)
	{
		isMoving = true;
		//Time.timeScale = 0.2f;

		jumpAudioSource.Play();

		float dist = Vector3.Distance(from.pos, to.pos);
		float progress = 0;

		Vector3 p1 = from.pos;
		Vector3 p2 = from.pos + Vector3.up * extraHeight + (to.pos - from.pos).normalized * destBend;
		Vector3 p3 = to.pos + Vector3.up * extraHeight + (from.pos - to.pos).normalized * destBend;
		Vector3 p4 = to.pos;

		float averageLenth = ((p4-p1).magnitude + (p1 - p2).magnitude + (p3 - p2).magnitude + (p4 - p3).magnitude)/2;

		spriteRenderer.flipX = p1.x - p4.x > 0;

		animator.SetTrigger("StartJump");

		if (debugMovement)
		{
			Debug.Log("Jump distance: " + dist);
			Debug.Log("Average length: " + averageLenth);
		}

		bool landAnimTriggerd = false;

		while (progress <= averageLenth)
		{
			if(dist-progress < 1f && !landAnimTriggerd)
			{
				animator.SetTrigger("StopJump");
				landAnimTriggerd = true;
			}

			float realT = progress / averageLenth;

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
		Color newColor = gradient.Evaluate(Random.Range(0f, 1f));
		to.SetPlatColor(newColor);
		to.SetLampColor(newColor);
		isMoving = false;
		//Time.timeScale = 1f;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("PlayerTarget"))
		{
			targetManager.Hit();
		}
	}

	private void OnDrawGizmos()
	{
		Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Gizmos.DrawWireSphere(mPos, 1f);

		//RaycastHit2D raycastHit = Physics2D.CircleCast(mPos, 0.01f, Vector3.up);
		
	}
}
