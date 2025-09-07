using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRigController : MonoBehaviour
{
    [Header("Target")]
    public Transform Target;
    public Vector3 PositionOffset = new Vector3(0, 1.6f, 0);

    [Header("Follow Rotation Base Offset")]
    public Vector3 RotationOffsetEuler;

    [Header("Stabilized Follow")]
    [Range(0f, 1f)] public float YawFollowGain = 0.6f;
    [Range(0f, 1f)] public float PitchFollowGain = 0.4f;
    [Range(0f, 1f)] public float RollFollowGain = 0f;
    public float YawDeadZone = 3f;
    public float PitchDeadZone = 2f;
    public float MaxYawError = 35f;
    public float MaxPitchError = 20f;
    public float MaxYawSpeed = 240f;
    public float MaxPitchSpeed = 180f;
    public bool UseWorldUp = true;

    [Header("Position Smoothing + LookAhead")]
    public float PositionSmoothTime = 0.08f;
    public float LookAheadMultiplier = 0.6f;
    public float LookAheadMax = 2f;
    public float TeleportDistance = 10f;

    [Header("Manual Rotation Blend")]
    [Range(0f, 1f)] public float ManualRotationBlend = 0f;
    public enum RotationOffsetSpace { Self, World }
    public RotationOffsetSpace ManualSpace = RotationOffsetSpace.Self;

    [Header("Auto Realign Behind (Movement)")]
    public bool AutoRealign = true;
    public bool UseMovementDirection = true;
    public float MovementSpeedThreshold = 0.35f;
    public float RealignTriggerAngle = 95f;
    public float RealignTargetAngularSpeed = 180f;
    public float RealignTime = 0.35f;
    public float RealignCooldown = 0.5f;
    public float RealignPitchBlend = 0.5f;

    [Header("External Motion (optional)")]
    public bool UseExternalVelocity = false;
    public Vector3 ExternalVelocity;

    [Header("Collision Avoidance")]
    public bool AvoidCollisions = true;
    public LayerMask GeometryMask = ~0;
    public float CollisionRadius = 0.25f;
    public float MinDistance = 0.2f;
    public float CollisionBuffer = 0.05f;
    public float CollisionSmoothTime = 0.04f;

    [Header("See-Through")]
    public bool SeeThrough = true;
    public OccluderMode SeeThroughMode = OccluderMode.ShadowsOnly;
    public float SeeThroughRayRadius = 0.02f;
    [Range(0.02f, 1f)] public float FadeAlpha = 0.25f;
    public float FadeOutTime = 0.08f;
    public float FadeInTime = 0.15f;
    public int MaxOccluders = 8;
    public bool KeepShadowsWhenHidden = true;

    [Header("Update")]
    public UpdateMode Mode = UpdateMode.LateUpdate;

    Vector3 _posVelocity;
    Vector3 _colVelocity;
    Vector3 _lastTargetPos;
    bool _hasLast;

    float _yaw, _pitch;
    float _yawVel, _pitchVel;

    Vector3 _manualEuler; bool _hasManual;

    float _prevGoalYaw; bool _hasPrevGoal;
    float _prevMoveYaw; bool _hasPrevMoveYaw;

    bool _realignActive;
    float _realignT;
    float _realignStartYaw;
    float _realignStartPitch;
    float _realignGoalYaw;
    float _realignGoalPitch;
    float _cooldown;

    readonly Dictionary<Renderer, OccluderState> _tracked = new();
    static readonly List<Renderer> _tmpToRestore = new();
    static readonly List<Renderer> _tmpClear = new();

    void Reset()
    {
        PositionOffset = new Vector3(0, 1.6f, 0);
        RotationOffsetEuler = Vector3.zero;

        YawFollowGain = 0.6f;
        PitchFollowGain = 0.4f;
        RollFollowGain = 0f;
        YawDeadZone = 3f;
        PitchDeadZone = 2f;
        MaxYawError = 35f;
        MaxPitchError = 20f;
        MaxYawSpeed = 240f;
        MaxPitchSpeed = 180f;
        UseWorldUp = true;

        PositionSmoothTime = 0.08f;
        LookAheadMultiplier = 0.6f;
        LookAheadMax = 2f;
        TeleportDistance = 10f;

        ManualRotationBlend = 0f;
        ManualSpace = RotationOffsetSpace.Self;

        AutoRealign = true;
        UseMovementDirection = true;
        MovementSpeedThreshold = 0.35f;
        RealignTriggerAngle = 95f;
        RealignTargetAngularSpeed = 180f;
        RealignTime = 0.35f;
        RealignCooldown = 0.5f;
        RealignPitchBlend = 0.5f;

        UseExternalVelocity = false;
        ExternalVelocity = Vector3.zero;

        AvoidCollisions = true;
        GeometryMask = ~0;
        CollisionRadius = 0.25f;
        MinDistance = 0.2f;
        CollisionBuffer = 0.05f;
        CollisionSmoothTime = 0.04f;

        SeeThrough = true;
        SeeThroughMode = OccluderMode.ShadowsOnly;
        SeeThroughRayRadius = 0.02f;
        FadeAlpha = 0.25f;
        FadeOutTime = 0.08f;
        FadeInTime = 0.15f;
        MaxOccluders = 8;
        KeepShadowsWhenHidden = true;

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
            _prevGoalYaw = _yaw; _hasPrevGoal = true;
            _prevMoveYaw = _yaw; _hasPrevMoveYaw = true;
            transform.position = targetPos + PositionOffset;
            ApplyRotation();
            return;
        }

        var delta = targetPos - _lastTargetPos;
        var vel = UseExternalVelocity ? ExternalVelocity : (delta / Mathf.Max(dt, 0.0001f));
        var lookAhead = Vector3.ClampMagnitude(vel * LookAheadMultiplier, LookAheadMax);
        var desiredPos = targetPos + PositionOffset + lookAhead;

        if (Vector3.Distance(transform.position, desiredPos) > TeleportDistance)
            transform.position = desiredPos;
        else
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _posVelocity, Mathf.Max(0.0001f, PositionSmoothTime), Mathf.Infinity, dt);

        if (AvoidCollisions) ResolveCollision(targetPos + PositionOffset, dt);

        var followRot = Target.rotation * Quaternion.Euler(RotationOffsetEuler);
        ExtractYawPitch(followRot, out float goalYaw, out float goalPitch);

        float moveYaw = _prevMoveYaw;
        float moveSpeed = 0f;
        bool hasMoveDir = false;
        var planar = Vector3.ProjectOnPlane(vel, Vector3.up);
        moveSpeed = planar.magnitude;
        if (moveSpeed > 1e-4f)
        {
            moveYaw = Mathf.Atan2(planar.x, planar.z) * Mathf.Rad2Deg;
            hasMoveDir = moveSpeed >= MovementSpeedThreshold;
        }

        if (AutoRealign)
        {
            if (_cooldown > 0f) _cooldown -= dt;

            float err = UseMovementDirection && hasMoveDir ? Mathf.Abs(Mathf.DeltaAngle(_yaw, moveYaw)) : Mathf.Abs(Mathf.DeltaAngle(_yaw, goalYaw));
            float angSpeed = 0f;
            if (UseMovementDirection && hasMoveDir)
                angSpeed = _hasPrevMoveYaw ? Mathf.Abs(Mathf.DeltaAngle(_prevMoveYaw, moveYaw)) / Mathf.Max(dt, 0.0001f) : 0f;
            else
                angSpeed = _hasPrevGoal ? Mathf.Abs(Mathf.DeltaAngle(_prevGoalYaw, goalYaw)) / Mathf.Max(dt, 0.0001f) : 0f;

            bool manual = _hasManual && ManualRotationBlend > 0.05f;

            if (!_realignActive && _cooldown <= 0f && !manual && err >= RealignTriggerAngle && angSpeed >= RealignTargetAngularSpeed)
            {
                _realignActive = true;
                _realignT = 0f;
                _realignStartYaw = _yaw;
                _realignStartPitch = _pitch;
                _realignGoalYaw = UseMovementDirection && hasMoveDir ? moveYaw : goalYaw;
                _realignGoalPitch = Mathf.LerpAngle(_pitch, goalPitch, RealignPitchBlend);
            }

            if (_realignActive)
            {
                if (manual)
                {
                    _realignActive = false;
                    _cooldown = RealignCooldown;
                }
                else
                {
                    _realignT += dt / Mathf.Max(0.0001f, RealignTime);
                    float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(_realignT));
                    _yaw = Mathf.LerpAngle(_realignStartYaw, _realignGoalYaw, t);
                    _pitch = Mathf.LerpAngle(_realignStartPitch, _realignGoalPitch, t);
                    if (t >= 1f)
                    {
                        _realignActive = false;
                        _cooldown = RealignCooldown;
                    }
                }
            }
        }

        if (!_realignActive)
        {
            float yawErr = Mathf.DeltaAngle(_yaw, goalYaw);
            float pitchErr = Mathf.DeltaAngle(_pitch, goalPitch);
            if (Mathf.Abs(yawErr) <= YawDeadZone) yawErr = 0f;
            if (Mathf.Abs(pitchErr) <= PitchDeadZone) pitchErr = 0f;
            yawErr = Mathf.Clamp(yawErr, -MaxYawError, MaxYawError);
            pitchErr = Mathf.Clamp(pitchErr, -MaxPitchError, MaxPitchError);

            float targetYawCam = _yaw + yawErr * YawFollowGain;
            float targetPitchCam = _pitch + pitchErr * PitchFollowGain;

            _yaw   = SmoothDampAngleClamped(_yaw,   targetYawCam,   ref _yawVel,   0.10f, MaxYawSpeed,   dt);
            _pitch = SmoothDampAngleClamped(_pitch, targetPitchCam, ref _pitchVel, 0.10f, MaxPitchSpeed, dt);
        }

        ApplyRotation();

        _lastTargetPos = targetPos;
        _prevGoalYaw = goalYaw; _hasPrevGoal = true;
        if (hasMoveDir) { _prevMoveYaw = moveYaw; _hasPrevMoveYaw = true; }

        if (SeeThrough) ResolveSeeThrough(targetPos + PositionOffset, transform.position);
        else if (_tracked.Count > 0) { foreach (var kv in _tracked) kv.Value.RequestVisible(FadeInTime); UpdateOccluders(Time.deltaTime); }
    }

    void ApplyRotation()
    {
        var stabilized = ComposeFromYawPitch(_yaw, _pitch, UseWorldUp ? Vector3.up : Target.up);
        var finalRot = ApplyManual(stabilized);
        transform.rotation = finalRot;
    }

    void ResolveCollision(Vector3 pivot, float dt)
    {
        var desired = transform.position;
        var dir = desired - pivot;
        var dist = dir.magnitude;
        if (dist < 1e-4f) return;
        dir /= dist;

        if (Physics.SphereCast(pivot, CollisionRadius, dir, out var hit, dist, GeometryMask, QueryTriggerInteraction.Ignore))
        {
            float d = Mathf.Clamp(hit.distance - CollisionBuffer, MinDistance, dist);
            var target = pivot + dir * d;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _colVelocity, Mathf.Max(0.0001f, CollisionSmoothTime), Mathf.Infinity, dt);
        }
    }

    void ResolveSeeThrough(Vector3 eye, Vector3 cam)
    {
        var dir = cam - eye;
        float dist = dir.magnitude;
        if (dist < 1e-3f) { UpdateOccluders(Time.deltaTime); return; }
        dir /= dist;

        var hits = Physics.SphereCastAll(eye, SeeThroughRayRadius, dir, dist, GeometryMask, QueryTriggerInteraction.Ignore);
        int added = 0;
        var current = HashSetPool.Get();
        foreach (var h in hits)
        {
            var r = h.collider.GetComponent<Renderer>() ?? h.collider.GetComponentInParent<Renderer>();
            if (!r) continue;
            if (current.Add(r))
            {
                EnsureTracked(r).RequestHidden(SeeThroughMode, FadeAlpha, FadeOutTime, KeepShadowsWhenHidden);
                if (++added >= MaxOccluders) break;
            }
        }

        if (_tracked.Count > 0)
        {
            _tmpToRestore.Clear();
            foreach (var r in _tracked.Keys) if (!current.Contains(r)) _tmpToRestore.Add(r);
            foreach (var r in _tmpToRestore) _tracked[r].RequestVisible(FadeInTime);
        }

        HashSetPool.Release(current);
        UpdateOccluders(Time.deltaTime);
    }

    void UpdateOccluders(float dt)
    {
        _tmpClear.Clear();
        foreach (var kv in _tracked)
        {
            kv.Value.Update(dt);
            if (kv.Value.IsIdleAndVisible) _tmpClear.Add(kv.Key);
        }
        foreach (var r in _tmpClear) _tracked.Remove(r);
    }

    OccluderState EnsureTracked(Renderer r)
    {
        if (_tracked.TryGetValue(r, out var s)) return s;
        s = new OccluderState(r);
        _tracked.Add(r, s);
        return s;
    }

    Quaternion ApplyManual(Quaternion followRot)
    {
        if (!_hasManual || ManualRotationBlend <= 0f) return followRot;
        var manualWorld = ManualSpace == RotationOffsetSpace.Self ? (followRot * Quaternion.Euler(_manualEuler)) : Quaternion.Euler(_manualEuler);
        if (ManualRotationBlend >= 1f) return manualWorld;
        return Quaternion.Slerp(followRot, manualWorld, ManualRotationBlend);
    }

    public void SetManualEuler(Vector3 eulerDegrees) { _manualEuler = NormalizeEuler(eulerDegrees); _hasManual = true; }
    public void AddYaw(float d)   { _manualEuler.y = WrapAngle(_manualEuler.y + d); _hasManual = true; }
    public void AddPitch(float d) { _manualEuler.x = Mathf.Clamp(WrapAngle(_manualEuler.x + d), -89.9f, 89.9f); _hasManual = true; }
    public void AddRoll(float d)  { _manualEuler.z = WrapAngle(_manualEuler.z + d); _hasManual = true; }
    public void ClearManualRotation() { _manualEuler = Vector3.zero; _hasManual = false; }

    public void SetExternalMoveVelocity(Vector3 v) { ExternalVelocity = v; }
    public void ForceRealignToMove()
    {
        if (!Target) return;
        var v = UseExternalVelocity ? ExternalVelocity : ((Target.position - _lastTargetPos) / Mathf.Max(Time.deltaTime, 0.0001f));
        var planar = Vector3.ProjectOnPlane(v, Vector3.up);
        if (planar.sqrMagnitude < MovementSpeedThreshold * MovementSpeedThreshold) return;
        float moveYaw = Mathf.Atan2(planar.x, planar.z) * Mathf.Rad2Deg;

        var followRot = Target.rotation * Quaternion.Euler(RotationOffsetEuler);
        ExtractYawPitch(followRot, out float goalYaw, out float goalPitch);

        _realignActive = true;
        _realignT = 0f;
        _realignStartYaw = _yaw;
        _realignStartPitch = _pitch;
        _realignGoalYaw = moveYaw;
        _realignGoalPitch = Mathf.LerpAngle(_pitch, goalPitch, RealignPitchBlend);
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

    static Vector3 NormalizeEuler(Vector3 e) => new Vector3(WrapAngle(e.x), WrapAngle(e.y), WrapAngle(e.z));
    static float WrapAngle(float a){ a %= 360f; if (a > 180f) a -= 360f; if (a < -180f) a += 360f; return a; }

    public enum UpdateMode { Update, FixedUpdate, LateUpdate }
    public enum OccluderMode { Hide, ShadowsOnly, Fade }

    class HashSetPool
    {
        static readonly Stack<HashSet<Renderer>> pool = new();
        public static HashSet<Renderer> Get() => pool.Count > 0 ? pool.Pop() : new HashSet<Renderer>();
        public static void Release(HashSet<Renderer> s) { s.Clear(); pool.Push(s); }
    }

    class OccluderState
    {
        public readonly Renderer R;
        Material[] _mats;
        Color[] _origColors;
        int[] _colorPropId;
        ShadowCastingMode _origShadowMode;
        bool _origForceOff;

        float _targetAlpha = 1f;
        float _currentAlpha = 1f;
        float _fadeSpeed;
        OccluderMode _mode;
        bool _requestedHidden;
        bool _keepShadows;

        public bool IsIdleAndVisible => !_requestedHidden && Mathf.Approximately(_currentAlpha, 1f) && _mode != OccluderMode.Hide && _mode != OccluderMode.ShadowsOnly;

        public OccluderState(Renderer r)
        {
            R = r;
            _origShadowMode = r.shadowCastingMode;
            _origForceOff = r.forceRenderingOff;
        }

        public void RequestHidden(OccluderMode mode, float fadeAlpha, float fadeOutTime, bool keepShadows)
        {
            _mode = mode;
            _requestedHidden = true;
            _keepShadows = keepShadows;

            switch (mode)
            {
                case OccluderMode.Hide:
                    R.forceRenderingOff = true;
                    if (keepShadows) R.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    break;
                case OccluderMode.ShadowsOnly:
                    R.forceRenderingOff = false;
                    R.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    break;
                case OccluderMode.Fade:
                    EnsureFadeSetup();
                    _targetAlpha = Mathf.Clamp01(fadeAlpha);
                    _fadeSpeed = (_currentAlpha - _targetAlpha) / Mathf.Max(fadeOutTime, 0.0001f);
                    break;
            }
        }

        public void RequestVisible(float fadeInTime)
        {
            _requestedHidden = false;
            switch (_mode)
            {
                case OccluderMode.Hide:
                    R.forceRenderingOff = _origForceOff;
                    R.shadowCastingMode = _origShadowMode;
                    break;
                case OccluderMode.ShadowsOnly:
                    R.shadowCastingMode = _origShadowMode;
                    break;
                case OccluderMode.Fade:
                    _targetAlpha = 1f;
                    _fadeSpeed = (_currentAlpha - _targetAlpha) / Mathf.Max(fadeInTime, 0.0001f);
                    break;
            }
        }

        public void Update(float dt)
        {
            if (_mode == OccluderMode.Fade && _mats != null)
            {
                float dir = _currentAlpha > _targetAlpha ? 1f : -1f;
                _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, Mathf.Abs(_fadeSpeed) * dt);
                for (int i = 0; i < _mats.Length; i++)
                {
                    int id = _colorPropId[i];
                    if (id < 0) continue;
                    var c = _mats[i].GetColor(id);
                    c.a = _currentAlpha;
                    _mats[i].SetColor(id, c);
                }
            }
        }

        void EnsureFadeSetup()
        {
            if (_mats != null) return;
            _mats = R.materials;
            _origColors = new Color[_mats.Length];
            _colorPropId = new int[_mats.Length];

            for (int i = 0; i < _mats.Length; i++)
            {
                var m = _mats[i];
                int id = m.HasProperty("_BaseColor") ? Shader.PropertyToID("_BaseColor") : m.HasProperty("_Color") ? Shader.PropertyToID("_Color") : -1;
                _colorPropId[i] = id;
                _origColors[i] = id >= 0 ? m.GetColor(id) : Color.white;
            }

            _currentAlpha = 1f;
            _targetAlpha = 1f;
        }
    }
}
