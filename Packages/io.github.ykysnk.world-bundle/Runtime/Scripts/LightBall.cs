using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Light Ball")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LightBall : BasicUdonSharpBehaviour
    {
        [SerializeField] private float maxIntensity = 5f;
        [SerializeField] private new Light light;
        [SerializeField] private Rigidbody rigidBody;

        private void Start() => StartFreamRateLoop();

        protected override void OnChange()
        {
            light = GetComponentInChildren<Light>();
            rigidBody = GetComponent<Rigidbody>();
        }

        protected override void FreamRateLoop()
        {
            var player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player)) return;
            var vel = rigidBody.velocity;
            var speed = Mathf.Clamp(vel.magnitude, 0f, maxIntensity);

            light.intensity = speed;
        }
    }
}