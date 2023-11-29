using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open in Editor"))
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);
        }

        base.OnInspectorGUI();
    }
}

public class DialogueEditor : EditorWindow
{
    private DialogueData_SO currentData;
    private ReorderableList piecesList = null;
    private Vector2 scrollPos = Vector2.zero;
    private Dictionary<string, ReorderableList> optionListDict = new Dictionary<string, ReorderableList>();

    [MenuItem("3D RPG/Dialogue Editor")]
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.autoRepaintOnSceneChange = true;
    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.currentData = data;
    }

    [OnOpenAsset]
    public static bool OpenAsset(int instanceID, int line)
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instanceID) as DialogueData_SO;
        if (data != null)
        {
            InitWindow(data);
            return true;
        }

        return false;
    }

    #region Event Functions

    private void OnGUI()
    {
        if (currentData != null)
        {
            EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);
            GUILayout.Space(10);

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (piecesList == null)
            {
                SetUpReorderableList();
            }

            piecesList.DoLayoutList();
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No Dialogue Data Selected", EditorStyles.boldLabel);

            if (GUILayout.Button("Create New Dialogue Data"))
            {
                string dataPath = "Assets/Game Data/Dialogue Data/";
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }

                DialogueData_SO newData = CreateInstance<DialogueData_SO>();
                AssetDatabase.CreateAsset(newData, dataPath + "New Dialogue Data.asset");
                currentData = newData;
            }
        }
    }

    private void OnSelectionChange()
    {
        var newData = Selection.activeObject as DialogueData_SO;
        if (newData != null)
        {
            currentData = newData;
            SetUpReorderableList();
        }
        else
        {
            currentData = null;
            piecesList = null;
        }

        Repaint();
    }

    private void OnDisable()
    {
        optionListDict.Clear();
    }

    #endregion

    private void SetUpReorderableList()
    {
        piecesList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece),
            true, true, true, true);

        piecesList.drawHeaderCallback += OnDrawPieceHeader;
        piecesList.drawElementCallback += OnDrawPieceElement;
        piecesList.elementHeightCallback += OnHeightChanged;
    }

    #region Callback Functions

    private void OnDrawPieceHeader(Rect rect)
    {
        GUI.Label(rect, "Dialogue Pieces");
    }

    private void OnDrawPieceElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorUtility.SetDirty(currentData);
        GUIStyle textStyle = new GUIStyle("TextField");

        if (index < currentData.dialoguePieces.Count)
        {
            var currentPiece = currentData.dialoguePieces[index];
            var tempRect = rect;

            tempRect.height = EditorGUIUtility.singleLineHeight;
            currentPiece.IsExpand = EditorGUI.Foldout(tempRect, currentPiece.IsExpand, currentPiece.ID);

            if (currentPiece.IsExpand)
            {
                tempRect.width = 30;
                tempRect.y += tempRect.height;
                EditorGUI.LabelField(tempRect, "ID");

                tempRect.x += tempRect.width;
                tempRect.width = 50;
                currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID);

                tempRect.x += tempRect.width + 10;
                EditorGUI.LabelField(tempRect, "Quest");

                tempRect.x += 50;
                tempRect.width = 200;
                currentPiece.quest =
                    (QuestData_SO)EditorGUI.ObjectField(tempRect, currentPiece.quest, typeof(QuestData_SO), false);

                tempRect.y += EditorGUIUtility.singleLineHeight + 5;
                tempRect.x = rect.x;
                tempRect.height = 60;
                tempRect.width = tempRect.height;
                currentPiece.image = (Sprite)EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false);

                tempRect.x += tempRect.width + 5;
                tempRect.width = rect.width - tempRect.x;
                textStyle.wordWrap = true;
                currentPiece.text = EditorGUI.TextField(tempRect, currentPiece.text, textStyle);

                tempRect.y += tempRect.height + 5;
                tempRect.x = rect.x;
                tempRect.width = rect.width;

                string optionListKey = currentPiece.ID + currentPiece.text;
                if (optionListKey != null)
                {
                    if (!optionListDict.ContainsKey(optionListKey))
                    {
                        var optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption),
                            true, true, true, true);

                        optionList.drawHeaderCallback += OnDrawOptionHeader;
                        optionList.drawElementCallback += (rect, index, isActive, isFocused) =>
                        {
                            OnDrawOptionElement(currentPiece, rect, index, isActive, isFocused);
                        };
                        optionListDict[optionListKey] = optionList;
                    }

                    optionListDict[optionListKey].DoList(tempRect);
                }
            }
        }
    }

    private float OnHeightChanged(int index)
    {
        return GetPieceHeight(currentData.dialoguePieces[index]);
    }

    private float GetPieceHeight(DialoguePiece piece)
    {
        var height = EditorGUIUtility.singleLineHeight;
        var isExpand = piece.IsExpand;

        if (isExpand)
        {
            height += EditorGUIUtility.singleLineHeight * 9;

            var options = piece.options;
            if (options.Count > 1)
            {
                height += EditorGUIUtility.singleLineHeight * options.Count;
            }
        }

        return height;
    }

    private void OnDrawOptionHeader(Rect rect)
    {
        GUI.Label(rect, "Option Text");
        rect.x += rect.width * 0.5f + 10;
        GUI.Label(rect, "Target ID");
        rect.x += rect.width * 0.3f;
        GUI.Label(rect, "Apply");
    }

    private void OnDrawOptionElement(DialoguePiece currentPiece, Rect rect, int index, bool isActive, bool isFocused)
    {
        var currentOption = currentPiece.options[index];
        var tempRect = rect;

        tempRect.width = rect.width * 0.5f;
        currentOption.text = EditorGUI.TextField(tempRect, currentOption.text);

        tempRect.x += tempRect.width + 5;
        tempRect.width = rect.width * 0.3f;
        currentOption.targetID = EditorGUI.TextField(tempRect, currentOption.targetID);

        tempRect.x += tempRect.width + 5;
        tempRect.width = rect.width * 0.2f;
        currentOption.takeQuest = EditorGUI.Toggle(tempRect, currentOption.takeQuest);
    }

    #endregion
}