using System.Collections.Generic;
using UnityEngine;

namespace Demon
{
    public sealed class SpawnState : IState
    {
        StateControl _context;
        float _waitTime;

        public void Enter(StateControl control)
        {
            _context = control;

            var spawnPoint = GetSpawnPoint();
            control.transform.SetPositionAndRotation(spawnPoint, Quaternion.identity);

            _context.PatrolPath.Spawn(spawnPoint);
            _waitTime = 0;
        }

        public void Update()
        {
            if (_context == null) return;

            _waitTime += Time.deltaTime;
            if (_waitTime > _context.Settings.Spawn.WaitTime)
            {
                _context.ChangeState(new WanderState());
            }
        }

        public void Exit()
        {
            _context = null;
        }

        private Vector3 GetSpawnPoint()
        {
            var pool = new List<Vector3>();

            foreach (var x in _context.PatrolPath.Points)
            {
                if (Vector3.Distance(x.Point, _context.PlayerPosition) > _context.Settings.Spawn.DistanceToPlayer)
                {
                    pool.Add(x.Point);
                }
            }

            var index = Random.Range(0, pool.Count - 1);
            return pool[index];
        }
    }
}