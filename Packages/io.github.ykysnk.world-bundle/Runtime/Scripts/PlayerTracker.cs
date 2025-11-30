using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Player Tracker")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTracker : BasicUdonSharpBehaviour
    {
        [SerializeField] private VRCPlayerApi.TrackingDataType trackingDataType;

        private void Start() => StartFreamRateLoop();

        protected override void FreamRateLoop()
        {
            var player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player)) return;
            var data = player.GetTrackingData(trackingDataType);
            transform.SetPositionAndRotation(data.position, data.rotation);
        }
    }
}