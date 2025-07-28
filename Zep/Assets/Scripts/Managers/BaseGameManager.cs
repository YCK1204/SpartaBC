using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameManager
{
    public bool IsGameRunning { get; set; }
    public Action OnEndGame { get; set; }
    public Action OnStartGame { get; set; }
    public Action OnPauseGame { get; set; }
    public Action OnResumeGame { get; set; }
    public void ResumeGame();
    public void StartGame();
    public void PauseGame();
    public void EndGame();
}
public class BaseGameManager : MonoBehaviour, IGameManager
{
    public bool IsGameRunning { get; set; } = false;
    float _score = 0;
    public float Score { get { return _score; } set { _score = value; OnAddScore?.Invoke(); } }
    public Action OnAddScore { get; set; }
    public Action OnEndGame { get; set; }
    public Action OnStartGame { get; set; }
    public Action OnPauseGame { get; set; }
    public Action OnResumeGame { get; set; }

    float _timeScale;
    private void Start()
    {
        Init();
        DateTime now = DateTime.Now;

        int year = now.Year;
        int month = now.Month;
        int day = now.Day;
        int hour = now.Hour;
        int minute = now.Minute;
        int second = now.Second;

        Debug.Log($"현재 시간: {year}년 {month}월 {day}일 {hour}시 {minute}분 {second}초");
    }
    protected virtual void Init()
    {
    }
    public virtual void EndGame()
    {
        //Time.timeScale = 0f;
        OnEndGame?.Invoke();
    }
    public virtual void PauseGame()
    {
        _timeScale = Time.timeScale;
        Time.timeScale = 0f;
        IsGameRunning = false;
        OnPauseGame?.Invoke();
    }

    public virtual void StartGame()
    {
        Time.timeScale = 1f;
        IsGameRunning = true;
        OnStartGame?.Invoke();
    }
    public void ResumeGame()
    {
        Time.timeScale = _timeScale;
        IsGameRunning = true;
        OnResumeGame?.Invoke();
    }
}
