using UnityEngine;

public class CameraAnchorFollower : MonoBehaviour
{
    [Header("Target")]
    public Transform Target;
    public Vector3 PositionOffset = new Vector3(0, 1.6f, 0);

    [Header("Follow Rotation Base Offset")]
    public Vector3 RotationOffsetEuler;

    [Header("Stabilized Follow (limit over-follow)")]
    [Range(0f, 1f)] public float YawFollowGain = 0.6f;    
    [Range(0f, 1f)] public float PitchFollowGain = 0.4f;  
    [Range(0f, 1f)] public float RollFollowGain = 0.0f;   

    [Tooltip("Dead zone in degrees where camera won't react")]
    public float YawDeadZone = 3f;
    public float PitchDeadZone = 2f;

    [Tooltip("Clamp error to keep the target in frame")]
    public float MaxYawError = 35f;
    public float MaxPitchError = 20f;

    [Tooltip("Max turn speed (deg/s)")]
    public float MaxYawSpeed = 240f;
    public float MaxPitchSpeed = 180f;

    [Tooltip("Use world up for horizon stability")]
    public bool UseWorldUp = true;

    [Header("Translational Smoothing + Look-Ahead")]
    public float PositionSmoothTime = 0.08f;
    public float LookAheadMultiplier = 0.6f;
    public float LookAheadMax = 2f;

    [Header("Other")]
    public float TeleportDistance = 10f;
    public UpdateMode Mode = UpdateMode.LateUpdate;

    [Header("Manual Rotation Blend")]
    [Range(0f, 1f), Tooltip("0 = follow only, 1 = manual only")]
    public float ManualRotationBlend = 0f;

    public enum RotationOffsetSpace { Self, World }
    [Tooltip("Space for manual rotation (Self = add after follow)")]
    public RotationOffsetSpace ManualSpace = RotationOffsetSpace.Self;
    
    Vector3 _posVelocity;
    Vector3 _lastTargetPos;
    bool _hasLast;

    float _yaw, _pitch;          
    float _yawVel, _pitchVel;
    
    Vector3 _manualEuler; bool _hasManual;

    void Reset()
    {
        PositionSmoothTime = 0.08f;
        LookAheadMultiplier = 0.6f;
        LookAheadMax = 2f;
        TeleportDistance = 10f;

        YawFollowGain = 0.6f;
        PitchFollowGain = 0.4f;
        RollFollowGain = 0.0f;
        YawDeadZone = 3f;
        PitchDeadZone = 2f;
        MaxYawError = 35f;
        MaxPitchError = 20f;
        MaxYawSpeed = 240f;
        MaxPitchSpeed = 180f;
        UseWorldUp = true;

        ManualRotationBlend = 0f;
        ManualSpace = RotationOffsetSpace.Self;

        Mode = UpdateMode.LateUpdate;
    }

    void Update()      { if (Mode == UpdateMode.Update) Tick(Time.deltaTime); }
    void FixedUpdate() { if (Mode == UpdateMode.FixedUpdate) Tick(Time.fixedDeltaTime); }
    void LateUpdate()  { if (Mode == UpdateMode.LateUpdate) Tick(Time.deltaTime); }

    void Tick(float dt)
    {
        if (!Target) return;

        var targetPos = Target.position;

        if (!_hasLast)
        {
            _lastTargetPos = targetPos; _hasLast = true;

            var baseFollow = Target.rotation * Quaternion.Euler(RotationOffsetEuler);
            ExtractYawPitch(baseFollow, out _yaw, out _pitch);
            ApplyRotationNow();
            transform.position = targetPos + PositionOffset;
            return;
        }
        
        var delta = targetPos - _lastTargetPos;
        var velocity = delta / Mathf.Max(dt, 0.0001f);
        var lookAhead = Vector3.ClampMagnitude(velocity * LookAheadMultiplier, LookAheadMax);
        var desiredPos = targetPos + PositionOffset + lookAhead;

        if (Vector3.Distance(transform.position, desiredPos) > TeleportDistance)
            transform.position = desiredPos;
        else
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _posVelocity,
                Mathf.Max(0.0001f, PositionSmoothTime), Mathf.Infinity, dt);

        // --- stabilized angles ---
        var followRot = Target.rotation * Quaternion.Euler(RotationOffsetEuler);
        ExtractYawPitch(followRot, out float goalYaw, out float goalPitch);

