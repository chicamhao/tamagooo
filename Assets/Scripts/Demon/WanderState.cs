using UnityEngine;
using UnityEngine.AI;

namespace Demon
{
    public class WanderState : IState
    {
        private StateControl _control;
        private PatrolPath _path => _control.PatrolPath;

        private NavMeshAgent _agent => _control.Agent;

        private Vector3 _destination;
        public Vector3 Destionation => _destination;

        private float _idleTime;
        private float _idleTimeDuration;

        private float _waitTime;

        private WanderSettings _settings;

        Vector3 _currentPoint;

        public void Enter(StateControl control)
        {
            _control = control;
            _settings = control.Settings.Wander;
            ToUnvisited();
        }

        public void Update()
        { 
            _agent.speed = _settings.Speed;

            _waitTime += Time.deltaTime;
            if (_waitTime >= _settings.WaitTime)
            {
                _agent.isStopped = true;
                _control.ChangeState(new DespawnState());
            }

            if (_idleTimeDuration != 0)
            {
                _idleTime += Time.deltaTime;
                if (_idleTime > _idleTimeDuration)
                {
                    _idleTime = _idleTimeDuration = 0;

                    ToUnvisited();
                }
                else
                {
                    return; // keep idle state
                }
            }

            if (IsAtNextPoint())
            {
                _path.CurrentPoint = _path.NextPoint;

                // to idle.
                if (_path.IsCurrentPointSingle())
                {
                    _idleTime = 0;
                    _idleTimeDuration = Random.Range(_settings.IdleDurationMin, _settings.IdleDurationMax);
                }
                else
                {
                    ToUnvisited();
                }
            }
        }

        private bool IsAtNextPoint()
        {
            return !_agent.pathPending && _agent.remainingDistance < 0.5f;
        }

        private void ToUnvisited()
        {
            var next = _control.PatrolPath.GetUnvisitedPoint();
            _control.PatrolPath.NextPoint = next;
            _agent.SetDestination(next.position);
        }

        public void Exit()
        {
            _waitTime = _idleTime = _idleTimeDuration = 0;
            _agent.ResetPath();
            _agent.isStopped = true;
        }
    }
}