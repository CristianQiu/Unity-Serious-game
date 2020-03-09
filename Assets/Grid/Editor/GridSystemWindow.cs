using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// The window to be able to see and modify the grid master settings
/// </summary>
public class GridSystemWindow : EditorWindow
{
    #region Private Attributes

    private GridMaster gridMaster = null;
    private Material debugGridMat = null;

    #endregion

    #region Properties

    public bool IsThereGridAndIsBuilt { get { return (gridMaster != null && gridMaster.HasGridBeenBuilt); } }

    #endregion

    #region EditorWindow Methods

    [MenuItem("Window/Grid System")]
    private static void Init()
    {
        // get existing open window or if none, make a new one
        GridSystemWindow window = GetWindow<GridSystemWindow>("Grid System");
        window.Show();
    }

    /// <summary>
    /// Called when the window is created, if any tab of this window is already there even if not focused, it isn't called
    /// </summary>
    private void OnEnable()
    {
        // register whenever the application changes its mode (play, stop...)
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        // register to whenever a new scene is opened
        EditorSceneManager.sceneOpened += OnSceneOpened;
        // register to receive the event when a cam finishes rendering
        Camera.onPostRender += OnCamPostRender;

        Initialize(true);
    }

    /// <summary>
    /// Method to draw in the window
    /// </summary>
    private void OnGUI()
    {
        GUISpaces(1);

        if (Application.isPlaying)
        {
            ShowCurrentGridSettings();
            ShowInPlayWarning();
            return;
        }

        ShowModifyGrid();

        GUISpaces(1);

        ShowLayerOptions();

        GUISpaces(1);

        ShowDebugOptions();

        GUISpaces(1);

        ShowUpdateGridButton();
    }

