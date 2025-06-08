using System;
using UnityEngine;

namespace Demon
{
    [CreateAssetMenu(fileName = "DemonSettings", menuName = "ScriptableObjects/DemonSettings", order = 1)]
    public sealed class DemonSettings : ScriptableObject
    {
        public SpawnSettings Spawn;
        public WanderSettings Wander;
    }

    [Serializable]
    public sealed class SpawnSettings
    {
        [Tooltip("Minimum distance from the next spawn point to the player.")]
        public float DistanceToPlayer = 15f;

        [Tooltip("Duration from idle state to wander state.")]
        public float WaitTime = 30f;
    }

    [Serializable]
    public sealed class WanderSettings
    {
        public float Speed = 4;

        [Tooltip("Way points is currently visited are excluded from the candidate list.")]
        public float VisitedDuration = 15f;
    }
}
