using DG.Tweening;
using UnityEngine;

namespace bullethell
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseUI : MonoBehaviour
    {
        protected StageManager stageManager;

        CanvasGroup cg;
        Tween currentTween;

        public virtual void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            cg = GetComponent<CanvasGroup>();
        }

        public virtual void DoUpdate(float dt) { }

        public virtual void Show()
        {
            gameObject.SetActive(true);

            currentTween?.Kill();

            Transform tf = transform;
            Vector3 scaleFrom = new Vector3(0.9f, 0.9f, 0.9f);

            cg.alpha = 0f;
            tf.localScale = scaleFrom;

            currentTween = DOTween.Sequence()
                .Append(cg.DOFade(1f, 0.3f))
                .Join(tf.DOScale(Vector3.one, 0.2f));
        }

        public virtual void Hide()
        {
            currentTween?.Kill();

            Transform tf = transform;
            Vector3 scaleTo = new Vector3(1.2f, 1.2f, 1.2f);

            currentTween = DOTween.Sequence()
                .Append(cg.DOFade(0f, 0.3f))
                .Join(tf.DOScale(scaleTo, 0.2f))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
    }
}
