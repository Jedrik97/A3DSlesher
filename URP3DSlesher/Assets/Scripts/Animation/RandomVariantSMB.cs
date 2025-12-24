using UnityEngine;

public sealed class RandomVariantSMB : StateMachineBehaviour
{
    [Header("Variant Param")]
    [SerializeField] private string variantParam = "HitVariant";

    [Header("Range")]
    [SerializeField] private int minInclusive = 0;
    [SerializeField] private int maxExclusive = 3;

    private int _hash;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_hash == 0)
            _hash = Animator.StringToHash(variantParam);

        animator.SetInteger(_hash, Random.Range(minInclusive, maxExclusive));
    }
}