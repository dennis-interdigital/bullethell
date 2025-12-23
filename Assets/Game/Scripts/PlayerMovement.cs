using UnityEngine;

namespace bullethell
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float smoothTime = 0.1f;
        public Vector2 followOffset = new Vector2(0.5f, 0.5f);

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
        private bool isMoving;
        private float currentTilt;
        private Camera mainCam;

        private StageManager stageManager;

        private bool isInit;
        private bool movementEnabled = true; // 🔑 NEW

        // ─────────────────────────────
        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            mainCam = Camera.main;
            targetPosition = transform.position;
            isInit = true;
        }

        // ─────────────────────────────
        public void DoUpdate(float dt)
        {
            if (!isInit || !movementEnabled)
            {
                HandleTilt(); // smoothly reset tilt when stopped
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = transform.position.z;

                Vector3 offset = new Vector3(
                    followOffset.x * Mathf.Sign(mousePos.x - transform.position.x),
                    followOffset.y * Mathf.Sign(mousePos.y - transform.position.y),
                    0f
                );

                targetPosition = ClampToScreen(mousePos - offset);

                if (activeFlameFX && !activeFlameFX.isPlaying)
                    activeFlameFX.Play();

                isMoving = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopLocalMovement();
            }

            if (isMoving)
            {
                Vector3 newPos = Vector3.SmoothDamp(
                    transform.position,
                    targetPosition,
                    ref velocity,
                    smoothTime,
                    moveSpeed
                );

                transform.position = ClampToScreen(newPos);
            }

            HandleTilt();
        }

        // ─────────────────────────────
        private void HandleTilt()
        {
            if (!modelTransform) return;

            float tiltTarget = 0f;

            if (isMoving && movementEnabled)
            {
                float horizontalInput = targetPosition.x - transform.position.x;
                tiltTarget = Mathf.Clamp(horizontalInput, -1f, 1f) * -maxTiltZ;
            }

            currentTilt = Mathf.Lerp(
                currentTilt,
                tiltTarget,
                Time.deltaTime * tiltSpeed
            );

            modelTransform.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
        }

        // ─────────────────────────────
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

        // ─────────────────────────────
        // PUBLIC CONTROL API
        // ─────────────────────────────

        public void StopMovement()
        {
            movementEnabled = false;
            StopLocalMovement();
        }

        public void StartMovement()
        {
            movementEnabled = true;
            targetPosition = transform.position;
            velocity = Vector3.zero;
        }

        private void StopLocalMovement()
        {
            isMoving = false;
            velocity = Vector3.zero;

            if (activeFlameFX && activeFlameFX.isPlaying)
                activeFlameFX.Stop();
        }

        public bool IsMovementEnabled()
        {
            return movementEnabled;
        }
    }
}
