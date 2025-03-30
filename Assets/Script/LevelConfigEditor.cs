// LevelConfigEditor.cs - 帮助创建关卡配置的编辑器脚本
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LevelConfigEditor : EditorWindow
{
    [MenuItem("塔防游戏/关卡编辑器")]
    public static void ShowWindow()
    {
        GetWindow<LevelConfigEditor>("关卡编辑器");
    }

    private LevelConfig levelConfig;
    private SerializedObject serializedObject;
    private SerializedProperty wavesProperty;
    private Vector2 scrollPos;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("关卡配置编辑器", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        levelConfig = (LevelConfig)EditorGUILayout.ObjectField("关卡配置", levelConfig, typeof(LevelConfig), false);

        if (levelConfig == null)
        {
            EditorGUILayout.HelpBox("请选择一个关卡配置或创建新的配置", MessageType.Info);
            if (GUILayout.Button("创建新关卡配置"))
            {
                CreateNewLevelConfig();
            }
            return;
        }

        if (serializedObject == null || serializedObject.targetObject != levelConfig)
        {
            serializedObject = new SerializedObject(levelConfig);
            wavesProperty = serializedObject.FindProperty("waves");
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("initialCurrency"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("波次配置", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.PropertyField(wavesProperty, true);
        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("保存配置"))
        {
            EditorUtility.SetDirty(levelConfig);
            AssetDatabase.SaveAssets();
            Debug.Log($"关卡 {levelConfig.levelName} 配置已保存");
        }
    }

    private void CreateNewLevelConfig()
    {
        string path = EditorUtility.SaveFilePanelInProject("保存关卡配置", "Level_Config", "asset", "选择保存关卡配置的位置");
        if (string.IsNullOrEmpty(path))
            return;

        LevelConfig newConfig = CreateInstance<LevelConfig>();
        AssetDatabase.CreateAsset(newConfig, path);
        AssetDatabase.SaveAssets();
        levelConfig = newConfig;
    }
}
#endif
