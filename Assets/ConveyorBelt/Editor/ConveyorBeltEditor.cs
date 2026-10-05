using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ConveyorBeltTagInitializer
{
    static ConveyorBeltTagInitializer()
    {
        EnsureTagExists("ConveyorBelt");
    }

    private static void EnsureTagExists(string tagName)
    {
        if (!TagExists(tagName))
        {
            AddTag(tagName);
        }
    }

    private static bool TagExists(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagName)) return true;
        }
        return false;
    }

    private static void AddTag(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
        tagManager.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ConveyorBelt))]
public class ConveyorBeltEditor : Editor
{
    private Texture2D logo;
    private ConveyorBelt belt;

    private bool showShape = true;
    private bool showExtrusion = true;
    private bool showMaterial = true;
    private bool showAudio = true;
    private bool showTrigger = true;
    private bool showEvents = true;
    private bool showDebug = true;

    private void OnEnable()
    {
        logo = Resources.Load<Texture2D>("ConveyorBeltLogo");
        belt = (ConveyorBelt)target;

        // Asegurar que el objeto tenga el tag correcto
        if (!belt.gameObject.CompareTag("ConveyorBelt"))
        {
            belt.gameObject.tag = "ConveyorBelt";
            EditorUtility.SetDirty(belt);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawHeaderBar();

        showShape = EditorGUILayout.BeginFoldoutHeaderGroup(showShape, "Profile Shape");
        if (showShape)
        {
            EditorGUILayout.HelpBox("Defines the shape of the conveyor belt in 3D space.", MessageType.None);
            DrawTooltipField(nameof(belt.length), "Conveyor conveyorLength (X axis)");
            DrawTooltipField(nameof(belt.height), "Conveyor height (Y axis)");
            DrawTooltipField(nameof(belt.cornerSegments), "Number of segments used to round corners");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawDivider();

        showExtrusion = EditorGUILayout.BeginFoldoutHeaderGroup(showExtrusion, "Extrusion");
        if (showExtrusion)
        {
            EditorGUILayout.HelpBox("Controls conveyor belt length and mesh resolution.", MessageType.None);
            DrawTooltipField(nameof(belt.width), "Total conveyor length (Z axis)");
            DrawTooltipField(nameof(belt.segments), "Number of segments for extrusion along length");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawDivider();

        showMaterial = EditorGUILayout.BeginFoldoutHeaderGroup(showMaterial, "Material & UV");
        if (showMaterial)
        {
            DrawTooltipField(nameof(belt.material), "Material to apply to the conveyor");
            DrawTooltipField(nameof(belt.uvScaleU), "Tiling scale in U direction (length)");
            DrawTooltipField(nameof(belt.uvScaleV), "Tiling scale in V direction (width)");
            DrawTooltipField(nameof(belt.scrollSpeed), "Scroll speed for UV animation");
            DrawTooltipField(nameof(belt.scrollSpeedModifier), "Modifier to match UV scroll with object motion");
            DrawTooltipField(nameof(belt.startConveyorBeltOnPlay), "Start conveyor automatically when playing");
            DrawTooltipField(nameof(belt.scrollCurve), "Speed modulation over time");
            DrawTooltipField(nameof(belt.textureTilingU_PerWorldUnit), "World units per U tile");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawDivider();

        showAudio = EditorGUILayout.BeginFoldoutHeaderGroup(showAudio, "Audio");
        if (showAudio)
        {
            DrawTooltipField(nameof(belt.startSound), "Sound played when starting");
            DrawTooltipField(nameof(belt.runningSound), "Looped running sound");
            DrawTooltipField(nameof(belt.stopSound), "Sound played when stopping");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawDivider();

        showTrigger = EditorGUILayout.BeginFoldoutHeaderGroup(showTrigger, "Trigger Movement");
        if (showTrigger)
        {
            DrawTooltipField(nameof(belt.moveObjectsInTrigger), "Move objects detected in trigger zone");
            DrawTooltipField(nameof(belt.triggerSizeOffset), "Extra size added to trigger bounds");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawDivider();

        showEvents = EditorGUILayout.BeginFoldoutHeaderGroup(showEvents, "Events");
        if (showEvents)
        {
            DrawTooltipField(nameof(belt.onStart), "Event triggered on start");
            DrawTooltipField(nameof(belt.onStop), "Event triggered on stop");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        DrawDivider();

        showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
        if (showDebug)
        {
            DrawTooltipField(nameof(belt.showGizmos), "Toggle scene gizmo drawing");
            DrawTooltipField(nameof(belt.gizmoColor), "Color used for gizmos");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Start", EditorGUIUtility.IconContent("PlayButton").image)))
        {
            belt.StartConveyorBelt();
            EditorUtility.SetDirty(belt);
        }
        if (GUILayout.Button(new GUIContent("Stop", EditorGUIUtility.IconContent("PauseButton").image)))
        {
            belt.StopConveyorBelt();
            EditorUtility.SetDirty(belt);
        }
        if (GUILayout.Button(new GUIContent("Generate Mesh", EditorGUIUtility.IconContent("d_PreTextureMipMapHigh").image)))
        {
            belt.GenerateExtrudedMesh();
            EditorUtility.SetDirty(belt);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("© 2025 Faktory Studios", EditorStyles.centeredGreyMiniLabel);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTooltipField(string propertyName, string tooltip)
    {
        SerializedProperty prop = serializedObject.FindProperty(propertyName);
        if (prop != null)
        {
            GUIContent label = new GUIContent(ObjectNames.NicifyVariableName(propertyName), tooltip);
            EditorGUILayout.PropertyField(prop, label, true);
        }
    }

    private void DrawDivider()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
    }

    private void DrawHeaderBar()
    {
        if (logo)
        {
            GUILayout.Label(logo, GUILayout.Height(80));
        }

        Rect rect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
        GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(10, 0, 10, 0)
        };
        GUI.Label(rect, " Conveyor Belt System", titleStyle);
    }
}