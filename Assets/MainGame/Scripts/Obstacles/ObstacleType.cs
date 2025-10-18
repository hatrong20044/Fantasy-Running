using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleType : MonoBehaviour
{
    public enum ObstacleSubType
    {
        Jumpable,
        Slideable,
        NonPassableStatic,
        NonPassableDynamic
    }

    public ObstacleSubType subType;
}
