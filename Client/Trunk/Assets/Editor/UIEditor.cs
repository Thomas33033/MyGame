using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UIEditor
{
    [MenuItem("Tools/UIEditor/检查并设置UI图集", false, 500)]
    public static void CheckAndSetUISprite()
    {
        List<string> fileList = Utils.RecursivePathGetFiles(Application.dataPath + "/_Res/Bundles/Combine/UI");
        foreach (var path in fileList)
        {
            if (path.EndsWith(".meta"))
                continue;

            string assetPath = path.Replace(Application.dataPath, "Assets");
            AssetImporter asset = AssetImporter.GetAtPath(assetPath);
            if (asset == null || !(asset is TextureImporter))
                continue;

            string atlasName = new DirectoryInfo(Path.GetDirectoryName(path)).Name;
            TextureImporter textureImporter = asset as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePackingTag = atlasName;
            textureImporter.mipmapEnabled = false;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

            textureImporter.SaveAndReimport();
        }

        AssetDatabase.Refresh();
    }
}