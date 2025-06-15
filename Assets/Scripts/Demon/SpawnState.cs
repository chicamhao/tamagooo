using System.Collections.Generic;
using UnityEngine;

namespace Demon
{
    public sealed class SpawnState : IState
    {
        StateControl _context;
        SpawnSettings _settings;
        float _waitTime;

        public void Enter(StateControl control)
        {
            _context = control;
            _settings = _context.Settings.Spawn;

            var spawnPoint = GetSpawnPoint();
            control.Agent.Warp(spawnPoint.position);
            control.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);

            _context.PatrolPath.Spawn(spawnPoint);
            _waitTime = 0;
        }

        public void Update()
        {
            _waitTime += Time.deltaTime;
            if (_waitTime > _settings.WaitTime)
            {
                _context.ChangeState(new WanderState());
            }
        }

        public void Exit()
        {
            _waitTime = 0;
        }

        private Transform GetSpawnPoint()
        {
            var pool = new List<Transform>();

            foreach (var x in _context.PatrolPath.Points)
            {
                if (Vector3.Distance(x.position, _context.PlayerPosition) > _settings.DistanceToPlayer)
                {
                    pool.Add(x);
                }
            }

            var index = Random.Range(0, pool.Count - 1);
            return pool[index];
        }
    }
}