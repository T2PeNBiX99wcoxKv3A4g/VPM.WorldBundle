using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Player Transform")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public class PlayerTransform : BasicUdonSharpBehaviour
    {
        private bool _isEdit;

        private void Start() => StartFreamRateLoop();

        protected override void FreamRateLoop()
        {
            var player = Networking.LocalPlayer;
            if (_isEdit || !Utilities.IsValid(player)) return;
            transform.SetPositionAndRotation(player.GetPosition(), player.GetRotation());
        }

        public void Edit()
        {
            _isEdit = true;
        }

        public void Apply(VRC_SceneDescriptor.SpawnOrientation spawnOrientation, bool lerpOnRemote)
        {
            var player = Networking.LocalPlayer;
            player.TeleportTo(transform.position, transform.rotation, spawnOrientation, lerpOnRemote);
            _isEdit = false;
        }
    }
}