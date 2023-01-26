using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThoughtBubbleMiniGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(AudioSource))]
     public class ThoughtBubble : MonoBehaviour
    {
        [SerializeField] private int pointsOnTouch = 1;
        [SerializeField] private int pointsOnExplode = -1;
        [SerializeField] private float explodeTime = 5.0f;
        [SerializeField] private float maxScale = 10.0f;
        [SerializeField] private float fadeTime = 0.5f;
        [SerializeField] private AudioClip explodeSound;
        [SerializeField] private AudioClip pierceSound;
        [SerializeField] private Color finalColor = Color.red;

        bool startedExpanding = false;
        bool finishedExpasion = false;
        bool bubblePierced = false;
        private SpriteRenderer thisSpriteRenderer;
        private Collider2D thisCollider;
        private AudioSource thisAudioSource;

        private Coroutine expationCoroutine;

        private void Awake()
        {
            transform.localScale = new Vector3(0.0f, 0.0f, transform.localScale.z);
            thisSpriteRenderer = GetComponent<SpriteRenderer>();
            thisCollider = GetComponent<Collider2D>();
            thisAudioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!startedExpanding || finishedExpasion || bubblePierced)
                return;

            if (Input.touchCount > 0)
            {

                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Vector2 touchPos = new Vector2(wp.x, wp.y);
                Collider2D touchCollider = Physics2D.OverlapPoint(touchPos);

                if (thisCollider == touchCollider)
                {
                    Pierce();
                }
            }
        }

        public void Expand()
        {
            expationCoroutine = StartCoroutine(PerformExpansion());
        }

        public void Pierce()
        {
            if (finishedExpasion)
                return;

            StartCoroutine(PerformPierce());
        }

        public void StopExpandingAndFade()
        {
            StartCoroutine(StopExpandingAndFadeCO());
        }

        private IEnumerator StopExpandingAndFadeCO()
        {
            finishedExpasion = true;
            if (expationCoroutine != null)
                StopCoroutine(expationCoroutine);

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(Fade());
        }

        private IEnumerator PerformPierce()
        {
            bubblePierced = true;

            if (pierceSound)
            {
                thisAudioSource.clip = pierceSound;
                thisAudioSource.Play();
            }

            ScoreManager.Instance.IncreaseScore(pointsOnTouch);
            yield return PerformShrink();
        }

        private IEnumerator PerformExpansion()
        {
            yield return new WaitForSeconds(1.0f);

            startedExpanding = true;
            float scaleIncreasePerSecond = maxScale / explodeTime;
            float currentScale = 0.0f;
            Coroutine shakeCoroutine;
            Coroutine changeColorCoroutine;

            while (currentScale < maxScale)
            {
                yield return null;

                if (bubblePierced)
                    yield break;

                currentScale += scaleIncreasePerSecond * Time.deltaTime;
                if (currentScale > maxScale)
                    currentScale = maxScale;

                transform.localScale = new Vector3(currentScale, currentScale, transform.localScale.z);
            }

            finishedExpasion = true;

            shakeCoroutine =  StartCoroutine(ShakeAnimation(fadeTime));
            changeColorCoroutine =  StartCoroutine(ColorChangeAnimation(fadeTime));
            yield return shakeCoroutine;
            yield return changeColorCoroutine;

            ScoreManager.Instance.IncreaseScore(pointsOnExplode);

            if (explodeSound)
            {
                thisAudioSource.clip = explodeSound;
                thisAudioSource.Play();
            }

            yield return Fade();

        }

        private IEnumerator PerformShrink()
        {
            float currentScale = transform.localScale.x;
            float scaleDecreasePerSecond = maxScale / fadeTime;

            while (currentScale > 0.0f)
            {
                yield return null;

                currentScale -= scaleDecreasePerSecond * Time.deltaTime;
                if (currentScale < 0.0f)
                    currentScale = 0.0f;

                transform.localScale = new Vector3(currentScale, currentScale, transform.localScale.z);
            }

            gameObject.SetActive(false);
            Destroy(this.gameObject);

        }

        private IEnumerator Fade()
        {
            float currentAlpha = 1.0f;
            float alphaDecreasePerSecond = currentAlpha / fadeTime;
            Color color = thisSpriteRenderer.color;

            while (currentAlpha > 0.0f)
            {
                currentAlpha -= alphaDecreasePerSecond * Time.deltaTime;
                if (currentAlpha < 0.0f)
                    currentAlpha = 0.0f;


                thisSpriteRenderer.color = new Color(color.r, color.g, color.b, currentAlpha);
                yield return null;
            }

            gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

        private IEnumerator ShakeAnimation(float duration)
        {
            float xRange = 0.02f;
            float animTime = 0.1f;
            float baseXPosition = transform.position.x;
            float currentXPosition = baseXPosition;
            float positionChangePerSecond = (4*xRange) / animTime;
            float currentDuration = 0.0f;
            bool inIncreaseStage = true;

            while(currentDuration < duration)
            {
                yield return new WaitForEndOfFrame();

                currentDuration += Time.deltaTime;
                float positionChange = positionChangePerSecond * Time.deltaTime;

                currentXPosition = inIncreaseStage ?
                    currentXPosition + positionChange :
                    currentXPosition - positionChange;

                if (inIncreaseStage && currentXPosition > baseXPosition + xRange)
                {
                    currentXPosition = baseXPosition + xRange;
                    inIncreaseStage = false;
                }
                else if (!inIncreaseStage && currentXPosition < baseXPosition - xRange)
                {
                    currentXPosition = baseXPosition - xRange;
                    inIncreaseStage = true;
                }

                transform.position = new Vector3(currentXPosition, transform.position.y, transform.position.z);
            }
            
        }

        private IEnumerator ColorChangeAnimation(float duration)
        {
            Color currentColor = thisSpriteRenderer.color;
            float redChangePerSecond = (finalColor.r - currentColor.r) / duration; 
            float greenChangePerSecond = (finalColor.g - currentColor.g) / duration; 
            float blueChangePerSecond = (finalColor.b - currentColor.b) / duration; 
            float alphaChangePerSecond = (finalColor.a - currentColor.a) / duration;
            float currentTime = 0.0f;

            while(currentTime < duration)
            {
                yield return new WaitForEndOfFrame();

                currentTime += Time.deltaTime;
                float newRed = currentColor.r + redChangePerSecond * Time.deltaTime;
                float newGreen = currentColor.g + greenChangePerSecond * Time.deltaTime;
                float newBlue = currentColor.b + blueChangePerSecond * Time.deltaTime;
                float newAlpha = currentColor.a + alphaChangePerSecond * Time.deltaTime;

                currentColor = new Color(newRed, newGreen, newBlue, newAlpha);
                thisSpriteRenderer.color = currentColor;
            }

        }

        public void SetMaxScale(float maxScale) => this.maxScale = maxScale;
        public float GetMaxScale() => maxScale;
        public void SetExplodeTime(float explodeTime) => this.explodeTime = explodeTime;
        public void SetPointsOnTouch(int pointsOnTouch) => this.pointsOnTouch = pointsOnTouch;
        public void SetPointsOnExplode(int pointsOnExplode) => this.pointsOnExplode = pointsOnExplode;
    }
}
     

