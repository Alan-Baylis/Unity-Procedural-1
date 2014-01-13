using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportPackage : MonoBehaviour {

    [MenuItem("Export/Create Camera and Input System")]
    static void ExportCameraAndInputSystem()
    {
        string packageName = "RtsCameraAndInput.unitypackage";

        AssetDatabase.ExportPackage("Assets/RtsCamera", "ExportedPackages/Camera And Input System/" + packageName, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
        Debug.Log(string.Format("Export {0} success in: {1}", packageName, System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ExportedPackages/Camera And Input System/" + packageName)));
    }

    [MenuItem("Export/Export Full RTS System")]
    static void ExportRtsSystem()
    {
        string[] paths = {
                             "Assets/RtsCamera",
                             "Assets/RtsGame"
                         };

        string packageName = "RtsSystem.unitypackage";

        AssetDatabase.ExportPackage(paths, "ExportedPackages/Camera And Input System/" + packageName, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
        Debug.Log(string.Format("Export {0} success in: {1}", packageName, System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ExportedPackages/Camera And Input System/" + packageName)));
    }
}
