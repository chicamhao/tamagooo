using Action;
using UnityEngine;
using UnityEngine.AI;

namespace Helper
{
    public static class Calculator
    {
        private static readonly Collider[] _colliders = new Collider[3];

        // get the center point of the bottom hemisphere of the character.
        public static Vector3 GetCapsuleBottomHemisphere(CharacterController controller)
        {
            return controller.transform.position + (controller.transform.up * controller.radius);
        }
        
        // get the center point of top hemisphere of the character.
        public static Vector3 GetCapsuleTopHemisphere(CharacterController controller, float height)
        {
            return controller.transform.position + (controller.transform.up * (height - controller.radius));
        }
        
        public static Vector3 GetCapsuleTopHemisphere(CharacterController controller)
        {
            return GetCapsuleTopHemisphere(controller, controller.height);
        }
        
        // Gets a reoriented direction that is tangent to a given slope
        public static Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal, Vector3 up)
        {
            var directionRight = Vector3.Cross(direction, up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        // returns false if there was an obstruction.
        public static bool Standable(ActionContext context, float height)
        {
            _colliders[0] = null;
            _colliders[1] = null;
            _colliders[2] = null;

            // detect obstructions
            Physics.OverlapCapsuleNonAlloc(
                GetCapsuleBottomHemisphere(context.Controller),
                GetCapsuleTopHemisphere(context.Controller, height),
                context.Controller.radius, _colliders);

            foreach (Collider c in _colliders)
            {
                if (c != null && c != context.Controller && !c.isTrigger)
                {
                    return false;
                }
            }

            return true;
        }

        public static float GetNavMeshDistance(Vector3 start, Vector3 end)
        {
            NavMeshPath path = new();
            if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
            {
                float totalDistance = 0f;
                for (int i = 1; i < path.corners.Length; i++)
                {
                    totalDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                return totalDistance;
            }
            return -1f; // path not found
        }

        public static Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return center;
        }
    }
}