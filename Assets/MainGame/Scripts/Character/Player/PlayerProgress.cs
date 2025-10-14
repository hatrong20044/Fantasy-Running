using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public float DistanceTravelled { get; private set; }

    private Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        DistanceTravelled = Vector3.Distance(startPos, transform.position);
    }
}