    /// <summary>
    /// Called when the window is closed
    /// </summary>
    private void OnDisable()
    {
        // unregister
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        Camera.onPostRender -= OnCamPostRender;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="forceGridRecreationIfGridMasterFound"></param>
    private void Initialize(bool forceGridRecreationIfGridMasterFound)
    {
        // look for the grid master in the scene
        gridMaster = FindObjectOfType<GridMaster>();

        // get the debug material
        if (debugGridMat == null)
            debugGridMat = new Material(Shader.Find("Debug/DefaultDebug"));

        // if the grid master wasn't found we'd want to create a new one
        if (gridMaster == null)
        {
            CreateGridMaster(true);
        }
        else
        {
            // force the recreation in certain cases to avoid bugs
            if (forceGridRecreationIfGridMasterFound)
                gridMaster.CreateGrid(gridMaster.nodeDiameter, gridMaster.numRows, gridMaster.numCols);
        }
    }

    /// <summary>
    /// Create the grid master and choose wether to also create a first grid instantly
    /// </summary>
    /// <param name="alsoCreateGrid"></param>
    private void CreateGridMaster(bool alsoCreateGrid)
    {
        // create the object
        GameObject gridMasterObj = new GameObject();
        gridMasterObj.transform.position = Vector3.zero;
        gridMasterObj.transform.rotation = Quaternion.identity;
        gridMasterObj.name = "GridMaster";

        // add the grid master component
        gridMaster = gridMasterObj.AddComponent<GridMaster>();

        if (alsoCreateGrid)
            gridMaster.CreateGrid(gridMaster.nodeDiameter, gridMaster.numRows, gridMaster.numCols);
    }

    #endregion

    #region GUI Methods

    /// <summary>
    /// Show the current grid settings
    /// </summary>
    private void ShowCurrentGridSettings()
    {
        GUIStyle style = CreateGUIStyle(0, FontStyle.Bold, TextAnchor.MiddleCenter);
        
        // show section title
        EditorGUILayout.LabelField("Current Grid", style);

        style = CreateGUIStyle(0, FontStyle.Normal, TextAnchor.MiddleCenter);

        // and the settings
        EditorGUILayout.BeginVertical("box");

        // if there is a grid and is built show its settings
        if (IsThereGridAndIsBuilt)
        {
            EditorGUILayout.LabelField("Node diameter : " + gridMaster.nodeDiameter, style);
            EditorGUILayout.LabelField("Grid rows : " + gridMaster.numRows, style);
            EditorGUILayout.LabelField("Grid columns : " + gridMaster.numCols, style);
        }
        // otherwise display a message saying there is not a grid
        else
        {
            GUISpaces(1);
            EditorGUILayout.LabelField("There is not a GridMaster or the grid has not been build yet", style);
            GUISpaces(1);
        }
      
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Show a message telling the user that nothing can be modified in play mode
    /// </summary>
    private void ShowInPlayWarning()
    {
        GUIStyle style = CreateGUIStyle(0, FontStyle.Normal, TextAnchor.MiddleCenter);
        style.normal.textColor = Color.red;

        EditorGUILayout.BeginVertical("box");

        GUISpaces(1);
        EditorGUILayout.LabelField("Nothing can be modified in play mode !", style);
        GUISpaces(1);

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Show the settings to modify the grid
    /// </summary>
    private void ShowModifyGrid()
    {
        GUIStyle style = CreateGUIStyle(0, FontStyle.Bold, TextAnchor.MiddleCenter);

        // show section title
        EditorGUILayout.LabelField("Modify Grid", style);

        EditorGUI.BeginChangeCheck();

        // node diameter
        EditorGUILayout.BeginVertical("box");
        gridMaster.nodeDiameter = EditorGUILayout.Slider("Node diameter", gridMaster.nodeDiameter, 0.1f, 2.0f);

        // grid rows
        gridMaster.numRows = EditorGUILayout.IntSlider("Grid rows", gridMaster.numRows, 5, 50);

        // grid columns
        gridMaster.numCols = EditorGUILayout.IntSlider("Grid columns", gridMaster.numCols, 5, 50);
        EditorGUILayout.EndVertical();

        EditorGUI.EndChangeCheck();

        if (GUI.changed)
        {
            // for some reason the grid master was deleted with the window open, recreate it
            if (gridMaster == null)
                CreateGridMaster(false);

            // if the parameters changed recreate the grid
            gridMaster.CreateGrid(gridMaster.nodeDiameter, gridMaster.numRows, gridMaster.numCols);

            // mark the scene as dirty so we may be asked to save it before exiting the scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    /// <summary>
    /// Show the layers options
    /// </summary>
    private void ShowLayerOptions()
    {
        GUIStyle style = CreateGUIStyle(0, FontStyle.Bold, TextAnchor.MiddleCenter);

        // show section title
        EditorGUILayout.LabelField("Layers", style);

        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginChangeCheck();

        // get the names of the layers
        gridMaster.obstacleLayerName = EditorGUILayout.TextField("Obstacle layer name", gridMaster.obstacleLayerName);
        gridMaster.thinWallLayerName = EditorGUILayout.TextField("Thin wall layer name", gridMaster.thinWallLayerName);

        // and update the masks from the gridMaster if they changed
        if (GUI.changed)
            gridMaster.BuildMasks();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Show debug options
    /// </summary>
    private void ShowDebugOptions()
    {
        GUIStyle style = CreateGUIStyle(0, FontStyle.Bold, TextAnchor.MiddleCenter);

        // show section title
        EditorGUILayout.LabelField("Debug", style);

        EditorGUILayout.BeginVertical("box");

        gridMaster.walkableColor = EditorGUILayout.ColorField("Walkable color", gridMaster.walkableColor);
        gridMaster.notWalkableColor = EditorGUILayout.ColorField("Not walkable color", gridMaster.notWalkableColor);
        gridMaster.hasAlignedWallAtSomeSideColor = EditorGUILayout.ColorField("Touching wall color", gridMaster.hasAlignedWallAtSomeSideColor);

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Show the button to update the grid
    /// </summary>
    private void ShowUpdateGridButton()
    {
        // only show it if there's a grid
        if (!IsThereGridAndIsBuilt)
            return;

        // update if pressed
        if (GUILayout.Button("Update grid"))
            gridMaster.CreateGrid(gridMaster.nodeDiameter, gridMaster.numRows, gridMaster.numCols);
    }

    /// <summary>
    /// Helper method to create GUI styles. If fontSize is 0 the default size will be used
    /// </summary>
    /// <param name="fontSize"></param>
    /// <param name="fontStyle"></param>
    /// <param name="textAnchor"></param>
    /// <returns></returns>
    private GUIStyle CreateGUIStyle(int fontSize, FontStyle fontStyle, TextAnchor textAnchor)
    {
        GUIStyle style = new GUIStyle
        {
            fontStyle = fontStyle,
            alignment = textAnchor
        };

        if (fontSize > 0)
            style.fontSize = fontSize;

        return style;
    }

    /// <summary>
    /// Create a certain amount of vertical spaces
    /// </summary>
    /// <param name="numSpaces"></param>
    private void GUISpaces(int numSpaces)
    {
        for (int i = 0; i < numSpaces; i++)
            EditorGUILayout.Space();
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Function registered to be called each time the play mode changes
    /// </summary>
    /// <param name="newState"></param>
    private void OnPlayModeStateChanged(PlayModeStateChange newState)
    {
        if (newState == PlayModeStateChange.EnteredPlayMode)
            Initialize(false);
        else if (newState == PlayModeStateChange.EnteredEditMode)
            Initialize(true);

        Repaint();
    }

    /// <summary>
    /// Function registered to be called each time a scene is opened
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        // probably we don't want to deal with multiscene editing
        // ...

        Initialize(true);
        Repaint();
    }

    /// <summary>
    /// Called right after any camera finishes rendering
    /// </summary>
    /// <param name="currCam"></param>
    private void OnCamPostRender(Camera currCam)
    {
        if (!IsThereGridAndIsBuilt)
            return;

        if (Event.current.type == EventType.Repaint)
        {
            // prepare some variables used inside the for loop
            float diameter = gridMaster.nodeDiameter;
            float smallAmount = diameter * 0.02f;
            float loweredDiameter = diameter - smallAmount;

            int rows = gridMaster.numRows;
            int cols = gridMaster.numCols;

            Color walkableCol = gridMaster.walkableColor;
            Color notWalkableCol = gridMaster.notWalkableColor;
            Color hasWallCol = gridMaster.hasAlignedWallAtSomeSideColor;

            // prepare to draw all the grid using quads
            GL.PushMatrix();
            debugGridMat.SetPass(0);
            GL.MultMatrix(gridMaster.transform.localToWorldMatrix);
            GL.Begin(GL.QUADS);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    GridNode n = gridMaster.Nodes[i][j];

                    float x = j * diameter;
                    float z = i * diameter;

                    // we'll use a little offset so we can see separation in between the quads
                    float xSmall = x + smallAmount;
                    float xLowered = x + loweredDiameter;

                    float zLowered = z + loweredDiameter;
                    float zSmall = z + smallAmount;

                    Color c = walkableCol;

                    if (!n.Walkable)
                        c = notWalkableCol;
                    else if (n.HasAlignedWallAtSomeSide)
                        c = hasWallCol;

                    GL.Color(c);
                    GL.Vertex3(xSmall, 0.0f, zLowered);
                    GL.Vertex3(xSmall, 0.0f, zSmall);
                    GL.Vertex3(xLowered, 0.0f, zSmall);
                    GL.Vertex3(xLowered, 0.0f, zLowered);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }

    #endregion
}