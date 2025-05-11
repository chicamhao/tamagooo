using UnityEngine;

namespace Action
{
    public sealed class GroundHandle
    {
        static readonly float _groundDistance = 1f;
        static readonly float s_groundDistanceInAir = 0.07f; 
        static readonly float s_jumpGroundingPreventionTime = 0.2f;
        
        public bool IsGrounded { get; set; }
        public bool HasJumpedThisFrame { get; set; }
        public float LastTimeJumped { get; set; }
        public Vector3 GroundNormal { get; set; }
        public Vector3 Velocity { get; set; }

        private readonly CharacterController _controller;
        
        public GroundHandle(CharacterController controller)
        {
            _controller = controller;
        }        
        
        public void Validate(float time)
        {
            // only detect ground if it's been a short amount of time since last jump.
            // otherwise may snap to the ground instantly after trying to jump 
            if (!(time >= LastTimeJumped + s_jumpGroundingPreventionTime))
            {
                return;
            };
            
            // make sure that the ground check distance while already in air is very small,
            // to prevent suddenly snapping to ground.
            var chosenGroundCheckDistance =
                IsGrounded ? (_controller.skinWidth + _groundDistance) : s_groundDistanceInAir;

            IsGrounded = false;
            GroundNormal = Vector3.up;

            // collect info of the ground normal with a downward capsule cast representing character capsule.
            if (Physics.CapsuleCast(
                    Calculator.GetCapsuleBottomHemisphere(_controller),
                    Calculator.GetCapsuleTopHemisphere(_controller),
                    _controller.radius, Vector3.down, out var hit, chosenGroundCheckDistance))
            {
                // storing upward direction for the surface found.
                GroundNormal = hit.normal;

                if (Vector3.Dot(GroundNormal, _controller.transform.up) >
                    0f // ground normal goes in the same direction as character up?
                    && Vector3.Angle(_controller.transform.up, GroundNormal) < _controller.slopeLimit)
                {
                    IsGrounded = true;
                    // snap to the ground.
                    if (hit.distance < _controller.skinWidth)
                    {
                        _controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }
}