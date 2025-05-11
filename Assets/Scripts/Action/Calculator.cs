using UnityEngine;

namespace Action
{
    public static class Calculator
    {
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
    }
}