using Define;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Zep : BaseScene
{
    [SerializeField]
    GameObject Door;
    [SerializeField]
    Sprite OpenDoorSprite;
    [SerializeField]
    Sprite CloseDoorSprite;
    [SerializeField]
    GameObject GameRecord;
    protected override void Init()
    {
        base.Init();
        Type = SceneType.Zep;
    }
    public void OpenDoor()
    {
        var renderer = Door.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = OpenDoorSprite;
    }
    public void CloseDoor()
    {
        var renderer = Door.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = CloseDoorSprite;
    }
    public void LoadHouse()
    {
        Manager.Scene.LoadScene("House");
    }
    public void OpenGameRecord()
    {
        var topTierText = GameRecord.transform.GetChild<TextMeshProUGUI>(true, "TopTier");
        var topScoreText = GameRecord.transform.GetChild<TextMeshProUGUI>(true, "TopScore");
        var recentTierText = GameRecord.transform.GetChild<TextMeshProUGUI>(true, "RecentTier");
        var recentScoreText = GameRecord.transform.GetChild<TextMeshProUGUI>(true, "RecentScore");

        topTierText.text = PlayerPrefs.GetString("TopTier");
        topScoreText.text = PlayerPrefs.GetFloat("TopScore").ToString();
        recentTierText.text = PlayerPrefs.GetString("RecentTier");
        recentScoreText.text = PlayerPrefs.GetFloat("RecentScore").ToString();

        GameRecord.SetActive(true);
    }
    public void CloseGameRecord()
    {
        GameRecord.SetActive(false);
    }
}
