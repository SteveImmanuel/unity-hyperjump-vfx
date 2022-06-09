using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointVisualizer : MonoBehaviour
{
    public float radius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }
}
