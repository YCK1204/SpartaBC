using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class BirdTextures
{
    [Header("Ground Settings")]
    public Sprite[] Grounds;

    [Header("Rock Settings")]
    public Sprite[] Rocks;
}
// 1    1   2       2   1   2   2   3   3       3   1
//   1  1       2   1   2   2   3   2       3   1   3
public class SpawnController : BaseSpawnController
{
    [Header("Bird Texture Configuration")]
    [SerializeField]
    BirdTextures textureSettings;

    [Header("Scrolling Object Settings")]
    [SerializeField]
    ScrollingObject[] GroundDown;
    [SerializeField]
    ScrollingObject[] GroundUp;
    [SerializeField]
    ScrollingObject[] BG;

    Dictionary<string, ScrollingObject[]> ScrollingObjects = new Dictionary<string, ScrollingObject[]>();
    List<int[]> _obstacleSizeList = new List<int[]>
    {
        new int[] { 1, 0 },
        new int[] { 0, 1 },
        new int[] { 1, 1 },
        new int[] { 2, 0 },
        new int[] { 0, 2 },

        new int[] { 2, 1 },
        new int[] { 1, 2 },
        new int[] { 2, 2 },
        new int[] { 3, 0 },
        new int[] { 0, 3 },

        new int[] { 3, 1 },
        new int[] { 1, 3 },
        new int[] { 2, 3 },
        new int[] { 3, 2 },
    };
    [Header("Obstacle Settings")]
    [SerializeField]
    GameObject RockUp;
    [SerializeField]
    GameObject RockDown;
    [SerializeField]
    Transform UpParent;
    [SerializeField]
    Transform DownParent;
    private void Start()
    {
        BirdGameManager birdGameManager = Manager.Game as BirdGameManager;
        birdGameManager.Spawner = this;

        ScrollingObjects.Add("GroundUp", GroundUp);
        ScrollingObjects.Add("GroundDown", GroundDown);
        ScrollingObjects.Add("BG", BG);
    }
    public void StartSpawnObstacle()
    {
        StartCoroutine(CoSpawnObstacle());
    }
    IEnumerator CoSpawnObstacle()
    {
        System.Random random = new System.Random();

        while (true)
        {
            if (Manager.Game.IsGameRunning)
            {
                int i = random.Next(0, _obstacleSizeList.Count);
                int upSize = _obstacleSizeList[i][0];
                int downSize = _obstacleSizeList[i][1];

                if (upSize != 0)
                {
                    var go = Instantiate(RockUp, UpParent);
                    int ranX = random.Next(1, upSize + 1);
                    go.transform.localScale = new Vector3(ranX, -upSize, 1);
                }
                if (downSize != 0)
                {
                    var go = Instantiate(RockDown, DownParent);
                    int ranX = random.Next(1, downSize + 1);
                    go.transform.localScale = new Vector3(ranX, downSize, 1);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public override void UpdatePosition(GameObject go)
    {
        if (ScrollingObjects.ContainsKey(go.tag) == false)
        {
            GameObject.Destroy(go);
            return;
        }

        ScrollingObject[] objects = ScrollingObjects[go.tag];

        Vector2 max = Vector2.zero;
        foreach (ScrollingObject g in objects)
        {
            var collider = g.GetComponent<Collider2D>();
            if (collider.bounds.max.x > max.x)
                max = collider.bounds.max;
        }

        Vector2 min = go.GetComponent<Collider2D>().bounds.min;
        float offsetX = go.transform.position.x - min.x;
        go.transform.position = new Vector3(max.x + offsetX, go.transform.position.y, go.transform.position.z);
    }
}