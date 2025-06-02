using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Demon
{
    public sealed class PatrolPath : MonoBehaviour
    {
        public float _wayPointDistance = 5f;

        private void OnDrawGizmos()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var position = transform.GetChild(i).position;
                Gizmos.DrawSphere(position, .2f);
                foreach (var p in GetNextWayPoints(i))
                {
                    Gizmos.DrawLine(position, p);
                }
            }
        }

        private List<Vector3> GetNextWayPoints(int index)
        {
            var from = transform.GetChild(index).position;
            var points = new List<Vector3>();  
            for (var i = 0; i < transform.childCount; i++)
            {
                if (i == index) continue;

                var to = transform.GetChild(i).position;
                var distance = Vector3.Distance(from, to);

                if (distance < _wayPointDistance)
                {
                    points.Add(transform.GetChild(i).position);
                }
            }
            return points;
        }
    }
}

