using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdGameManager : BaseGameManager
{
    public SpawnController Spawner { get; set; }
    [Header("Player")]
    [SerializeField]
    BirdController Player;
    [Header("Game UI Button")]
    [SerializeField]
    Button GameStartBtn;
    [Header("Game UI Text")]
    [SerializeField]
    TextMeshProUGUI ScoreText;
    int _passCount = 0;

    [SerializeField]
    RectTransform GameResultPanel;
    [SerializeField]
    TextMeshProUGUI ResultTier;
    [SerializeField]
    TextMeshProUGUI ResultTime;
    [SerializeField]
    TextMeshProUGUI ResultPassCount;

    protected override void Init()
    {
        base.Init();
        Manager.Game = this;

        OnStartGame += StartBirdGame;
        OnAddScore += AddScoreBirdGame;
        OnEndGame += EndBirdGame;
        OnPauseGame += PauseBirdGame;
        OnPauseGame += ResumeBirdGame;
        StartCoroutine(CoToggleStartImgs());
    }
    void StartBirdGame()
    {
        GameStartBtn.gameObject.SetActive(false);
        StopCoroutine(CoToggleStartImgs());
        IsGameRunning = true;
        Spawner.StartSpawnObstacle();
        Player.rb.constraints = RigidbodyConstraints2D.None;
        StartCoroutine(CoAddScore());
    }
    void AddScoreBirdGame()
    {
        ScoreText.text = Score.ToString("0.00");
    }
    void EndBirdGame()
    {
        IsGameRunning = false;
        StopCoroutine(CoAddScore());
        ShowGameResult();
        SaveGameData();
    }
    void SaveGameData()
    {
        var score = PlayerPrefs.GetFloat("TopScore");

        PlayerPrefs.SetString("RecentTier", ResultTier.text);
        PlayerPrefs.SetFloat("RecentScore", float.Parse(Score.ToString("0.00")));
        if (score < Score)
        {
            PlayerPrefs.SetString("TopTier", ResultTier.text);
            PlayerPrefs.SetFloat("TopScore", float.Parse(Score.ToString("0.00")));
        }
    }
    void PauseBirdGame()
    {
        StopCoroutine(CoAddScore());
    }
    void ResumeBirdGame()
    {
        StartCoroutine(CoAddScore());
    }
    IEnumerator CoAddScore()
    {
        while (IsGameRunning)
        {
            Score += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator CoToggleStartImgs()
    {
        var imgs = GameStartBtn.transform.GetChild<RectTransform>(false, "Imgs");
        while (!IsGameRunning)
        {
            imgs.gameObject.SetActiveToggle();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    public void CountPass()
    {
        _passCount++;
    }
    public void BackToZep()
    {
        Manager.Scene.LoadScene("Zep");
    }
    public void Retry()
    {
        Manager.Scene.LoadScene("FlappyBird");
    }
    void ShowGameResult()
    {
        ResultPassCount.text = _passCount.ToString();
        ResultTime.text = ScoreText.text;
        ResultTier.text = Score < 20 ? "C" : Score < 40 ? "B" : Score < 60 ? "A" : "S";
        GameResultPanel.gameObject.SetActive(true);
    }
}
