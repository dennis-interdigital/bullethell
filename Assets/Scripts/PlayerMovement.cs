using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float smoothTime = 0.1f;
    public Vector2 followOffset = new Vector2(0.5f, 0.5f); // <— Added offset

    [Header("Tilt Settings")]
    public Transform modelTransform;
    public float maxTiltZ = 30f;
    public float tiltSpeed = 5f;

    [Header("Bounds Settings")]
    public float padding = 0.5f;

    [Header("Visual")]
    [SerializeField] private ParticleSystem activeFlameFX;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private bool isMoving = false;
    private float currentTilt = 0f;
    private Camera mainCam;

    private StageManager stageManager;

    public void Init(StageManager stageManager)
    {
        this.stageManager = stageManager;
        mainCam = Camera.main;
        targetPosition = transform.position;
    }

    public void DoUpdate(float dt)
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;

            // Apply follow offset based on relative direction
            Vector3 offset = new Vector3(
                followOffset.x * Mathf.Sign(mousePos.x - transform.position.x),
                followOffset.y * Mathf.Sign(mousePos.y - transform.position.y),
                0f
            );

            targetPosition = ClampToScreen(mousePos - offset);

            activeFlameFX.Play();
            isMoving = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            activeFlameFX.Stop();
            isMoving = false;
            velocity = Vector3.zero;
        }

        if (isMoving)
        {
            Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, moveSpeed);
            transform.position = ClampToScreen(newPos);
        }

        //Handle Plane tilt
        float tiltTarget = 0f;
        if (isMoving)
        {
            float horizontalInput = (targetPosition.x - transform.position.x);
            tiltTarget = Mathf.Clamp(horizontalInput, -1f, 1f) * -maxTiltZ;
        }

        currentTilt = Mathf.Lerp(currentTilt, tiltTarget, dt * tiltSpeed);
        modelTransform.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }

    private Vector3 ClampToScreen(Vector3 worldPos)
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        float minX = mainCam.transform.position.x - camWidth + padding;
        float maxX = mainCam.transform.position.x + camWidth - padding;
        float minY = mainCam.transform.position.y - camHeight + padding;
        float maxY = mainCam.transform.position.y + camHeight - padding;

        worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
        worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);

        return worldPos;
    }
}
