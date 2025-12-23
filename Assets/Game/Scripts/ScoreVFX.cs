using UnityEngine;

namespace bullethell
{
    public class ScoreVFX : MonoBehaviour
    {
        [Header("Flight")]
        [SerializeField] private float flyDuration = 0.6f;
        [SerializeField] private float curveHeight = 2f;
        [SerializeField]
        private AnimationCurve moveCurve =
            AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Transform target;
        private Vector3 startPos;
        private float timer;
        private bool isFlying;

        private StageManager stageManager;
        private int scoreValue;

        // ─────────────────────────────
        public void Init(
            StageManager stageManager,
            Vector3 startWorldPos,
            Transform target,
            int scoreValue)
        {
            this.stageManager = stageManager;
            this.target = target;
            this.scoreValue = scoreValue;

            startPos = startWorldPos;
            transform.position = startWorldPos;

            timer = 0f;
            isFlying = true;
        }

        void Update()
        {
            if (!isFlying || target == null)
                return;

            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / flyDuration);
            float easedT = moveCurve.Evaluate(t);

            Vector3 endPos = target.position;

            // Parabolic curve (Touhou-style pickup)
            Vector3 mid = (startPos + endPos) * 0.5f;
            mid.y += curveHeight;

            Vector3 a = Vector3.Lerp(startPos, mid, easedT);
            Vector3 b = Vector3.Lerp(mid, endPos, easedT);
            transform.position = Vector3.Lerp(a, b, easedT);

            if (t >= 1f)
            {
                Complete();
            }
        }

        void Complete()
        {
            isFlying = false;

            // Add score
            stageManager.gameController.AddBossTrigger(scoreValue);

            // Return to VFX pool
            gameObject.SetActive(false);
        }
    }
}
