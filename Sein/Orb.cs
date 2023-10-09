using ItemChanger;
using UnityEngine;

namespace Sein;

internal class Orb : MonoBehaviour
{
    public static void Hook()
    {
        Events.OnSceneChange += _ => InstantiateOrb();
    }

    private static IC.EmbeddedSprite SeinSprite = new("Sein");

    private static void InstantiateOrb()
    {
        if (!SeinMod.OriActive()) return;

        GameObject orb = new("SeinOrb");
        orb.AddComponent<Orb>();
        orb.transform.localScale = new(SCALE, SCALE, 1);

        // TODO: Scaling
        var sr = orb.AddComponent<SpriteRenderer>();
        sr.sprite = SeinSprite.Value;
    }

    private static float SCALE = 0.6f;
    private static float ACCEL = 16f;
    private static float MAX_SPEED = 60f;
    private static float MAX_IDLE_VELOCITY = 5f;
    private static float Y_OFFSET = 0.4f;
    private static float Y_RANGE = 0.15f;
    private static float Y_PERIOD = 1.25f;
    private static float X_RANGE = 0.85f;
    private static float X_PERIOD = 3.15f;
    private static Vector3 TARGET_SIZE => new(X_RANGE * 2, Y_RANGE * 2, 1);
    private static float MAX_BRAKE_DISTANCE = MAX_SPEED * MAX_SPEED / (2 * ACCEL);

    private HeroController controller;
    private GameObject knight;

    private Vector3 KnightPos => knight.transform.position;

    private Vector3 TargetPos => KnightPos + new Vector3(0, Y_OFFSET, 0);

    protected void Awake()
    {
    }

    private float xTimer = 0;
    private float yTimer = 0;
    private Vector3 prevTarget;

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

    private Vector3 prevVelocity = Vector3.zero;

    private Vector3 ComputeTargetVelocity(Vector3 target)
    {
        var dist = target - transform.position;
        var mag = dist.magnitude;
        if (mag <= 1e-6f) return Vector3.zero;
        if (mag >= MAX_BRAKE_DISTANCE) return dist.normalized * MAX_SPEED;

        // d = v^2/2a
        // v = sqrt(2ad)
        var targetVel = Mathf.Sqrt(2 * ACCEL * mag);
        return dist.normalized * targetVel;
    }

    private void AccelerateTo(Vector3 target)
    {
        var targetVel = ComputeTargetVelocity(target);
        var dist = targetVel - prevVelocity;
        Vector3 newVelocity;
        if (dist.magnitude <= ACCEL * Time.deltaTime) newVelocity = targetVel;
        else newVelocity = prevVelocity + dist.normalized * ACCEL * Time.deltaTime;

        var travel = (newVelocity + prevVelocity) / 2 * Time.deltaTime;
        transform.position += travel;
        prevVelocity = newVelocity;
    }

    private bool Init()
    {
        if (controller != null) return true;

        controller = GameManager.instance.hero_ctrl;
        if (controller == null) return false;

        knight = controller.gameObject;
        prevTarget = KnightPos;
        transform.position = TargetPos;
        return true;
    }

    protected void Update()
    {
        if (!Init()) return;

        var newTarget = ComputeNewTarget();
        AccelerateTo(newTarget);
        prevTarget = newTarget;
    }
}
