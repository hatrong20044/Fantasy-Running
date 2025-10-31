using UnityEngine;
using UnityEngine.Rendering;

public class PlayerProgress : MonoBehaviour
{
    private Vector3 startPos;
    private float distanceTravelled;
    public static PlayerProgress Instance;

    public float DistanceTravelled => distanceTravelled;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        distanceTravelled = Vector3.Distance(startPos, transform.position);
    }

    public void ResetProgress()
    {
        startPos = transform.position;
        distanceTravelled = 0f;
    }
}
