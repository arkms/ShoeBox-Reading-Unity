using UnityEditor;
using UnityEngine;
using System.Xml;
using System.Collections.Generic;

public class ShoeBoxReading : EditorWindow
{
    private Object text;
    private Texture2D texture2d;
    private SpriteAlignment pivot = SpriteAlignment.Center;
    private Vector2 customPivot = new Vector2(0.5f, 0.5f);

    [MenuItem("Window/ShoeBox Import")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(ShoeBoxReading), false, "ShoeBox Imported");
    }

    void OnEnable()
    {
    }

    void OnGUI()
    {
        text = EditorGUILayout.ObjectField("File: ", text, typeof(TextAsset), false);
        texture2d = (Texture2D)EditorGUILayout.ObjectField("Texture: ", texture2d, typeof(Texture2D), false);
        pivot = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot: ", pivot);
        if (pivot == SpriteAlignment.Custom)
        {
            Vector2 customPivotTemp = EditorGUILayout.Vector2Field("Custom pivot:", customPivot);
            //user change value,, it can be more than 1 or less than 0
            if (customPivotTemp != customPivot)
            {
                if (customPivotTemp.x > 1)
                {
                    customPivotTemp.x = 1;
                }
                else if (customPivotTemp.x < 0)
                {
                    customPivotTemp.x = 0;
                }
                if (customPivotTemp.y > 1)
                {
                    customPivotTemp.y = 1;
                }
                else if (customPivotTemp.y < 0)
                {
                    customPivotTemp.y = 0;
                }
                //apply changes
                customPivot = customPivotTemp;
            }
        }

        if (GUILayout.Button("Read"))
        {
            Read();
        }
    }

    void Read()
    {
        XmlTextReader reader = new XmlTextReader(AssetDatabase.GetAssetPath(text));
        Rect rect;
        List<SpriteMetaData> listSprite = new List<SpriteMetaData>();
        SpriteMetaData spritedata;
        int imageHeight= texture2d.height;
        int spriteWidht;
        int SpriteY;
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "SubTexture")
                {
                    //got some info before
                    SpriteY = int.Parse(reader.GetAttribute(2));
                    spriteWidht= int.Parse(reader.GetAttribute(4));

                    //create rect of sprite
                    rect = new Rect(
                        int.Parse(reader.GetAttribute(1)), //x
                        imageHeight - SpriteY - spriteWidht, //y
                        int.Parse(reader.GetAttribute(3)), //width
                        spriteWidht //hegith
                        ); 
                    
                    //init spritedata
                    spritedata= new SpriteMetaData();
                    spritedata.rect= rect;
                    spritedata.name= reader.GetAttribute(0).Remove(reader.GetAttribute(0).Length - 4, 4); //we remove the extension of the name
                    spritedata.alignment = (int)pivot;
                    if (pivot == SpriteAlignment.Custom)
                    {
                        spritedata.pivot = customPivot;
                    }
                    //add to list
                    listSprite.Add(spritedata);
                }
            }
        }

        //was sucessfull?
        if (listSprite.Count > 0)
        {
            //import texture
            TextureImporter textImp = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2d)) as TextureImporter;
            //add spritesheets
            textImp.spritesheet = listSprite.ToArray();
            //configure texture
            textImp.textureType = TextureImporterType.Sprite;
            textImp.spriteImportMode = SpriteImportMode.Multiple;
            //import, for forceupdate and save it
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture2d), ImportAssetOptions.ForceUpdate);
            //Debug.Log("Done");
        }
        else
        {
            Debug.LogWarning("This is not a file of ShoeBox or is not a XML file");
        }
    }
}
