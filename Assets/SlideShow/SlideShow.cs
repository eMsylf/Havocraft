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
    private GameControls controls;
    private GameControls Controls
    {
        get
        {
            if (controls == null)
            {
                controls = new GameControls();
            }
            return controls;
        }
    }

    public float SlideFadeIn = 1f;
    public float SlideDuration = 1f;
    public float SlideFadeOut = 1f;
    private List<RawImage> Slides;
    private int currentSlide = 0;
    public UnityEvent OnComplete;

    private void Awake()
    {
        Slides = GetComponentsInChildren<RawImage>().ToList();
        Controls.ToContinue.Enable();
        Controls.ToContinue.SkipOne.performed += _ => Skip();
        Controls.ToContinue.SkipAll.performed += _ => SkipAll();
        Debug.Log("Added");
    }

    private void OnDisable()
    {
        Controls.ToContinue.SkipOne.performed -= _ => Skip();
        Controls.ToContinue.SkipAll.performed -= _ => SkipAll();
        Controls.ToContinue.Disable();
    }

    private void OnDestroy()
    {
        Controls.ToContinue.SkipOne.performed -= _ => Skip();
        Controls.ToContinue.SkipAll.performed -= _ => SkipAll();
        Controls.ToContinue.Disable();
    }

    private void Start()
    {
        foreach (RawImage slide in Slides)
        {
            slide.gameObject.SetActive(false);
        }
        NextSlide(true);
    }

    public void Skip()
    {
        Debug.Log("Skip slide");
        NextSlide(false);
    }

    public void SkipAll()
    {
        Debug.Log("Skip all slides");
        Complete();
        OnComplete.Invoke();
    }

    public void Complete()
    {
        foreach (RawImage slide in Slides)
        {
            slide.DOKill();
            slide.gameObject.SetActive(false);
        }
    }

    private void NextSlide(bool first)
    {
        if (currentSlide >= Slides.Count - 1)
        {
            Complete();
            OnComplete.Invoke();
            return;
        }
        if (!first)
        {
            // Close off the previous slide
            Slides[currentSlide].DOKill();
            Slides[currentSlide].gameObject.SetActive(false);
            currentSlide++;
        }
        // Save the original color
        Color originalColor = Slides[currentSlide].color;
        // Set opacity to 0
        Slides[currentSlide].color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Slides[currentSlide].gameObject.SetActive(true);
        Slides[currentSlide].DOFade(1f, SlideFadeIn).
            OnComplete(() => Slides[currentSlide].DOFade(1f, SlideDuration).
            OnComplete(() => Slides[currentSlide].DOFade(0f, SlideFadeOut).
            OnComplete(() => NextSlide(false))));
    }


}
