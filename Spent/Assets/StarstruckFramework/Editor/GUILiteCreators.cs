#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class GUILiteCreators
{
	[UnityEditor.MenuItem("GameObject/GUILite/GUILiteText (Font Priority)", false, 0)]
	static void CreateGuiLiteFontSizeText(MenuCommand menuCommand)
	{
		string localPath = "Assets/StarstruckFramework/Prefabs/GUILite Templates/GUILiteText (Font Size).prefab";

		GameObject go = GameObject.Instantiate<GameObject>((GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)));
		go.name = "Text";

		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[UnityEditor.MenuItem("GameObject/GUILite/GUILiteText (Container Priority)", false, 0)]
	static void CreateGuiLiteContainerText(MenuCommand menuCommand)
	{
		string localPath = "Assets/StarstruckFramework/Prefabs/GUILite Templates/GUILiteText (Container).prefab";

		GameObject go = GameObject.Instantiate<GameObject>((GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)));
		go.name = "Text";

		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

    [UnityEditor.MenuItem("GameObject/GUILite/GUILiteButton (Without Text)", false, 0)]
    static void CreateGuiLiteButtonWithoutText(MenuCommand menuCommand)
    {
        string localPath = "Assets/StarstruckFramework/Prefabs/GUILite Templates/GUILiteButton (Without Text).prefab";

        GameObject go = GameObject.Instantiate<GameObject>((GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)));
        go.name = "Button";

        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [UnityEditor.MenuItem("GameObject/GUILite/GUILiteButton (With Text)", false, 0)]
    static void CreateGuiLiteButtonWithText(MenuCommand menuCommand)
    {
        string localPath = "Assets/StarstruckFramework/Prefabs/GUILite Templates/GUILiteButton (With Text).prefab";

        GameObject go = GameObject.Instantiate<GameObject>((GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)));
        go.name = "Button";

        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [UnityEditor.MenuItem("GameObject/GUILite/GUILiteCostButton", false, 0)]
    static void CreateGuiLiteCostButton(MenuCommand menuCommand)
    {
        string localPath = "Assets/StarstruckFramework/Prefabs/GUILite Templates/GUILiteCostButton.prefab";

        GameObject go = GameObject.Instantiate<GameObject>((GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)));
        go.name = "CostButton";

        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
#endif