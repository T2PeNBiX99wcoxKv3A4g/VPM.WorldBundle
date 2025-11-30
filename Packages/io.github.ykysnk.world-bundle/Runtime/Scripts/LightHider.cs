using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Light Hider")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LightHider : BasicUdonSharpBehaviour
    {
        [Header("Main Settings")] [SerializeField]
        private int distanceLimit = 40;

        [SerializeField] private float delaySeconds = 0.8f;

        [SerializeField] private new Light light;

        protected override void OnChange()
        {
            light = GetComponent<Light>();
        }

        private void Send()
        {
            SendCustomEventDelayedSeconds(nameof(CheckDistance), delaySeconds);
        }

        [PublicAPI]
        public void Copy(ref LightHider other)
        {
            other.distanceLimit = distanceLimit;
            other.delaySeconds = delaySeconds;
        }

        private bool IsValidDistance()
        {
            var player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player)) return false;
            return Vector3.Distance(transform.position, player.GetPosition()) < distanceLimit;
        }

        public void CheckDistance()
        {
            var player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player))
            {
                Send();
                return;
            }

            light.enabled = IsValidDistance();
            Send();
        }
    }
}