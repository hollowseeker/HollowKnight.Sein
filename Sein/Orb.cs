using System.Runtime.InteropServices;
using UnityEngine;

namespace Sein
{
    internal class Orb : MonoBehaviour
    {
        private static float ACCEL = 40f;
        private static float MAX_SPEED = 65f;
        private static float MAX_IDLE_VELOCITY = 5f;
        private static float Y_OFFSET = 0.25f;
        private static float Y_RANGE = 0.2f;
        private static float Y_PERIOD = 1.1f;
        private static float X_RANGE = 0.4f;
        private static float X_PERIOD = 1.965f;
        private static Vector3 TARGET_SIZE => new(X_RANGE * 2, Y_RANGE * 2, 1);

        private HeroController controller;
        private GameObject knight;
        private Vector3 target;

        private bool KnightRight => controller.cState.facingRight;

        private Vector3 KnightPos => knight.transform.position;

        private Vector3 TargetPos => KnightPos + new Vector3(0, Y_OFFSET, 0);

        private float xTimer = 0;
        private float yTimer = 0;
        private Vector3 prevTarget;

        protected void Awake()
        {
            controller = GameManager.instance.hero_ctrl;
            knight = controller.gameObject;
            prevTarget = KnightPos;
        }

        private Vector3 ComputeNewTarget()
        {
            xTimer += Time.deltaTime;
            yTimer += Time.deltaTime;
            if (xTimer > X_PERIOD) xTimer -= X_PERIOD;
            if (yTimer > Y_PERIOD) yTimer -= Y_PERIOD;

            var center = TargetPos;
            var newX = center.x + X_RANGE * Mathf.Sin(2 * xTimer * Mathf.PI / X_PERIOD);
            var newY = center.y + Y_RANGE * Mathf.Cos(2 * yTimer * Mathf.PI / Y_PERIOD);
            Vector3 newTarget = new(newX, newY, 0);

            var diff = newTarget - prevTarget;
            if (diff.sqrMagnitude > MAX_IDLE_VELOCITY * MAX_IDLE_VELOCITY)
                newTarget = prevTarget + diff.normalized * MAX_IDLE_VELOCITY;

            Bounds targetBounds = new(center, TARGET_SIZE);
            if (!targetBounds.Contains(newTarget))
            {
                // Drag target.
                if (newTarget.x > targetBounds.max.x) newTarget.x = targetBounds.max.x;
                else if (newTarget.x < targetBounds.min.x) newTarget.x = targetBounds.min.x;
                if (newTarget.y > targetBounds.max.y) newTarget.y = targetBounds.max.y;
                else if (newTarget.y < targetBounds.min.y) newTarget.y = targetBounds.min.y;
            }
            return newTarget;
        }

        private Vector3 velocity;

        private void AccelerateTo(Vector3 target)
        {
            // FIXME
        }

        protected void Update()
        {
            var newTarget = ComputeNewTarget();
            AccelerateTo(newTarget);
            prevTarget = newTarget;
        }
    }
}
