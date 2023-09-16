using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

public class CreatureCreatorMenu : OdinMenuEditorWindow
{
    [MenuItem("Tools/Glyphix/CreatureCreator")]
    private static void Open()
    {
        var window = GetWindow<CreatureCreatorMenu>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        tree.AddAllAssetsAtPath("Creatures", "Assets/Prefabs/Creatures/0 - Prefabs", typeof(Creature));

        return tree;
    }
}
