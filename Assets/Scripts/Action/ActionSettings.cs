using System;
using UnityEngine;

namespace Action
{
    [CreateAssetMenu(fileName = "ActionSettings", menuName = "ScriptableObjects/ActionSettings", order = 1)]
    public sealed class ActionSettings : ScriptableObject
    {
        public MoveSettings Move;
        public JumpSettings Jump;
        public CrouchSettings Crouch;
    }

    [Serializable]
    public sealed class MoveSettings
    {
        [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;

        [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float MaxSpeedOnGround = 13f;

        [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15f;

        [Tooltip("Multiplication for the sprint speed (based on grounded speed)")]
        public float SprintSpeedModifier = 1.5f;
    }

    [Serializable]
    public sealed class JumpSettings
    {
        [Tooltip("Vertical speed")]
        public float JumpForce = 9f;

        [Tooltip("Max movement speed when not grounded")]
        public float MaxSpeedInAir = 25f;

        [Tooltip("Acceleration speed when in the air")]
        public float AccelerationSpeedInAir = 25f;

        [Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 12f;

        [Tooltip("Height of character when crouching")]
        public float CapsuleHeightStanding = 1.8f;
    }

    [Serializable]
    public sealed class CrouchSettings
    {
        [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Speed of crouching transitions")]
        public float CrouchingSharpness = 10f;

        [Tooltip("Height of character when standing")]
        public float CapsuleHeightStanding = 1.8f;

        [Tooltip("Height of character when crouching")]
        public float CapsuleHeightCrouching = 0.9f;

        [Tooltip("Max movement speed when crouching")]
        [Range(0, 1)]
        public float MaxSpeedCrouchedRatio = 0.5f;
    }
}
