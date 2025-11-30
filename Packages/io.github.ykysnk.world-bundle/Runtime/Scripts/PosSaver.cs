using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace io.github.ykysnk.WorldBundle
{
    [AddComponentMenu("yky/World Bundle/Pos Saver")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PosSaver : BasicUdonSharpBehaviour
    {
        private const string NowPos = "now_pos";
        private const string NowRot = "now_rot";

        public override void OnPlayerRestored(VRCPlayerApi player) => Load(player);

        protected override void Save(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            PlayerData.SetVector3(NowPos, player.GetPosition());
            PlayerData.SetQuaternion(NowRot, player.GetRotation());
        }

        protected override void Load(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal ||
                !PlayerData.TryGetVector3(player, NowPos, out var nowPos) ||
                !PlayerData.TryGetQuaternion(player, NowRot, out var nowRot))
                return;
            player.TeleportTo(nowPos, nowRot);
            Log($"Loaded from save: Teleport to position: {nowPos} rotation: {nowRot}");
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args) => Save();
        public override void InputLookVertical(float value, UdonInputEventArgs args) => Save();
        public override void InputMoveHorizontal(float value, UdonInputEventArgs args) => Save();
        public override void InputMoveVertical(float value, UdonInputEventArgs args) => Save();
        public override void OnPlayerLeft(VRCPlayerApi player) => Save(player);
    }
}