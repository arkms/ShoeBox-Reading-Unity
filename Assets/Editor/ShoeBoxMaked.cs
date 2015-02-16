using UnityEditor;
using UnityEngine;
using System.Xml;
//using System.Collections;

public class ShoeBoxMaked : EditorWindow
{
    private Texture2D texture2d;

    [MenuItem("Window/ShoeBoxReading/ShoeBox Export")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(ShoeBoxMaked), false, "ShoeBox Imported");
    }

    void OnGUI()
    {
        texture2d = (Texture2D)EditorGUILayout.ObjectField("Texture: ", texture2d, typeof(Texture2D), false);

        if (GUILayout.Button("Create") && texture2d != null)
        {
            Create();
        }
    }

    void Create()
    {
        XmlDocument doc = new XmlDocument();

        //Create element of texture
        XmlElement TextureCamp = doc.CreateElement(string.Empty, "TextureAtlas", string.Empty);
        TextureCamp.SetAttribute("imagePath", texture2d.name + ".png");
        doc.AppendChild(TextureCamp);

        //get all sprites in texture
        TextureImporter textImp = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2d)) as TextureImporter;
        SpriteMetaData[] sprites = textImp.spritesheet;

        //set information of each sprite
        for(int i=0; i<sprites.Length; i++)
        {
            XmlElement SpritesNode = doc.CreateElement(string.Empty, "SubTexture", string.Empty);
            SpritesNode.SetAttribute("name", sprites[i].name + ".png");
            SpritesNode.SetAttribute("x", sprites[i].rect.x.ToString());
            SpritesNode.SetAttribute("y", Mathf.Abs(sprites[i].rect.y + sprites[i].rect.height - texture2d.height).ToString());
            SpritesNode.SetAttribute("width", sprites[i].rect.width.ToString());
            SpritesNode.SetAttribute("height", sprites[i].rect.height.ToString());
            TextureCamp.AppendChild(SpritesNode);
        }

        //Create path to save
        string pathfull = Application.dataPath;
        pathfull= pathfull.Substring(0, pathfull.Length - 6);
        string pathobj = AssetDatabase.GetAssetPath(texture2d);
        pathobj = pathobj.Substring(0, pathobj.Length - texture2d.name.Length - 4);
        string path = pathfull + pathobj +  texture2d.name + ".xml";

        //save and refresh Editor
        doc.Save(path);
        AssetDatabase.Refresh();
    }
}
