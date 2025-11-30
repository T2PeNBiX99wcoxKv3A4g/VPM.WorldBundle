using io.github.ykysnk.WorldBasic.Udon;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Master Test")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MasterTest : BasicUdonSharpBehaviour
    {
        [SerializeField] private TMP_Text firstMasterGuidText;
        [SerializeField] private TMP_Text isInstanceOwnerText;
        [SerializeField] private RectTransform contentTransform;
        [SerializeField] private GameObject playerGuidTextPrefab;
        [SerializeField] private float delaySeconds = 2f;

        public override void OnPlayerRestored(VRCPlayerApi player) =>
            SendCustomEventDelayedSeconds(nameof(UpdateText), delaySeconds);

        public override void OnPlayerLeft(VRCPlayerApi player) => UpdateText();

        public void UpdateText()
        {
            firstMasterGuidText.text = $"First Master - {playerGuid.GetFirstMasterGuid(playerGuid.RandomKey)}";
            isInstanceOwnerText.text = $"Networking.IsInstanceOwner - {Networking.IsInstanceOwner}";

            for (var i = contentTransform.transform.childCount - 1; i >= 0; i--)
                Destroy(contentTransform.transform.GetChild(i).gameObject);

            var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);

            foreach (var player in players)
            {
                var playerTextObj = Instantiate(playerGuidTextPrefab, contentTransform);
                var playerText = playerTextObj.GetComponent<TMP_Text>();

                if (!Utilities.IsValid(playerText))
                {
                    LogWarning($"({nameof(TMP_Text)}) is not found!!!");
                    return;
                }

                playerText.text = $"{player.displayName} - {playerGuid.GetPlayerGuid(player, playerGuid.RandomKey)}";
            }
        }
    }
}