using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class SlideShow : MonoBehaviour
{
    public float SlideDuration = 1f;
    public float SlideFadeSpeed = 1f;
    private List<RawImage> Slides;
    private int currentSlide = 0;
    public UnityEvent OnComplete;

    private void Awake()
    {
        Slides = GetComponentsInChildren<RawImage>().ToList();
    }

    private void Start()
    {
        foreach (RawImage slide in Slides)
        {
            slide.gameObject.SetActive(false);
        }
        NextSlide(true);
    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        Debug.Log("Skip slide");
        NextSlide(false);
    }

    private IEnumerator ShowSlide()
    {
        //Slides[currentSlide].DOFade(1f, SlideFadeSpeed);
        //currentSlide++;
        yield return new WaitForSeconds(SlideDuration);
    }

    private void NextSlide(bool first)
    {
        if (!first)
        {
            // Close off the previous slide
            StopAllCoroutines();
            Slides[currentSlide].DOComplete();
            Slides[currentSlide].gameObject.SetActive(false);
            currentSlide++;
        }
        if (currentSlide >= Slides.Count)
        {
            OnComplete.Invoke();
            return;
        }
        // Save the original color
        Color originalColor = Slides[currentSlide].color;
        // Set opacity to 0
        Slides[currentSlide].color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Slides[currentSlide].gameObject.SetActive(true);
        Slides[currentSlide].DOFade(1f, SlideFadeSpeed).
            OnComplete(() => Slides[currentSlide].DOFade(1f, SlideDuration).
            OnComplete(() => Slides[currentSlide].DOFade(0f, SlideFadeSpeed).
            OnComplete(() => NextSlide(false))));
    }


}
