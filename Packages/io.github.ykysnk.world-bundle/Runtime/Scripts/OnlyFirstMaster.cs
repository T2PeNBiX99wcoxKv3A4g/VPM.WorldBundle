using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [DisallowMultipleComponent]
    [AddComponentMenu("yky/World Bundle/Only First Master")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OnlyFirstMaster : BasicUdonSharpBehaviour
    {
        [SerializeField] private float delaySeconds = 2f;

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            SendCustomEventDelayedSeconds(nameof(SetObject), delaySeconds);
        }

        public void SetObject()
        {
            if (IsFirstMaster(Networking.LocalPlayer)) return;
            gameObject.SetActive(false);
        }
    }
}