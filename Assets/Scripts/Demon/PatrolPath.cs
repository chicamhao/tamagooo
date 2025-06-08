using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Demon
{
    public sealed class PatrolPath : MonoBehaviour
    {
        private DemonSettings _settings;
        private WayPoint _currentPoint;

        private readonly List<WayPoint> _points = new();
        public IReadOnlyList<WayPoint> Points => _points;

        private readonly Dictionary<WayPoint, float> _visitedTimes = new();

        public float _wayPointDistance = 5f;

        public void Initialize(DemonSettings settings)
        {
            _settings = settings;
            for (var i = 0; i < transform.childCount; i++)
            {
                var position = transform.GetChild(i).position;
                _points.Add(new WayPoint(position));
            }
        }

        public void Spawn(Vector3 spawnPoint)
        {
            _currentPoint = _points.Find(x => x.Point == spawnPoint);
            _visitedTimes[_currentPoint] = Time.time;
        }

        public WayPoint Next()
        {
            _visitedTimes[_currentPoint] = Time.time;
            var from = _currentPoint.Point;
            foreach (var point in _points.OrderBy(x => Vector3.Distance(x.Point, from)))
            {
                if (IsRecentlyVisited(point))
                    continue;

                _currentPoint = point;
                return point;
            }

            return _currentPoint;
        }

        bool IsRecentlyVisited(WayPoint point)
        {
            if (_visitedTimes.TryGetValue(point, out float lastTime))
                return (Time.time - lastTime) < _settings.Wander.VisitedDuration;
            return false;
        }


        private void OnDrawGizmos()
        {
            if (_points.Count == 0)
            {
                for (var i = 0; i < transform.childCount; i++)
                {
                    var position = transform.GetChild(i).position;
                    _points.Add(new WayPoint(position));
                }
            }

            foreach (var x in _points)
            {
                var color = Color.green;
                if (IsRecentlyVisited(x))
                {
                    color = Color.red;
                }
                Gizmos.color = color;
                Gizmos.DrawSphere(x.Point, .2f);
            }
        }
    }

    public readonly struct WayPoint
    {
        public readonly Vector3 Point;

        public WayPoint(Vector3 point)
        {
            Point = point;
        }
    }
}

