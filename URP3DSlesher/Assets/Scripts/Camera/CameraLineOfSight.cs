using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraLineOfSight : MonoBehaviour
{
    [Header("Target")]
    public Transform Target;
    public Vector3 EyeOffset = new Vector3(0, 1.6f, 0);

    [Header("Layers")]
    public LayerMask GeometryMask = ~0;      

    [Header("Collision Avoidance")]
    public bool AvoidCollisions = true;
    [Tooltip("Sphere radius used to keep camera off walls")]
    public float CollisionRadius = 0.25f;
    [Tooltip("Minimum allowed distance from eye to camera")]
    public float MinDistance = 0.2f;
    [Tooltip("Small offset from hit surface")]
    public float CollisionBuffer = 0.05f;
    [Tooltip("Smoothing for camera reposition")]
    public float PositionSmoothTime = 0.04f;

    [Header("See-Through (optional)")]
    public bool SeeThrough = true;
    public OccluderMode Mode = OccluderMode.ShadowsOnly; 
    [Tooltip("Thickness of visibility ray (spherecast)")]
    public float SeeThroughRayRadius = 0.02f;
    [Tooltip("Fade alpha (if Mode == Fade)")]
    [Range(0.02f, 1f)] public float FadeAlpha = 0.25f;
    [Tooltip("Fade-out time (to transparent)")]
    public float FadeOutTime = 0.08f;
    [Tooltip("Fade-in time (restore)")]
    public float FadeInTime = 0.15f;
    [Tooltip("Max occluders processed per frame")]
    public int MaxOccluders = 8;

    [Header("Runtime")]
    [Tooltip("For Mode=Hide, keep shadows using ShadowsOnly")]
    public bool KeepShadowsWhenHidden = true;

    public enum OccluderMode { Hide, ShadowsOnly, Fade }

    Vector3 _posVelocity;
    readonly Dictionary<Renderer, OccluderState> _tracked = new();

    void LateUpdate()
    {
        if (!Target) return;

        var pivot = Target.position + EyeOffset;
        var desired = transform.position; 

        
        if (AvoidCollisions)
        {
            var dir = desired - pivot;
            var dist = dir.magnitude;
            if (dist > 1e-4f)
            {
                dir /= dist;
                if (Physics.SphereCast(pivot, CollisionRadius, dir, out var hit, dist, GeometryMask, QueryTriggerInteraction.Ignore))
                {
                    float d = Mathf.Clamp(hit.distance - CollisionBuffer, MinDistance, dist);
                    desired = pivot + dir * d;
                }
                
                transform.position = Vector3.SmoothDamp(transform.position, desired, ref _posVelocity, PositionSmoothTime, Mathf.Infinity, Time.deltaTime);
            }
        }
        
        if (SeeThrough)
        {
            ResolveSeeThrough(pivot, transform.position);
        }
        else if (_tracked.Count > 0)
        {
          
            foreach (var kv in _tracked) kv.Value.RequestVisible();
            UpdateOccluders();
        }
    }

    void ResolveSeeThrough(Vector3 eye, Vector3 cam)
    {
        var dir = cam - eye;
        float dist = dir.magnitude;
        if (dist < 1e-3f)
        {
            UpdateOccluders(); 
            return;
        }
        dir /= dist;

        var hits = Physics.SphereCastAll(eye, SeeThroughRayRadius, dir, dist, GeometryMask, QueryTriggerInteraction.Ignore);
        int added = 0;
        HashSet<Renderer> current = HashSetPool.Get();
        foreach (var h in hits)
        {
            var r = h.collider.GetComponent<Renderer>() ?? h.collider.GetComponentInParent<Renderer>();
            if (!r) continue;
            if (current.Add(r))
            {
                EnsureTracked(r).RequestHidden(Mode, FadeAlpha, FadeOutTime, KeepShadowsWhenHidden);
                if (++added >= MaxOccluders) break;
            }
        }
        
        if (_tracked.Count > 0)
        {
            _tmpToRestore.Clear();
            foreach (var r in _tracked.Keys) if (!current.Contains(r)) _tmpToRestore.Add(r);
            foreach (var r in _tmpToRestore) _tracked[r].RequestVisible();
        }

        HashSetPool.Release(current);
        
        UpdateOccluders();
    }

    void UpdateOccluders()
    {
        _tmpClear.Clear();
        foreach (var kv in _tracked)
        {
            var st = kv.Value;
            st.Update(Time.deltaTime);
            if (st.IsIdleAndVisible) _tmpClear.Add(kv.Key);
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

  

    static readonly List<Renderer> _tmpToRestore = new();
    static readonly List<Renderer> _tmpClear = new();

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
        float _fadeSpeed = 0f;      
        OccluderMode _mode;
        bool _requestedHidden = false;
        bool _keepShadows = true;

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

        public void RequestVisible()
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
                    _fadeSpeed = (_currentAlpha - _targetAlpha) / Mathf.Max(0.0001f, 0.0001f); 
                    break;
            }
        }

        public void Update(float dt)
        {
            if (_mode == OccluderMode.Fade && _mats != null)
            {
                float speed = _currentAlpha > _targetAlpha ? _fadeSpeed : (_fadeSpeed != 0 ? Mathf.Abs(_fadeSpeed) : 1f / 0.15f);
                _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, Mathf.Abs(speed) * dt);

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
                int id = m.HasProperty("_BaseColor") ? Shader.PropertyToID("_BaseColor")
                      : m.HasProperty("_Color")      ? Shader.PropertyToID("_Color")
                      : -1;

                _colorPropId[i] = id;
                _origColors[i] = id >= 0 ? m.GetColor(id) : Color.white;

              
            }

          
            _currentAlpha = 1f;
            _targetAlpha = 1f;
        }
    }
}
