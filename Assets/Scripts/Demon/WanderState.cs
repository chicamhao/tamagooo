using UnityEngine;
using UnityEngine.AI;

namespace Demon
{
    public class WanderState : IState
    {
        private StateControl _control;
        private NavMeshAgent _agent;

        private Vector3 _destination;
        public Vector3 Destionation => _destination;

        public void Enter(StateControl control)
        {
            _control = control;
            _agent = control.GetComponent<NavMeshAgent>();
            _agent.speed = control.Settings.Wander.Speed;
            SetNextDestination();
        }

        public void Update()
        {
            //if (Vector3.Distance(ai.transform.position, ai.player.position) < ai.config.chaseDistance ||
            //    ai.GetLightIntensity() > ai.config.lightAggroThreshold)
            //{
            //    ai.ChangeState(new ChaseState());
            //    return;
            //}

            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                SetNextDestination();
            }
        }

        public void Exit() { }

        void SetNextDestination()
        {
            var next = _control.PatrolPath.Next();
            _agent.SetDestination(next.Point);
        }
    }
}