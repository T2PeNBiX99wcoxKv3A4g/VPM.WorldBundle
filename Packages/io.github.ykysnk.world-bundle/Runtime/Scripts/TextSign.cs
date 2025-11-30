using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Text Sign")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [PublicAPI]
    public class TextSign : BasicUdonSharpBehaviour
    {
        private const string TextKey = "text";
        private const string PosKey = "pos";
        private const string RotKey = "rot";
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button lockButton;
        [SerializeField] [HideInInspector] private VRC_Pickup pickup;
        private bool _isLocked = true;
        private float _posSaveTime;

        [UdonSynced] private string _text;

        private void Start()
        {
            inputField.interactable = false;
            inputField.readOnly = true;
            lockButton.gameObject.SetActive(true);
        }

        protected override void OnChange()
        {
            pickup = GetComponent<VRC_Pickup>();
            inputField = GetComponentInChildren<TMP_InputField>();
            lockButton = GetComponentInChildren<Button>();
        }

        public void OnValueChanged()
        {
            _text = inputField.text;
            _isLocked = true;
            Synchronize();
            Save();
            Log($"Changed text in local, {_text}");
        }

        public void OnEndEdit()
        {
            _isLocked = true;
            Save();
        }

        public void OnDeselect()
        {
            _isLocked = true;
            Save();
        }

        public void OnClickLock()
        {
            _isLocked = false;
            Synchronize();
            Save();
        }

        public override void OnDrop()
        {
            _isLocked = false;
            Save();
        }

        public override void OnPlayerRestored(VRCPlayerApi player) => Load(player);
        public override void OnPlayerLeft(VRCPlayerApi player) => Save(player);

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            if (Utilities.IsValid(pickup) && !pickup.IsHeld) return;
            Save();
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if (Utilities.IsValid(pickup) && !pickup.IsHeld) return;
            Save();
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (Utilities.IsValid(pickup) && !pickup.IsHeld) return;
            Save();
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (Utilities.IsValid(pickup) && !pickup.IsHeld) return;
            Save();
        }

        protected override void Load(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal || !IsFirstMaster(player)) return;

            GetOwner();

            if (Utilities.IsValid(inputField) && PlayerData.TryGetString(player, SaveKey(TextKey), out var savedText))
            {
                _text = savedText;
                inputField.text = _text;
                Log($"Loaded from save: {SaveKey(TextKey)}, {savedText}");
            }

            if (PlayerData.TryGetVector3(player, SaveKey(PosKey), out var savedPos))
            {
                transform.position = savedPos;

                Log($"Loaded from save: {SaveKey(PosKey)}, {savedPos}");
            }

            if (PlayerData.TryGetQuaternion(player, SaveKey(RotKey), out var savedRot))
            {
                transform.rotation = savedRot;

                Log($"Loaded from save: {SaveKey(RotKey)}, {savedRot}");
            }

            Synchronize();
        }

        protected override void Save(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal || !IsFirstMaster(player)) return;
            PlayerData.SetString(SaveKey(TextKey), _text);
            PlayerData.SetVector3(SaveKey(PosKey), transform.position);
            PlayerData.SetQuaternion(SaveKey(RotKey), transform.rotation);
        }

        protected override void AfterSynchronize(bool isOwner)
        {
            inputField.interactable = !_isLocked;
            inputField.readOnly = _isLocked;
            lockButton.gameObject.SetActive(_isLocked);
            if (isOwner) return;
            inputField.text = _text;
            Save();
            Log($"Loaded text form other user, {_text}");
        }
    }
}