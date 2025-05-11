using UnityEngine;
using UnityEngine.AI;

namespace Action
{
    public sealed class Mover : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        
        static readonly int s_forwardSpeedAnimation = Animator.StringToHash("_forwardSpeed");
        
        private void Update()
        {
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            // transform velocity direction from global space to local space.
            var velocity = transform.InverseTransformDirection(_navMeshAgent.velocity);
            
            // forward direction.
            _animator.SetFloat(s_forwardSpeedAnimation, Mathf.Abs(velocity.z));
        }
    }
}