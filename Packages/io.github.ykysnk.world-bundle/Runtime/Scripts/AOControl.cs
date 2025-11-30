using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Ambient Occlusion Control")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public class AOControl : BasicUdonSharpBehaviour
    {
        private const string IntensityKey = "intensity";
        [SerializeField] private AOMode mode = AOMode.ScalableAmbientObscurance;

        [SerializeField] private Image msvoButtonImage;
        [SerializeField] private Image saoButtonImage;
        [SerializeField] private Image offButtonImage;
        [SerializeField] private Color onButtonColor;
        [SerializeField] private Color offButtonColor;
        [SerializeField] private Slider intensitySlider;
        [SerializeField] [Range(0f, 1f)] private float intensity = 1f;

        private void Start() => Switch(false);

        private void Switch(bool isChanged)
        {
            if (Utilities.IsValid(msvoButtonImage))
                msvoButtonImage.color =
                    mode == AOMode.MultiScaleVolumetricObscurance ? onButtonColor : offButtonColor;
            if (Utilities.IsValid(saoButtonImage))
                saoButtonImage.color = mode == AOMode.ScalableAmbientObscurance ? onButtonColor : offButtonColor;
            if (Utilities.IsValid(offButtonImage))
                offButtonImage.color = mode == AOMode.Off ? onButtonColor : offButtonColor;
            if (Utilities.IsValid(intensitySlider) && isChanged)
                intensitySlider.value = intensity;

            if (Utilities.IsValid(postProcessVolumeOfMSVO))
            {
                postProcessVolumeOfMSVO.gameObject.SetActive(mode == AOMode.MultiScaleVolumetricObscurance);
                postProcessVolumeOfMSVO.weight = intensity;
            }

            if (!Utilities.IsValid(postProcessVolumeOfSAO)) return;
            postProcessVolumeOfSAO.gameObject.SetActive(mode == AOMode.ScalableAmbientObscurance);
            postProcessVolumeOfSAO.weight = intensity;
        }

        protected override void OnChange() => Switch(false);

        private void OnSwitchMode(AOMode newMode)
        {
            mode = newMode;
            Switch(false);
            Save();
        }

        public void OnClickMSVO() => OnSwitchMode(AOMode.MultiScaleVolumetricObscurance);
        public void OnClickSAO() => OnSwitchMode(AOMode.ScalableAmbientObscurance);
        public void OnClickOff() => OnSwitchMode(AOMode.Off);

        public void OnValueChanged()
        {
            intensity = intensitySlider.value;
            Switch(true);
            Save();
        }

        public override void OnPlayerRestored(VRCPlayerApi player) => Load(player);
        public override void OnPlayerLeft(VRCPlayerApi player) => Save(player);

        protected override void Load(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;

            GetOwner();

            if (PlayerData.TryGetInt(player, SaveMode(), out var savedMode))
            {
                mode = (AOMode)savedMode;
                Log($"Loaded from save: {SaveMode()}, {savedMode}");
            }

            if (PlayerData.TryGetFloat(player, SaveKey(IntensityKey), out var savedIntensity))
            {
                intensity = savedIntensity;
                Log($"Loaded from save: {SaveKey(IntensityKey)}, {savedIntensity}");
            }

            Switch(false);
        }

        protected override void Save(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            PlayerData.SetInt(SaveMode(), (int)mode);
            PlayerData.SetFloat(SaveKey(IntensityKey), intensity);
        }

        // ReSharper disable InconsistentNaming
        [SerializeField] private PostProcessVolume postProcessVolumeOfMSVO;

        [SerializeField] private PostProcessVolume postProcessVolumeOfSAO;
        // ReSharper restore InconsistentNaming
    }

    public enum AOMode
    {
        MultiScaleVolumetricObscurance,
        ScalableAmbientObscurance,
        Off
    }
}