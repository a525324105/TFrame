using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class EditorConfigWindow : OdinMenuEditorWindow
{
    [SerializeField]
    private SomeData someData = new SomeData(); 


    [Button("@\"TimeLabel: \" + DateTime.Now.ToString(\"HH:mm:ss\")")]
    public void TimeLabel()
    {
        
    }

    [HorizontalGroup("Split", 0.5f)]
    [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
    private void BuildConfig()
    {
        EditorConfigUtils.RunBuldBat();
    }

    [VerticalGroup("Split/right")]
    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    private void CleanConfig()
    {
        
    }

    //[MenuItem("Assets/TFrame/EditorConfigWindow", false, 1)]
    [MenuItem("TFrame/EditorConfigWindow", false, 1)]
    private static void OpenWindow()
    {
        var window = GetWindow<EditorConfigWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
        {
            { "Home",                           this,                           EditorIcons.House                       }, // Draws the this.someData field in this case.
            { "Odin Settings",                  null,                           EditorIcons.SettingsCog                 },
            { "Odin Settings/Color Palettes",   ColorPaletteManager.Instance,   EditorIcons.EyeDropper                  },
            { "Odin Settings/AOT Generation",   AOTGenerationConfig.Instance,   EditorIcons.SmartPhone                  },
            { "Player Settings",                Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault()       },
            { "Some Class",                     this.someData                                                           }
        };

        tree.AddAllAssetsAtPath("Odin Settings/More Odin Settings", "Plugins/Sirenix", typeof(ScriptableObject), true)
            .AddThumbnailIcons();

        tree.AddAssetAtPath("Odin Getting Started", "Plugins/Sirenix/Getting Started With Odin.asset");

        tree.MenuItems.Insert(2, new OdinMenuItem(tree, "Menu Style", tree.DefaultMenuStyle));

        tree.Add("Menu/Items/Are/Created/As/Needed", new GUIContent());
        tree.Add("Menu/Items/Are/Created", new GUIContent("And can be overridden"));

        tree.SortMenuItemsByName();

        return tree;
    }

    protected override void Initialize()
    {
        base.Initialize();
        OnProcess();
    }

    void OnProcess()
    {

    }
}
