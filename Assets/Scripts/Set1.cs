using UnityEngine;

public class Set1 : MonoBehaviour
{
    private static readonly Vector3 One = Vector3.one;
    // set transform is 1 so the robot don;t scale
    //no use
    void LateUpdate()
    {
        if (transform.localScale != One)
            transform.localScale = One;
    }
}