using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("yky/World Bundle/Play Anim On Player Restored")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayAnimOnPlayerRestored : BasicUdonSharpBehaviour
    {
        private const string DefaultParameterName = "Start";
        [SerializeField] private string parameterName = DefaultParameterName;
        [SerializeField] [HideInInspector] private int animParameterHash;
        [SerializeField] [HideInInspector] private Animator animator;

        private bool _isRestored;

        private void OnEnable()
        {
            if (!_isRestored) return;
            animator.SetTrigger(animParameterHash);
        }

        protected override void OnChange()
        {
            if (string.IsNullOrEmpty(parameterName))
                parameterName = DefaultParameterName;

            animator = GetComponent<Animator>();
            animParameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            animator.SetTrigger(animParameterHash);
            _isRestored = true;
        }
    }
}