        float yawErr = Mathf.DeltaAngle(_yaw, goalYaw);
        float pitchErr = Mathf.DeltaAngle(_pitch, goalPitch);

        if (Mathf.Abs(yawErr) <= YawDeadZone) yawErr = 0f;
        if (Mathf.Abs(pitchErr) <= PitchDeadZone) pitchErr = 0f;

        yawErr = Mathf.Clamp(yawErr, -MaxYawError, MaxYawError);
        pitchErr = Mathf.Clamp(pitchErr, -MaxPitchError, MaxPitchError);

        float targetYawCam = _yaw + yawErr * YawFollowGain;
        float targetPitchCam = _pitch + pitchErr * PitchFollowGain;

        _yaw   = SmoothDampAngleClamped(_yaw,   targetYawCam, ref _yawVel,   0.10f, MaxYawSpeed,   dt);
        _pitch = SmoothDampAngleClamped(_pitch, targetPitchCam, ref _pitchVel, 0.10f, MaxPitchSpeed, dt);

        var stabilized = ComposeFromYawPitch(_yaw, _pitch, UseWorldUp ? Vector3.up : Target.up);
        var finalRot = ApplyManual(stabilized);
        transform.rotation = finalRot;

        _lastTargetPos = targetPos;
    }

    void ApplyRotationNow()
    {
        var stabilized = ComposeFromYawPitch(_yaw, _pitch, UseWorldUp ? Vector3.up : Target.up);
        transform.rotation = ApplyManual(stabilized);
    }

    Quaternion ApplyManual(Quaternion followRot)
    {
        if (!_hasManual || ManualRotationBlend <= 0f) return followRot;
        var manualWorld = (ManualSpace == RotationOffsetSpace.Self)
            ? (followRot * Quaternion.Euler(_manualEuler))
            : Quaternion.Euler(_manualEuler);
        if (ManualRotationBlend >= 1f) return manualWorld;
        return Quaternion.Slerp(followRot, manualWorld, ManualRotationBlend);
    }

    static void ExtractYawPitch(Quaternion rot, out float yaw, out float pitch)
    {
        var fwd = rot * Vector3.forward;
        yaw = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;
        var levelFwd = Vector3.ProjectOnPlane(fwd, Vector3.up).normalized;
        float signed = Vector3.SignedAngle(levelFwd, fwd, Vector3.Cross(levelFwd, Vector3.up));
        pitch = Mathf.Clamp(signed, -89.9f, 89.9f);
    }

    static Quaternion ComposeFromYawPitch(float yaw, float pitch, Vector3 up)
    {
        var qYaw = Quaternion.AngleAxis(yaw, up);
        var right = Vector3.Cross(up, qYaw * Vector3.forward).normalized;
        var qPitch = Quaternion.AngleAxis(pitch, right);
        return qPitch * qYaw;
    }

    static float SmoothDampAngleClamped(float current, float target, ref float vel, float smoothTime, float maxSpeedDegPerSec, float dt)
    {
        float next = Mathf.SmoothDampAngle(current, target, ref vel, Mathf.Max(0.0001f, smoothTime), Mathf.Infinity, dt);
        float maxStep = maxSpeedDegPerSec * dt;
        float delta = Mathf.DeltaAngle(current, next);
        delta = Mathf.Clamp(delta, -maxStep, maxStep);
        return current + delta;
    }
    
    public void SetManualEuler(Vector3 eulerDegrees) { _manualEuler = NormalizeEuler(eulerDegrees); _hasManual = true; }
    public void AddYaw(float d)   { _manualEuler.y = WrapAngle(_manualEuler.y + d); _hasManual = true; }
    public void AddPitch(float d) { _manualEuler.x = Mathf.Clamp(WrapAngle(_manualEuler.x + d), -89.9f, 89.9f); _hasManual = true; }
    public void AddRoll(float d)  { _manualEuler.z = WrapAngle(_manualEuler.z + d); _hasManual = true; }
    public void ClearManualRotation() { _manualEuler = Vector3.zero; _hasManual = false; }

    static Vector3 NormalizeEuler(Vector3 e) => new Vector3(WrapAngle(e.x), WrapAngle(e.y), WrapAngle(e.z));
    static float WrapAngle(float a){ a %= 360f; if (a > 180f) a -= 360f; if (a < -180f) a += 360f; return a; }

    public enum UpdateMode { Update, FixedUpdate, LateUpdate }
}
