using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    private Vector3 startPos;
    private float distanceTravelled;

    public float DistanceTravelled => distanceTravelled;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        distanceTravelled = Vector3.Distance(startPos, transform.position);
    }

    public float GetDistance() => distanceTravelled;

    public void ResetProgress()
    {
        startPos = transform.position;
        distanceTravelled = 0f;
    }
}
