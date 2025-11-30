using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Ladder")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Ladder : BasicUdonSharpBehaviour
    {
        [Header("Main Settings")] [SerializeField]
        private PlayerTransform playerTransform;

        [Header("Player Settings")] [Header("Jump")] [Tooltip("Jump power")] [SerializeField]
        private float jumpImpulse = 3;

        [Space] [Header("Speed")] [SerializeField]
        private float climbSpeed = 1.5f;

        [Space] [Header("Speed Changer")] [SerializeField]
        private SpeedChanger speedChanger;

        private float _inputValue;
        private bool _isInput;

        private bool _isInside;
        private VRCPlayerApi _player;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            _isInside = true;
            _player = player;
            speedChanger.SetJumpImpulse(player, gameObject, 0);
            speedChanger.SetRunSpeed(player, gameObject, 0);
            speedChanger.SetWalkSpeed(player, gameObject, 0);
            speedChanger.SetStrafeSpeed(player, gameObject, 0);
            speedChanger.SetGravityStrength(player, gameObject, 0);
        }

        public override void OnPlayerTriggerStay(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            if (_inputValue == 0f)
                player.SetVelocity(Vector3.zero);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            _isInside = false;
            speedChanger.OnPlayerStopChangeSpeed(player, gameObject);
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (!value || !_isInside || !Utilities.IsValid(_player)) return;

            var trans = playerTransform.transform;

            _player.SetVelocity(trans.forward * -jumpImpulse);
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (!_isInside || !Utilities.IsValid(_player)) return;

            var trans = playerTransform.transform;
            var speed = value * climbSpeed;

            _inputValue += value;

            _player.SetVelocity(trans.right * speed);
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (!_isInside || !Utilities.IsValid(_player)) return;

            var trans = playerTransform.transform;
            var speed = value * climbSpeed;

            _inputValue += value;

            _player.SetVelocity(trans.forward * speed);
            _player.SetVelocity(trans.up * speed);
        }
    }
}