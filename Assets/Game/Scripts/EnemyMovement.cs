using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum AfterEnterBehavior
    {
        Stop,
        ContinuePattern
    }

    public enum EntryDirection
    {
        FromTop,
        FromLeft,
        FromRight
    }

    [Header("Entry")]
    public EntryDirection entryDirection = EntryDirection.FromTop;
    public float enterOffsetFromEdge = 2.5f;
    public float enterSpeed = 5f;
    public float enterThreshold = 0.05f;

    [Header("After Enter")]
    public AfterEnterBehavior afterEnter = AfterEnterBehavior.Stop;

    [Header("Pattern Phase")]
    public float verticalSpeed = 1.5f;

    [Header("Curve Movement")]
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;

    [Header("Curve Settings")]
    public float curveDuration = 4f;
    public float curveAmplitudeX = 3f;
    public float curveAmplitudeY = 0f;
    public bool loopCurve = true;

    Camera mainCam;

    Vector3 enterTargetWorld;
    Vector3 patternStartPos;

    float timer;
    bool hasEntered;

    void Start()
    {
        mainCam = Camera.main;
        ComputeEnterTarget();
    }

    void Update()
    {
        if (!hasEntered)
            HandleEntry();
        else
            HandlePattern();
    }

    // ─────────────────────────────
    void ComputeEnterTarget()
    {
        float camTop = mainCam.transform.position.y + mainCam.orthographicSize;
        float camBottom = mainCam.transform.position.y - mainCam.orthographicSize;
        float camRight = mainCam.transform.position.x + mainCam.orthographicSize * mainCam.aspect;
        float camLeft = mainCam.transform.position.x - mainCam.orthographicSize * mainCam.aspect;

        Vector3 pos = transform.position;

        switch (entryDirection)
        {
            case EntryDirection.FromTop:
                enterTargetWorld = new Vector3(pos.x, camTop - enterOffsetFromEdge, pos.z);
                break;

            case EntryDirection.FromLeft:
                enterTargetWorld = new Vector3(camLeft + enterOffsetFromEdge, pos.y, pos.z);
                break;

            case EntryDirection.FromRight:
                enterTargetWorld = new Vector3(camRight - enterOffsetFromEdge, pos.y, pos.z);
                break;
        }
    }

    // ─────────────────────────────
    void HandleEntry()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            enterTargetWorld,
            enterSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, enterTargetWorld) <= enterThreshold)
        {
            hasEntered = true;
            patternStartPos = transform.position;
            timer = 0f;
        }
    }

    // ─────────────────────────────
    void HandlePattern()
    {
        if (afterEnter == AfterEnterBehavior.Stop)
            return;

        timer += Time.deltaTime;

        float t = timer / curveDuration;
        if (loopCurve) t %= 1f;
        else t = Mathf.Clamp01(t);

        float xOffset = xCurve.Evaluate(t) * curveAmplitudeX;
        float yOffset = yCurve.Evaluate(t) * curveAmplitudeY;

        Vector3 pos = patternStartPos;

        pos += Vector3.down * verticalSpeed * timer;
        pos += new Vector3(xOffset, yOffset, 0f);

        transform.position = pos;
    }

    public bool HasEntered() => hasEntered;
}
