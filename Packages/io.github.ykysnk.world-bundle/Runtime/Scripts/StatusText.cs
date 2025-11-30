using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Status Text")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [PublicAPI]
    public class StatusText : BasicUdonSharpBehaviour
    {
        private const string StatusTextKey = "status";
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Image lockButtonImage;
        [SerializeField] private Sprite lockSprite;
        [SerializeField] private Sprite unLockSprite;
        public StatusText syncStatusText;
        [HideInInspector] public StatusText[] needSyncStatusTexts;
        private bool _isLocked = true;

        [UdonSynced] private string _statusText;

        private void Start()
        {
            SyncCheck();
            inputField.interactable = false;
            inputField.readOnly = true;
            lockButtonImage.gameObject.SetActive(false);
        }

        private void SyncCheck()
        {
            if (Utilities.IsValid(syncStatusText) && syncStatusText == this)
                syncStatusText = null;
        }

        protected override void OnChange() => SyncCheck();

        public void OnValueChanged()
        {
            if (!Utilities.IsValid(syncStatusText))
            {
                _statusText = inputField.text;
                _isLocked = true;
                foreach (var needSyncStatusText in needSyncStatusTexts)
                    needSyncStatusText.SyncValue(_statusText);
            }

            Synchronize();
            Save();
            Log($"Changed text in local, {_statusText}");
        }

        private void SyncValue(string text)
        {
            _statusText = text;
            inputField.text = text;
            Synchronize();
        }

        public void OnClickLock()
        {
            _isLocked = !_isLocked;
            Synchronize();
            Save();
        }

        public override void OnPlayerRestored(VRCPlayerApi player) => Load(player);
        public override void OnPlayerLeft(VRCPlayerApi player) => Save(player);

        protected override void Load(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            if (Utilities.IsValid(inputField) && IsFirstMaster(player))
            {
                GetOwner();

                if (Utilities.IsValid(syncStatusText) && PlayerData.TryGetString(player,
                        SaveKey(syncStatusText, StatusTextKey), out var savedSyncStatusText))
                {
                    _statusText = savedSyncStatusText;
                    inputField.text = _statusText;
                    Log(
                        $"Loaded from other status text save: {SaveKey(syncStatusText, StatusTextKey)}, {savedSyncStatusText}");
                }
                else if (PlayerData.TryGetString(player, SaveKey(StatusTextKey), out var savedStatusText))
                {
                    _statusText = savedStatusText;
                    inputField.text = _statusText;
                    Log($"Loaded from save: {SaveKey(StatusTextKey)}, {savedStatusText}");
                }
            }

            Synchronize();
        }

        protected override void Save(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal || !IsFirstMaster(player) ||
                Utilities.IsValid(syncStatusText)) return;
            PlayerData.SetString(SaveKey(StatusTextKey), _statusText);
        }

        protected override void AfterSynchronize(bool isOwner)
        {
            var player = Networking.LocalPlayer;
            var canEdit = IsFirstMaster(player) && !_isLocked && !Utilities.IsValid(syncStatusText);
            inputField.interactable = canEdit;
            inputField.readOnly = !canEdit;
            lockButtonImage.gameObject.SetActive(IsFirstMaster(player) && !Utilities.IsValid(syncStatusText));
            lockButtonImage.sprite = _isLocked ? lockSprite : unLockSprite;
            if (isOwner) return;
            inputField.text = _statusText;
            Log($"Loaded text form other user, {_statusText}");
        }
    }
}