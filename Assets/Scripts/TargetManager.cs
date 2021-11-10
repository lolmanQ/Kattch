using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
	[SerializeField]
	private GameObject playerTarget, player;

	[SerializeField]
	private float speed = 2f, speedIncrease = 0.3f, speedCap = 10f, maxMoveDistance = 10f;

	[SerializeField]
	private Vector2 stopDuration = Vector2.up;

	[SerializeField]
	private AnimationCurve speedCurve;

	[SerializeField]
	private List<GameObject> targetMoveToPoints = new List<GameObject>();

	private Vector3 lastPoint = Vector3.zero, nextPoint;
	private float stopTimer = 0f, score = 0;

	[SerializeField]
	private float timeBeforeDeath = 10, timeToAdd = 4, startTime = 10;

	private float deathTimer;

	private bool isMoving = false;

	[SerializeField]
	private TMPro.TextMeshProUGUI scoreText, deathTimerText;

	[SerializeField]
	private AudioSource hitAudioSource;

	private void Awake()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			targetMoveToPoints.Add(transform.GetChild(i).gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		deathTimer = startTime;
		lastPoint = SetNewPoint(true);
		playerTarget.transform.position = lastPoint;
		nextPoint = SetNewPoint();
		score = 0;	
	}

	// Update is called once per frame
	void Update()
	{
		if(lastPoint == nextPoint)
		{
			//Debug.Log("points are same");
			if(!isMoving && stopTimer <= 0)
			{
				nextPoint = SetNewPoint();
			}
		}

		if (!isMoving && stopTimer <= 0 && lastPoint != nextPoint)
		{
			StartCoroutine(Move(lastPoint, nextPoint));
		}
		stopTimer -= Time.deltaTime;
		deathTimer -= Time.deltaTime;

		if(deathTimer > 0)
		{
			deathTimerText.SetText("Time left: " + Mathf.Round(deathTimer * 10) / 10);
		}
		else if(deathTimer <= 0)
		{
			deathTimerText.SetText("Out of time");
			Time.timeScale = 0;
		}
	}

	Vector3 SetNewPoint(bool ignoreMaxDist = false)
	{
		int failedIterations = 0;
		GameObject newPoint;
		do
		{
			failedIterations++;
			int newPointIndex = Random.Range(0, targetMoveToPoints.Count);
			newPoint = targetMoveToPoints[newPointIndex];

			if(failedIterations > 5000)
			{
				Debug.LogWarning("Failed to find new move to point");
				break;
			}
			if((ignoreMaxDist || Vector3.Distance(newPoint.transform.position, lastPoint) <= maxMoveDistance) && newPoint.transform.position != lastPoint)
			{
				break;
			}

		} while (true);

		return newPoint.transform.position;
	}

	private IEnumerator Move(Vector3 start, Vector3 end)
	{
		isMoving = true;

		float progress = 0;
		float totalDistance = Vector3.Distance(start, end);

		while (progress <= totalDistance)
		{
			//Debug.Log("p: " + progress);
			//Debug.Log("d: " + totalDistance);
			float t = progress / totalDistance;
			float posT = speedCurve.Evaluate(t);

			if (float.IsNaN(posT))
			{
				Debug.Log(posT);
			}

			playerTarget.transform.position = Vector3.Lerp(start, end, posT);

			progress += Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}
		playerTarget.transform.position = end;
		lastPoint = end;
		isMoving = false;
		nextPoint = SetNewPoint();
		stopTimer = Random.Range(stopDuration.x, stopDuration.y);
	}

	public void Hit()
	{
		//Debug.Log("Hit");
		StopAllCoroutines();
		isMoving = false;
		stopTimer = Random.Range(stopDuration.x, stopDuration.y);
		Vector3 newPoint = SetNewPoint(true);
		playerTarget.transform.position = newPoint;
		lastPoint = newPoint;
		nextPoint = SetNewPoint();
		hitAudioSource.Play();
		speed += speedIncrease;
		speed = Mathf.Clamp(speed, 0, speedCap);
		deathTimer += timeToAdd;
		float clampedDeathTimer = Mathf.Clamp(deathTimer, 0, timeBeforeDeath);
		if(clampedDeathTimer != deathTimer)
		{
			score += Mathf.Round((deathTimer - clampedDeathTimer) * 10) / 10;
		}
		else
		{
			score++;
		}
		deathTimer = clampedDeathTimer;

		scoreText.SetText("Score: " + score);
	}

	private void OnDrawGizmosSelected()
	{

	}
}
