using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

public class Timer : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float totalTime;
    [SerializeField] private Image headCapImage;
    [SerializeField] private Image tailCapImage;
    [SerializeField] private TMP_Text timeText;
    
    public float CurrentTime { get; private set; }
    private bool _isPaused;
    
    public delegate void TimerDelegate();
    public TimerDelegate OnTimeout;

    private bool ischange = false;
    
    private void Awake()
    {
        _isPaused = true;
    }

    private void Update()
    {
        if (!_isPaused)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= totalTime)
            {
                headCapImage.gameObject.SetActive(false);
                tailCapImage.gameObject.SetActive(false);
                _isPaused = true;
                
                OnTimeout?.Invoke();
            }
            else
            {
                fillImage.fillAmount = (totalTime - CurrentTime) / totalTime;
                headCapImage.transform.localRotation = 
                    Quaternion.Euler(new Vector3(0, 0, fillImage.fillAmount * 360 * -1));
                
                var timeTextTime = totalTime - CurrentTime;
                timeText.text = timeTextTime.ToString("F0");
            }

            if (CurrentTime >= 20f && !ischange)
            {
                ischange = true;
                ChaingeColor();
            }
        }
    }

    public void StartTimer()
    {
        _isPaused = false;
        headCapImage.gameObject.SetActive(true);
        tailCapImage.gameObject.SetActive(true);
    }

    public void PauseTimer()
    {
        _isPaused = true;
    }

    public void InitTimer()
    {
        CurrentTime = 0;
        fillImage.fillAmount = 1;
        timeText.text = totalTime.ToString("F0");
        headCapImage.gameObject.SetActive(false);
        tailCapImage.gameObject.SetActive(false);
        _isPaused = true;
        
        // DoTween이 실행 중이라면 기존 Tween 제거
        timeText.DOKill();
        fillImage.DOKill();
        headCapImage.DOKill();
        tailCapImage.DOKill();

        // 색상 즉시 변경
        timeText.color = Color.white;
        fillImage.color = Color.white;
        headCapImage.color = Color.white;
        tailCapImage.color = Color.white;
        
        ischange = false;
    }

    private void ChaingeColor()
    {
        timeText.DOColor(Color.red, 10f);    
        fillImage.DOColor(Color.red, 10f);
        headCapImage.DOColor(Color.red, 10f);
        tailCapImage.DOColor(Color.red, 10f);
    }
}