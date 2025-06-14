using System;
using UnityEngine;

namespace Demon
{
    [CreateAssetMenu(fileName = "DemonSettings", menuName = "ScriptableObjects/DemonSettings", order = 1)]
    public sealed class DemonSettings : ScriptableObject
    {
        public SpawnSettings Spawn;
        public DespawnSettings Despawn;
        public WanderSettings Wander;
    }

    [Serializable]
    public sealed class SpawnSettings
    {
        [Tooltip("Minimum distance from the next spawn point to the player.")]
        public float DistanceToPlayer = 15f;

        [Tooltip("Waiting time to move to wander state.")]
        public float WaitTime = 30f;
    }

    [Serializable]
    public sealed class WanderSettings
    {
        public float Speed = 4;

        [Tooltip("Way points is currently visited are excluded from the candidate list.")]
        public float VisitedDuration = 120f;

        public float IdleDurationMin = 2f;
        public float IdleDurationMax = 5f;

        [Tooltip("Waiting time to move to despawn state.")]
        public float WaitTime = 60f;
    }

    [Serializable]
    public sealed class DespawnSettings
    {
        public float DisappearDuration = 2f;

        [Tooltip("Waiting time to move to spawn state.")]
        public float WaitTime = 10f;
    }
}
