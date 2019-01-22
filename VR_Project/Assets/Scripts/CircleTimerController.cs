using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTimerController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Progress;
    public bool IsReverse = false;
    public bool IsClockwise = true;
    public bool InProgress { get; private set; }

    public Image ForegroundImage;
    public Image BackgroundImage;

    private float durationTime;
    private float currentTime;

	// Use this for initialization
	void Start ()
    {
        Hide();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!InProgress)
            return;

        currentTime += Time.deltaTime;
        float progress = currentTime / durationTime;
        SetProgress(progress);
        if (progress >= 1f) StopTimer();

        VisualiseProgress();
    }

    public void Hide()
    {
        ForegroundImage.enabled = false;
        BackgroundImage.enabled = false;
    }

    public void Show()
    {
        ForegroundImage.enabled = true;
        BackgroundImage.enabled = true;
    }

    public void StartTimer(float seconds)
    {
        Show();
        durationTime = seconds;
        currentTime = 0f;
        InProgress = true;
    }

    public void StopTimer()
    {
        Hide();
        InProgress = false;
    }

    private void SetProgress(float progress)
    {
        if (progress > 1f) progress = 1f;
        else if (progress < 0f) progress = 0f;
        Progress = progress;
    }

    private void VisualiseProgress()
    {
        ForegroundImage.fillClockwise = IsClockwise;
        ForegroundImage.fillAmount = IsReverse ? 1f - Progress : Progress;
    }

}
