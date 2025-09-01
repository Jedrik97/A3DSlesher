using UnityEngine;

public class CameraAnchorFollower : MonoBehaviour
{
    public Transform Target;
    public Vector3 PositionOffset = new Vector3(0, 1.6f, 0);
    public Vector3 RotationOffsetEuler;
    public float PositionSmoothTime = 0.08f;
    public float RotationSmoothTime = 0.1f;
    public float LookAheadMultiplier = 0.6f;
    public float LookAheadMax = 2f;
    public float TeleportDistance = 10f;
    public UpdateMode Mode = UpdateMode.LateUpdate;

    Vector3 _posVelocity;
    Vector3 _lastTargetPos;
    Quaternion _rotVelocity;
    bool _hasLast;

    void Reset()
    {
        PositionSmoothTime = 0.08f;
        RotationSmoothTime = 0.1f;
        LookAheadMultiplier = 0.6f;
        LookAheadMax = 2f;
        TeleportDistance = 10f;
        Mode = UpdateMode.LateUpdate;
    }

    void Update()
    {
        if (Mode == UpdateMode.Update) Tick(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Mode == UpdateMode.FixedUpdate) Tick(Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        if (Mode == UpdateMode.LateUpdate) Tick(Time.deltaTime);
    }

    void Tick(float dt)
    {
        if (Target == null) return;

        var targetPos = Target.position;
        if (!_hasLast) { _lastTargetPos = targetPos; _hasLast = true; transform.position = targetPos + PositionOffset; transform.rotation = Target.rotation * Quaternion.Euler(RotationOffsetEuler); return; }

        var delta = targetPos - _lastTargetPos;
        var velocity = delta / Mathf.Max(dt, 0.0001f);

        var lookAhead = Vector3.ClampMagnitude(velocity * LookAheadMultiplier, LookAheadMax);
        var desiredPos = targetPos + PositionOffset + lookAhead;

        if (Vector3.Distance(transform.position, desiredPos) > TeleportDistance)
        {
            transform.position = desiredPos;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _posVelocity, Mathf.Max(0.0001f, PositionSmoothTime), Mathf.Infinity, dt);
        }

        var desiredRot = Target.rotation * Quaternion.Euler(RotationOffsetEuler);
        transform.rotation = SmoothDampRotation(transform.rotation, desiredRot, ref _rotVelocity, Mathf.Max(0.0001f, RotationSmoothTime), dt);

        _lastTargetPos = targetPos;
    }

    public void WarpToTarget()
    {
        if (Target == null) return;
        _hasLast = false;
        transform.position = Target.position + PositionOffset;
        transform.rotation = Target.rotation * Quaternion.Euler(RotationOffsetEuler);
    }

    public enum UpdateMode { Update, FixedUpdate, LateUpdate }

    static Quaternion SmoothDampRotation(Quaternion current, Quaternion target, ref Quaternion deriv, float smoothTime, float deltaTime)
    {
        if (deltaTime < Mathf.Epsilon) return current;
        float dot = Quaternion.Dot(current, target);
        float multi = dot > 0f ? 1f : -1f;
        target = new Quaternion(target.x * multi, target.y * multi, target.z * multi, target.w * multi);

        var res = new Vector4(
            Mathf.SmoothDamp(current.x, target.x, ref deriv.x, smoothTime, Mathf.Infinity, deltaTime),
            Mathf.SmoothDamp(current.y, target.y, ref deriv.y, smoothTime, Mathf.Infinity, deltaTime),
            Mathf.SmoothDamp(current.z, target.z, ref deriv.z, smoothTime, Mathf.Infinity, deltaTime),
            Mathf.SmoothDamp(current.w, target.w, ref deriv.w, smoothTime, Mathf.Infinity, deltaTime)
        ).normalized;

        var dtInv = 1f / Mathf.Max(deltaTime, 0.0001f);
        var diff = new Vector4(
            (res.x - current.x) * dtInv,
            (res.y - current.y) * dtInv,
            (res.z - current.z) * dtInv,
            (res.w - current.w) * dtInv
        );
        deriv = new Quaternion(
            deriv.x - diff.x,
            deriv.y - diff.y,
            deriv.z - diff.z,
            deriv.w - diff.w
        );
        return new Quaternion(res.x, res.y, res.z, res.w);
    }
}
