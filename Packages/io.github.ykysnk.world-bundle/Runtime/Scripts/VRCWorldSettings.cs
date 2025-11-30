using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/VRChat World Settings")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRCWorldSettings : BasicUdonSharpBehaviour
    {
        // VRC_Player Mods
        [Header("Player Settings")] [Header("Jump")] [Tooltip("Jump power")] [SerializeField]
        internal float jumpImpulse = 3;

        [Header("Speed")] [SerializeField] internal float runSpeed = 4;
        [SerializeField] internal float walkSpeed = 2;
        [SerializeField] internal float strafeSpeed = 2;
        [SerializeField] internal float gravityStrength = 1;

        [Header("Locomotion")] [Tooltip("Use SDK2 locomotion")] [SerializeField]
        internal bool useLegacyLocomotion;

        [Space] [Header("Avatar Scaling Settings")] [SerializeField]
        private bool applyAvatarScalingLimits;

        [SerializeField] private float minimumHeight = 0.2f;
        [SerializeField] private float maximumHeight = 5f;

        [SerializeField] private bool isAlwaysEnforceHeight;

        [Space]
        // VRC_Player Audio Override
        [Header("Voice Settings")]
        [Tooltip("Apply Voice Settings")]
        [SerializeField]
        private bool applyVoiceOptions;

        [SerializeField] private float voiceGain = 15;
        [SerializeField] private float voiceDistanceFar = 25;

        // VRC_Player Audio Override Advanced Options
        [Header("Voice Settings(advanced)")] [SerializeField]
        private bool applyVoiceAdvancedOptions;

        [SerializeField] private float voiceDistanceNear;
        [SerializeField] private float voiceVolumetricRadius;
        [SerializeField] private bool voiceLowpass = true;

        // VRC_Player Audio Override Avatar Audio Limits
        [Header("Audio Limits")] [Tooltip("Apply Audio Limits")] [SerializeField]
        private bool applyAvatarAudioLimits;

        [SerializeField] private float avatarAudioGain = 10;
        [SerializeField] private float avatarAudioFarRadius = 40;

        // VRC_Player Audio Override Avatar Audio Limits Advanced Options
        [Header("Audio Limits(advanced)")] [SerializeField]
        private bool applyAvatarAudioAdvancedOptions;

        [SerializeField] private float avatarAudioNearRadius = 40;
        [SerializeField] private float avatarAudioVolumetricRadius = 40;
        [SerializeField] private bool avatarAudioForceSpatial;
        [SerializeField] private bool avatarAudioCustomCurve = true;

        private void Start()
        {
            var player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player))
            {
                LogError("Local Player is inValid. Aborting.");
                enabled = false;
                return;
            }

            // VRC_Player Mods
            player.SetJumpImpulse(jumpImpulse);
            player.SetRunSpeed(runSpeed);
            player.SetWalkSpeed(walkSpeed);
            player.SetStrafeSpeed(strafeSpeed);
            player.SetGravityStrength(gravityStrength);
            if (useLegacyLocomotion) player.UseLegacyLocomotion();

            player.SetManualAvatarScalingAllowed(!applyAvatarScalingLimits);

            if (applyAvatarScalingLimits)
                Log(
                    "Avatar scaling has been disabled. Players will be unable to choose their own avatar scale. Udon can still use SetAvatarEyeHeight.");

            player.SetAvatarEyeHeightMinimumByMeters(minimumHeight);
            player.SetAvatarEyeHeightMaximumByMeters(maximumHeight);

            Log($"Avatar eye height limits have been set to {minimumHeight} to {maximumHeight} meters.");
        }

        public override void OnAvatarEyeHeightChanged(VRCPlayerApi player, float prevEyeHeightAsMeters)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            if (!isAlwaysEnforceHeight) return;

            var currentHeight = player.GetAvatarEyeHeightAsMeters();
            var clampedHeight = Mathf.Clamp(currentHeight, minimumHeight, maximumHeight);
            if (Mathf.Approximately(clampedHeight, currentHeight)) return;

            Log(
                $"Enforcing height limit. The local player's avatar eye height has been clamped to {clampedHeight} meters.");
            player.SetAvatarEyeHeightByMeters(clampedHeight);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || player.isLocal) return;

            // VRC_Player Audio Override
            if (applyVoiceOptions)
            {
                player.SetVoiceGain(voiceGain);
                player.SetVoiceDistanceFar(voiceDistanceFar);

                // Advanced Options
                if (applyVoiceAdvancedOptions)
                {
                    player.SetVoiceDistanceNear(voiceDistanceNear);
                    player.SetVoiceVolumetricRadius(voiceVolumetricRadius);
                    player.SetVoiceLowpass(voiceLowpass);
                }
            }

            // VRC_Player Audio Override Avatar Audio Limits
            if (!applyAvatarAudioLimits) return;
            player.SetAvatarAudioGain(avatarAudioGain);
            player.SetAvatarAudioFarRadius(avatarAudioFarRadius);

            // VRC_Player Audio Override Avatar Audio Limits Advanced Options
            if (!applyAvatarAudioAdvancedOptions) return;
            player.SetAvatarAudioForceSpatial(avatarAudioForceSpatial);
            player.SetAvatarAudioNearRadius(avatarAudioNearRadius);
            player.SetAvatarAudioVolumetricRadius(avatarAudioVolumetricRadius);
            player.SetAvatarAudioCustomCurve(avatarAudioCustomCurve);
        }
    }
}