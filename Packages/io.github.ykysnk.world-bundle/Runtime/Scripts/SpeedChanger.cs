using io.github.ykysnk.utils.Extensions;
using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

// ReSharper disable ArrangeObjectCreationWhenTypeEvident UseIndexFromEndExpression

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Speed Changer")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public class SpeedChanger : BasicUdonSharpBehaviour
    {
        [SerializeField] private VRCWorldSettings vrcWorldSettings;
        [SerializeField] private float delaySeconds = 20f;
        private readonly DataDictionary _changedGravityStrength = new DataDictionary();

        private readonly DataDictionary _changedJumpImpulse = new DataDictionary();

        private readonly DataList _changedObjects = new DataList();
        private readonly DataDictionary _changedRunSpeed = new DataDictionary();
        private readonly DataDictionary _changedStrafeSpeed = new DataDictionary();
        private readonly DataDictionary _changedWalkSpeed = new DataDictionary();
        private float _oldGravityStrengthFirst = -1;
        private float _oldJumpImpulseFirst = -1;
        private float _oldRunSpeedFirst = -1;
        private float _oldStrafeSpeedFirst = -1;
        private float _oldWalkSpeedFirst = -1;

        private VRCPlayerApi _player;

        private void Start()
        {
            _player = Networking.LocalPlayer;

            if (!Utilities.IsValid(_player))
            {
                LogError("Local Player is inValid. Aborting.");
                enabled = false;
                return;
            }

            Send();
        }

        private void Send()
        {
            SendCustomEventDelayedSeconds(nameof(CheckPlayerJumpImpulse), delaySeconds);
        }

        private void SaveOldValues(VRCPlayerApi player)
        {
            _oldJumpImpulseFirst = player.GetJumpImpulse();
            _oldRunSpeedFirst = player.GetRunSpeed();
            _oldWalkSpeedFirst = player.GetWalkSpeed();
            _oldStrafeSpeedFirst = player.GetStrafeSpeed();
            _oldGravityStrengthFirst = player.GetGravityStrength();
        }

        public void SetJumpImpulse(VRCPlayerApi player, GameObject obj, float impulse = 3F)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player)) return;
            if (_changedObjects.Count < 1)
                SaveOldValues(player);
            _changedJumpImpulse.SetValue(obj, impulse);
            player.SetJumpImpulse(impulse);
            if (_changedObjects.Contains(obj)) return;
            _changedObjects.Add(obj);
#if UNITY_EDITOR
            Log(
                $"Player start change speed. Game Object: ({obj.FullName()}), Changed Objects: ({_changedObjects.Count})");
#endif
        }

        public void SetRunSpeed(VRCPlayerApi player, GameObject obj, float speed = 4F)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player)) return;
            if (_changedObjects.Count < 1)
                SaveOldValues(player);
            _changedRunSpeed.SetValue(obj, speed);
            player.SetRunSpeed(speed);
            if (_changedObjects.Contains(obj)) return;
            _changedObjects.Add(obj);
#if UNITY_EDITOR
            Log(
                $"Player start change speed. Game Object: ({obj.FullName()}), Changed Objects: ({_changedObjects.Count})");
#endif
        }

        public void SetWalkSpeed(VRCPlayerApi player, GameObject obj, float speed = 2F)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player)) return;
            if (_changedObjects.Count < 1)
                SaveOldValues(player);
            _changedWalkSpeed.SetValue(obj, speed);
            player.SetWalkSpeed(speed);
            if (_changedObjects.Contains(obj)) return;
            _changedObjects.Add(obj);
#if UNITY_EDITOR
            Log(
                $"Player start change speed. Game Object: ({obj.FullName()}), Changed Objects: ({_changedObjects.Count})");
#endif
        }

        public void SetStrafeSpeed(VRCPlayerApi player, GameObject obj, float speed = 2F)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player)) return;
            if (_changedObjects.Count < 1)
                SaveOldValues(player);
            _changedStrafeSpeed.SetValue(obj, speed);
            player.SetStrafeSpeed(speed);
            if (_changedObjects.Contains(obj)) return;
            _changedObjects.Add(obj);
#if UNITY_EDITOR
            Log(
                $"Player start change speed. Game Object: ({obj.FullName()}), Changed Objects: ({_changedObjects.Count})");
