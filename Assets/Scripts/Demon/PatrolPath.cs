using System.Collections.Generic;
using System.Linq;
using Helper;
using UnityEngine;

namespace Demon
{
    public sealed class PatrolPath : MonoBehaviour
    {
        private DemonSettings _settings;

        private Transform _currentPoint;
        public Transform CurrentPoint
        {
            get { return _currentPoint; }
            set { _currentPoint = value; }
        }

        private Transform _nextPoint;
        public Transform NextPoint
        {
            get { return _nextPoint; }
            set
            {
                _visitedTimes[value] = Time.time;
                _nextPoint = value;
            }
        }


        private readonly List<Transform> _points = new();
        public IReadOnlyList<Transform> Points => _points;

        public readonly Dictionary<Transform, int> _connectedCounters = new();
        private readonly Dictionary<Transform, float> _visitedTimes = new();

        public float _pointDistance = 10f;

        public void Initialize(DemonSettings settings)
        {
            _settings = settings;
            for (var i = 0; i < transform.childCount; i++)
            {
                var point = transform.GetChild(i);
                _points.Add(point);
                _connectedCounters[point] = GetNextPoints(point).Count;
            }
        }

        public void Spawn(Transform spawnPoint)
        {
            _visitedTimes.Clear();

            _currentPoint = spawnPoint;
            _visitedTimes[_currentPoint] = Time.time;
        }

        public bool IsCurrentPointSingle()
        {
            if (_connectedCounters.TryGetValue(_currentPoint, out int count))
            { 
                return count == 1;
            }
            return false;
        }

        public Transform GetUnvisitedPoint()
        {
            var from = _currentPoint;
            foreach (var point in _points.OrderBy(x => Calculator.GetNavMeshDistance(x.position, from.position)))
            {
                if (IsRecentlyVisited(point)) continue;

                return point;
            }

            Debug.Log("all visited, reset visiting cache.");
            _visitedTimes.Clear();

            return _currentPoint;
        }

        bool IsRecentlyVisited(Transform point)
        {
            if (_visitedTimes.TryGetValue(point, out float lastTime))
                return Time.time - lastTime < _settings.Wander.VisitedDuration;
            return false;
        }


        private List<Transform> GetNextPoints(Transform from)
        {
            var points = new List<Transform>();
            for (var i = 0; i < transform.childCount; i++)
            {

                var to = transform.GetChild(i);
                var distance = Vector3.Distance(from.position, to.position);

                if (from != to && distance < _pointDistance)
                {
                    points.Add(transform.GetChild(i));
                }
            }
            return points;
        }

        private void OnDrawGizmos()
        {
            _points.Clear();
            for (var i = 0; i < transform.childCount; i++)
            {
                var position = transform.GetChild(i);
                _points.Add(position);
            }

            foreach (var x in _points)
            {
                var color = Color.green;

                if (x == _nextPoint)
                {
                    color = Color.yellow;
                }
                else if (IsRecentlyVisited(x))
                {
                    color = Color.red;
                }

                Gizmos.color = color;
                Gizmos.DrawSphere(x.position, .2f);

                foreach (var p in GetNextPoints(x))
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(x.position, p.position);
                }
            }
        }
    }  
}

