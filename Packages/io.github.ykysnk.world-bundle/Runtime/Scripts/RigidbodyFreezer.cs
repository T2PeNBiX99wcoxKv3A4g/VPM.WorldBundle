using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using Vector3 = UnityEngine.Vector3;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Rigid Body Freezer")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RigidbodyFreezer : BasicUdonSharpBehaviour
    {
        [Header("Main Settings")] [SerializeField]
        private int distanceLimit = 60;

        [SerializeField] private float delaySeconds = 0.8f;
        [SerializeField] private Rigidbody _rigidbody;
        private Vector3 _tempAngularVelocity;

        private Vector3 _tempVelocity;

        private void Start() => Send();

        protected override void OnChange()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Send() => SendCustomEventDelayedSeconds(nameof(CheckDistance), delaySeconds);

        [PublicAPI]
        public void Copy(ref RigidbodyFreezer other)
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

            if (IsValidDistance())
            {
                _rigidbody.constraints = RigidbodyConstraints.None;
                _rigidbody.WakeUp();

                if (_tempVelocity != Vector3.zero)
                {
                    _rigidbody.AddForce(_tempVelocity, ForceMode.VelocityChange);
                    _tempVelocity = Vector3.zero;
                }

                if (_tempAngularVelocity != Vector3.zero)
                {
                    _rigidbody.AddTorque(_tempAngularVelocity, ForceMode.VelocityChange);
                    _tempAngularVelocity = Vector3.zero;
                }
            }
            else
            {
                if (_rigidbody.velocity != Vector3.zero)
                    _tempVelocity = _rigidbody.velocity;
                if (_rigidbody.angularVelocity != Vector3.zero)
                    _tempAngularVelocity = _rigidbody.angularVelocity;

                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                _rigidbody.AddForce(Vector3.zero, ForceMode.VelocityChange);
            }

            Send();
        }
    }
}