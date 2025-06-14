using UnityEngine;
using UnityEngine.AI;

namespace Demon
{
    public sealed class StateControl : MonoBehaviour
    {
        public PatrolPath PatrolPath => _patrolPath;
        [SerializeField] PatrolPath _patrolPath;

        public DemonSettings Settings => _settings;
        [SerializeField] DemonSettings _settings;

        [SerializeField] Animator _animator;
        [SerializeField] NavMeshAgent _agent;

        public Vector3 PlayerPosition => _playerPosition;
        private Vector3 _playerPosition;

        private IState _currentState;

        readonly static int s_forwardSpeedAnimation = Animator.StringToHash("_forwardSpeed");


        public void Start()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _patrolPath = FindFirstObjectByType<PatrolPath>();
            _playerPosition = FindFirstObjectByType<CharacterController>().transform.position;

            _patrolPath.Initialize(_settings);

            ChangeState(new SpawnState());
        }

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(this);
        }

        public void Update()
        {
            _currentState?.Update();
            UpdateAnimator();
        }


        private void UpdateAnimator()
        {
            // transform velocity direction from global space to local space
            var velocity = transform.InverseTransformDirection(_agent.velocity);

            // forward direction
            _animator.SetFloat(s_forwardSpeedAnimation, Mathf.Abs(velocity.z));
        }

        private void OnDrawGizmos()
        {
            if (_currentState is WanderState)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(transform.position, (_currentState as WanderState).Destionation);
            }

            Gizmos.DrawWireSphere(transform.position, _settings.Spawn.DistanceToPlayer);
        }
    }
}