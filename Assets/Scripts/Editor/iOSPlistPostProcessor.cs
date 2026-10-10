#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

/// <summary>
/// iOS post-process build script for Halloween Cuties Coloring.
///
/// Runs at build order 200 (last), after all other post-processors.
///
/// Responsibilities:
/// 1. Inject NSPhotoLibraryAddUsageDescription + NSPhotoLibraryUsageDescription
///    (required by iOSUtils.m for the screenshot-save feature).
///
/// 2. ACTIVELY REMOVE NSUserTrackingUsageDescription.
///    This app does NOT use AppTrackingTransparency / ATT. No code calls
///    ATTrackingManager.RequestTrackingAuthorization(). Having the key present
///    without showing the ATT dialog triggers Apple Guideline 2.1 rejection.
///    Running at order 200 ensures this removal wins even if a CocoaPod,
///    another post-processor, or the GMA PListProcessor re-added the key.
/// </summary>
public static class iOSPlistPostProcessor
{
    [PostProcessBuild(200)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        string plistPath = System.IO.Path.Combine(buildPath, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // ---------------------------------------------------------------
        // REMOVE NSUserTrackingUsageDescription
        // This app does not use ATT. The key must be absent.
        // Apple Guideline 2.1 rejects apps that declare this key but never
        // show the ATTrackingManager.RequestTrackingAuthorization dialog.
        // ---------------------------------------------------------------
        if (rootDict.values.ContainsKey("NSUserTrackingUsageDescription"))
        {
            rootDict.values.Remove("NSUserTrackingUsageDescription");
            UnityEngine.Debug.Log(
                "[iOSPlistPostProcessor] Removed NSUserTrackingUsageDescription — " +
                "this app does not use ATT.");
        }

        // ---------------------------------------------------------------
        // ADD NSPhotoLibraryAddUsageDescription
        // Required by: AlmostEngine/UltimateScreenshotCreator/Plugins/iOS/iOSUtils.m
        // APIs: UIImageWriteToSavedPhotosAlbum, PHPhotoLibrary.requestAuthorization
        // ---------------------------------------------------------------
        if (!rootDict.values.ContainsKey("NSPhotoLibraryAddUsageDescription"))
        {
            rootDict.SetString(
                "NSPhotoLibraryAddUsageDescription",
                "This app saves your colored artwork to your photo library.");
        }

        // NSPhotoLibraryUsageDescription — defensive, older iOS read access
        if (!rootDict.values.ContainsKey("NSPhotoLibraryUsageDescription"))
        {
            rootDict.SetString(
                "NSPhotoLibraryUsageDescription",
                "This app saves your colored artwork to your photo library.");
        }

        plist.WriteToFile(plistPath);

        UnityEngine.Debug.Log(
            "[iOSPlistPostProcessor] Info.plist finalised: NSUserTrackingUsageDescription absent, " +
            "photo library keys present.");
    }
}
#endif