using UnityEngine;

/// <summary> Хранит 4 точечных ориентиров вокруг коробки. </summary>
public class BoxGripPoints : MonoBehaviour
{
    [Tooltip("Front, Back, Left, Right")]
    public Transform[] points = new Transform[4];
}
