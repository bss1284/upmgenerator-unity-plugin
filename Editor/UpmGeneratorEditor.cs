using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

namespace BSS.TextEditor {
    public class UpmGeneratorEditor : EditorWindow {
        private string prefix = "com";
        private string companyName = "company";
        private string projectName = "sample";

        public enum LicenseType {
            Empty,MIT
        }
        private LicenseType licenseType;

        [MenuItem("Tools/BSS/UPM Generator/Show Window")]
        public static void ShowWindow() {
            GetWindow<UpmGeneratorEditor>(false, "UPM Generator", true);
        }



        void OnGUI() {
            EditorGUILayout.Space(5f);
            EditorGUILayout.BeginVertical();
            //GUILayout.Label("Project Prefix (Recommend no changed)");
            //prefix = EditorGUILayout.TextField(prefix);

            EditorGUILayout.LabelField("Company Name");
            companyName = EditorGUILayout.TextField(companyName);
            EditorGUILayout.Space(5f);

            EditorGUILayout.LabelField("Project Name");
            projectName = EditorGUILayout.TextField(projectName);
            EditorGUILayout.Space(5f);

            EditorGUILayout.LabelField("License Type");
            licenseType =(LicenseType)EditorGUILayout.EnumPopup(licenseType);
            EditorGUILayout.Space(5f);


            if (GUILayout.Button("Generate")) {
                CreateUpmProject();
            }

            EditorGUILayout.BeginVertical();
        }

        private void CreateUpmProject() {
            string path = Application.dataPath;
            string projectPath = path + "/" + "upm-"+projectName;
            if (Directory.Exists(projectPath)){
                EditorUtility.DisplayDialog("Error", "Directory exists.", "OK");
                return;
            }
            Directory.CreateDirectory(projectPath);
            Directory.CreateDirectory(projectPath + "/Editor");
            Directory.CreateDirectory(projectPath + "/Runtime");
            var pakageFile = File.CreateText(projectPath + "/package.json");
            pakageFile.Write(GetPackageText());
            pakageFile.Close();
            File.CreateText(projectPath + "/README.md");
            File.CreateText(projectPath + "/CHANGELOG.md");
            var licenseFile=File.CreateText(projectPath + "/LICENSE.md");
            if (licenseType==LicenseType.MIT) {
                licenseFile.Write(GetMitText());
                licenseFile.Close();
            }

            string asmName= companyName + "." + projectName;
            var editorAsm=File.CreateText(projectPath + "/Editor/"+ asmName+ ".Editor.asmdef");
            editorAsm.Write($"{{\n    \"name\": \"{asmName}.Editor\"\n}}");
            editorAsm.Close();
            var runtimeAsm = File.CreateText(projectPath + "/Runtime/" + asmName+ ".asmdef");
            runtimeAsm.Write($"{{\n    \"name\": \"{asmName}\"\n}}");
            runtimeAsm.Close();


            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }



        private string GetMitText() {
            return $"MIT License\n\nCopyright(c) {DateTime.Now.Year.ToString()} {companyName}\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/ or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice(including the next paragraph) shall be included in all copies or substantial portions of the Software.";
        }
        private string GetPackageText() {
            return $"{{\n    \"name\": \"{prefix + "." + companyName.ToLower() + "." + projectName.ToLower()}\",\n    \"version\": \"1.0.0\",\n    \"displayName\": \"\",\n    \"desciption\": \"\",\n    \"unity\": \"\",\n    \"dependencies\": {{}} \n}}";

        }

    }


}

