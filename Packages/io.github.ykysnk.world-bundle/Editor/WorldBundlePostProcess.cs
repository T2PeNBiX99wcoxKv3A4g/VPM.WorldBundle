using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBundle.Editor;

public static class WorldBundlePostProcess
{
    [PostProcessScene(-100)]
    public static void ScenePostProcess()
    {
        SetStatusText();
    }

    private static void SetStatusText()
    {
        var statusTexts = Object.FindObjectsOfType<StatusText>(true);
        var tempSyncStatusTexts = new Dictionary<StatusText, List<StatusText>>();

        foreach (var statusText in statusTexts)
        {
            if (!Utilities.IsValid(statusText.syncStatusText)) continue;
            var syncStatusText = statusText.syncStatusText;
            if (!tempSyncStatusTexts.ContainsKey(syncStatusText))
                tempSyncStatusTexts.Add(syncStatusText, new());
            tempSyncStatusTexts[syncStatusText].Add(statusText);
        }

        foreach (var statusTextPair in tempSyncStatusTexts)
            statusTextPair.Key.needSyncStatusTexts = statusTextPair.Value.ToArray();
    }
}