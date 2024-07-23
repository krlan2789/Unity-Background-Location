using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR && UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor;
#endif
using UnityEngine;
using System.IO;

namespace LAN.LiveLocation.Postprocess
{
    public class LiveLocationXcodePostprocess : MonoBehaviour
    {
#if UNITY_EDITOR && UNITY_IOS
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.iOS)
            {
                string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                PlistElementDict rootDict = plist.root;
                PlistElementArray bgModes = rootDict.CreateArray("UIBackgroundModes");
                bgModes.AddString("location");
                bgModes.AddString("fetch");
                bgModes.AddString("processing");

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
#endif
    }
}
