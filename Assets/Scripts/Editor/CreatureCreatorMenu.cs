using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class CreatureCreatorMenu : OdinMenuEditorWindow
{
    private CreateNewCreature createNewCreature;

    public static readonly string folderPath = "Assets/Prefabs/Creatures/0 - Creature Data";


    [MenuItem("Tools/Glyphix/CreatureCreator")]
    private static void Open()
    {
        var window = GetWindow<CreatureCreatorMenu>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        createNewCreature = new CreateNewCreature();
        tree.Add("Create New", createNewCreature);
        tree.AddAllAssetsAtPath("Creatures", folderPath, typeof(CreatureData));

        return tree;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (createNewCreature != null)
        {
            DestroyImmediate(createNewCreature.creatureData);
        }
    }

    protected override void OnBeginDrawEditors()
    {
        // Gets currently selected item
        OdinMenuTreeSelection selected = this.MenuTree.Selection;

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();

            if (SirenixEditorGUI.ToolbarButton("Delete Current"))
            {
                CreatureData asset = selected.SelectedValue as CreatureData;
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }


    public class CreateNewCreature
    {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public CreatureData creatureData;
    
        public CreateNewCreature()
        {
            creatureData = ScriptableObject.CreateInstance<CreatureData>();
            creatureData.name = "New Creature";
        }

        [Button("Create New Creature")]
        private void CreateNewCreatureData()
        {
            AssetDatabase.CreateAsset(creatureData, CreatureCreatorMenu.folderPath + creatureData.name + ".asset");
            AssetDatabase.SaveAssets();

            // Creates new instance
            creatureData = ScriptableObject.CreateInstance<CreatureData>();
            creatureData.name = "New Creature";
        }
    }
}