#endif
        }

        public void SetGravityStrength(VRCPlayerApi player, GameObject obj, float strength = 1F)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player)) return;
            if (_changedObjects.Count < 1)
                SaveOldValues(player);
            _changedGravityStrength.SetValue(obj, strength);
            player.SetGravityStrength(strength);
            if (_changedObjects.Contains(obj)) return;
            _changedObjects.Add(obj);
#if UNITY_EDITOR
            Debug.Log(
                $"Player start change speed. Game Object: ({obj.FullName()}), Changed Objects: ({_changedObjects.Count})");
#endif
        }

        public void OnPlayerStartChangeSpeed(VRCPlayerApi player, GameObject obj)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player) || _changedObjects.Count > 0) return;
            SaveOldValues(player);
        }

        public void OnPlayerStopChangeSpeed(VRCPlayerApi player, GameObject obj)
        {
            if (!Utilities.IsValid(obj) || !Utilities.IsValid(player)) return;
            _changedObjects.Remove(obj);

            if (_changedObjects.Count < 1)
            {
                player.SetJumpImpulse(_oldJumpImpulseFirst > 0 ? _oldJumpImpulseFirst : vrcWorldSettings.jumpImpulse);
                player.SetRunSpeed(_oldRunSpeedFirst > 0 ? _oldRunSpeedFirst : vrcWorldSettings.runSpeed);
                player.SetWalkSpeed(_oldWalkSpeedFirst > 0 ? _oldWalkSpeedFirst : vrcWorldSettings.walkSpeed);
                player.SetStrafeSpeed(_oldStrafeSpeedFirst > 0 ? _oldStrafeSpeedFirst : vrcWorldSettings.strafeSpeed);
                player.SetGravityStrength(_oldGravityStrengthFirst > 0
                    ? _oldGravityStrengthFirst
                    : vrcWorldSettings.gravityStrength);
            }
            else
            {
                var lastObj = _changedObjects[_changedObjects.Count - 1];

                player.SetJumpImpulse(_changedJumpImpulse.TryGetValue(lastObj, TokenType.Float, out var jumpImpulse)
                    ? jumpImpulse.Float
                    : vrcWorldSettings.jumpImpulse);
                player.SetRunSpeed(_changedRunSpeed.TryGetValue(lastObj, TokenType.Float, out var runSpeed)
                    ? runSpeed.Float
                    : vrcWorldSettings.runSpeed);
                player.SetWalkSpeed(_changedWalkSpeed.TryGetValue(lastObj, TokenType.Float, out var walkSpeed)
                    ? walkSpeed.Float
                    : vrcWorldSettings.walkSpeed);
                player.SetStrafeSpeed(_changedStrafeSpeed.TryGetValue(lastObj, TokenType.Float, out var strafeSpeed)
                    ? strafeSpeed.Float
                    : vrcWorldSettings.strafeSpeed);
                player.SetGravityStrength(
                    _changedGravityStrength.TryGetValue(lastObj, TokenType.Float, out var gravityStrength)
                        ? gravityStrength.Float
                        : vrcWorldSettings.gravityStrength);
            }

#if UNITY_EDITOR
            Debug.Log(
                $"Player stop change speed. Game Object: ({obj.FullName()}), Changed Objects: ({_changedObjects.Count})");
#endif
        }

        public void CheckPlayerJumpImpulse()
        {
            if (_changedObjects.Count > 0 || !Utilities.IsValid(_player))
            {
                Send();
                return;
            }

            if (_player.GetJumpImpulse() <= 0)
            {
                Debug.LogWarning($"[{name}] Player jump impulse is too low. Reset player speed.");

                _player.SetJumpImpulse(vrcWorldSettings.jumpImpulse);
                _player.SetRunSpeed(vrcWorldSettings.runSpeed);
                _player.SetWalkSpeed(vrcWorldSettings.walkSpeed);
                _player.SetStrafeSpeed(vrcWorldSettings.strafeSpeed);
                _player.SetGravityStrength(vrcWorldSettings.gravityStrength);
                if (vrcWorldSettings.useLegacyLocomotion) _player.UseLegacyLocomotion();
            }

            Send();
        }
    }
}