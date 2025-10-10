using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    [System.Serializable]
    public class CoinPattern
    {
        public string name;
        public Vector3[] positions;
    }

    [SerializeField] private List<CoinPattern> patterns;
    [SerializeField] private float laneDistance = 2.5f;
    [SerializeField] private float coinSpacing = 2f;

    private void Awake()
    {
        this.InitializePatterns();
    }

    private void InitializePatterns()
    {
        if (patterns.Count == 0)
        {
            patterns = new()
            {
                new CoinPattern
                {
                    name = "Vertical",
                    positions = this.PositionVertical()
                },
                new CoinPattern
                {
                    name = "Horizontal",
                    positions = new Vector3[]
                    {
                        new Vector3(-laneDistance, 1, 0),
                        new Vector3(0,1,0),
                        new Vector3(laneDistance,1,0)
                    }
                }
            };
        }
    }

    public Vector3[] PositionVertical()
    {
        int index = Random.Range(3, 8);
        Vector3[] pos = new Vector3[index];
        for(int i = 0; i < index; i++)
        {
            pos[i] = new Vector3(0,1, i * coinSpacing);
        }
        return pos;
    }



    public CoinPattern GetRandomPattern()
    {
        return patterns[Random.Range(0, patterns.Count)];
    }

    public float SpawnRandomDistance()
    {
        return Random.Range(20f, 40f);
    }



}