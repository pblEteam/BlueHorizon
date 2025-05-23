#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Pinwheel.Griffin;
using Pinwheel.Griffin.PaintTool;
using Pinwheel.Griffin.SplineTool;
using Pinwheel.Griffin.StampTool;
using Pinwheel.Griffin.GroupTool;
using Pinwheel.Griffin.ErosionTool;

namespace Pinwheel.Griffin.Wizard
{
    public static class GCreateLevelTabDrawer
    {
        internal static Vector2 scrollPos;
        internal static MenuCommand menuCmd;

        private class GBaseGUI
        {
            public static readonly GUIContent INSTRUCTION = new GUIContent("Follow the steps below to create your level. Hover on labels for instruction.");
        }

        internal static void Draw()
        {
            //EditorGUILayout.LabelField(GBaseGUI.INSTRUCTION, GEditorCommon.BoldLabel);
            //GEditorCommon.DrawCommonLinks();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            //if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Universal)
            //{
            //    //DrawRenderPipelineSettingGUI();
            //}
            DrawCreateTerrainsGUI();
            //DrawTerrainsManagementGUI();
            //DrawSculptingGUI();
            //DrawTexturingGUI();
            //DrawVertexColorTexturingGUI();
            //DrawSimulationGUI();
            //DrawFoliageAndObjectSpawningGUI();
            //DrawCreateSplineGUI();
            //DrawWaterSkyGUI();
            //DrawUtilitiesGUI();

            EditorGUILayout.EndScrollView();
        }

        private class GRenderPipelineGUI
        {
            public static readonly string LABEL = "0. Universal Render Pipeline Setup";
            public static readonly string ID = "wizard-rp-setup";
            public static readonly GUIContent INSTRUCTION = new GUIContent("Install additional package for Universal Render Pipeline");
            public static readonly GUIContent STATUS_INSTALLED = new GUIContent("Status: INSTALLED");
            public static readonly GUIContent STATUS_NOT_INSTALLED = new GUIContent("Status: NOT INSTALLED");
            public static readonly GUIContent INSTALL_BTN = new GUIContent("Install");
        }


        private static void DrawRenderPipelineSettingGUI()
        {
            GEditorCommon.Foldout(GRenderPipelineGUI.LABEL, true, GRenderPipelineGUI.ID, () =>
            {
                EditorGUILayout.LabelField(GRenderPipelineGUI.INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                EditorGUILayout.LabelField(GPackageInitializer.isUrpSupportInstalled ? GRenderPipelineGUI.STATUS_INSTALLED : GRenderPipelineGUI.STATUS_NOT_INSTALLED, GEditorCommon.WordWrapLeftLabel);
                if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Universal)
                {
                    if (GUILayout.Button(GRenderPipelineGUI.INSTALL_BTN))
                    {
                        GUrpPackageImporter.Import();
#if GRIFFIN_URP
                        //Pinwheel.Griffin.URP.GGriffinUrpInstaller.Install();
#endif
                    }
                }
            });
        }

        private class GCreateTerrainGUI
        {
            public static readonly string LABEL = "1. Create Terrains";
            public static readonly string ID = "wizard-create-terrains";

            public static readonly string PHYSICAL_HEADER = "Position & Size";
            public static readonly GUIContent ORIGIN = new GUIContent("Origin", "Position of the first terrain in the grid.");
            public static readonly GUIContent TILE_SIZE = new GUIContent("Tile Size", "Size of each terrain tile in world space.");
            public static readonly GUIContent TILE_WIDTH = new GUIContent("Tile Width", "Width of each terrain tile in world space.");
            public static readonly GUIContent TILE_LENGTH = new GUIContent("Tile Length", "Length of each terrain tile in world space.");
            public static readonly GUIContent TILE_MAX_HEIGHT = new GUIContent("Tile Height", "Maximum height of each terrain tile in world space.");
            public static readonly GUIContent TILE_X = new GUIContent("Tile Count X", "Number of tiles along X-axis.");
            public static readonly GUIContent TILE_Z = new GUIContent("Tile Count Z", "Number of tiles along Z-axis.");

            public static readonly GUIContent WORLD_SIZE = new GUIContent("World Size", "Size of the terrain grid in world space.");

            public static readonly string MATERIAL_HEADER = "Material";
            public static readonly string UTILITIES_HEADER = "Utilities";
            public static readonly GUIContent NAME_PREFIX = new GUIContent("Name Prefix", "The beginning of each terrain's name. Useful for some level streaming system.");
            public static readonly GUIContent GROUP_ID = new GUIContent("Group Id", "An integer for grouping and connecting adjacent terrain tiles.");

            public static readonly string DATA_HEADER = "Data";
            public static readonly GUIContent DIRECTORY = new GUIContent("Directory", "Where to store created terrain data. A sub-folder of Assets/ is recommended.");
            public static readonly GUIContent CREATE_BTN = new GUIContent("Create");

            public static readonly string LIGHTING_HEADER = "Lighting";
            public static readonly string TEXTURING_HEADER = "Texturing";
        }

        private static void DrawCreateTerrainsGUI()
        {
            //GEditorCommon.Foldout(GCreateTerrainGUI.LABEL, true, GCreateTerrainGUI.ID, () =>
            //{
                GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;
                float spacing = 2;
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float originalLabelWidth = EditorGUIUtility.labelWidth;
                int originalIndent = EditorGUI.indentLevel;

                #region Physical settings
                EditorGUILayout.LabelField(GCreateTerrainGUI.PHYSICAL_HEADER, GEditorCommon.LargeLabel);
                #region pre-register spaces
                EditorGUILayout.GetControlRect(); //Tile Count X label
                EditorGUILayout.GetControlRect(); //Tile Count X field
                Rect rectTerrainGridBase = EditorGUILayout.GetControlRect(GUILayout.Height(lineHeight * 15));
                EditorGUILayout.GetControlRect(); //Origin field
                EditorGUILayout.GetControlRect(); //Tile Width field
                EditorGUILayout.GetControlRect(); //Tile Length field
                EditorGUILayout.GetControlRect(); //Tile Height field
                EditorGUILayout.Space();
                #endregion

                EditorGUI.BeginChangeCheck();
                Handles.BeginGUI();
                //Handles.DrawSolidRectangleWithOutline(r, Color.clear, Color.gray);

                EditorGUIUtility.labelWidth = 70;
                EditorGUI.indentLevel = 0;

                #region Draw terrain grid & measurements
                Vector3 worldSize = new Vector3
                (
                    settings.tileCountX * settings.tileSize.x,
                    settings.tileSize.y,
                    settings.tileCountZ * settings.tileSize.z
                );

                float worldAspect = worldSize.x / worldSize.z;
                float rectTerrainGridBaseSize = rectTerrainGridBase.height;
                Vector2 rectTerrainGridSize = new Vector2(rectTerrainGridBaseSize * worldAspect, rectTerrainGridBaseSize);
                Vector2 rectTerrainGridCenter = rectTerrainGridBase.center - new Vector2(0, lineHeight * 0);
                Rect rectTerrainGrid = new Rect()
                {
                    size = rectTerrainGridSize,
                    center = rectTerrainGridCenter
                };

                float rectTerrainGridScaleFactor = Mathf.Min(1, rectTerrainGridBase.width / rectTerrainGrid.width);
                rectTerrainGrid.size *= rectTerrainGridScaleFactor;
                rectTerrainGrid.center = rectTerrainGridCenter;

                Handles.DrawSolidRectangleWithOutline(rectTerrainGrid, Color.clear, Color.gray);
                for (int z = 0; z < settings.tileCountZ; ++z)
                {
                    for (int x = 0; x < settings.tileCountX; ++x)
                    {
                        Vector2 rectTileSize = new Vector2(rectTerrainGrid.width / settings.tileCountX, rectTerrainGrid.height / settings.tileCountZ);
                        Vector2 rectTilePosition = new Vector2(rectTerrainGrid.min.x + x * rectTileSize.x, rectTerrainGrid.min.y + z * rectTileSize.y);
                        Rect rectTile = new Rect()
                        {
                            size = rectTileSize,
                            position = rectTilePosition
                        };

                        Handles.DrawSolidRectangleWithOutline(rectTile, Color.clear, Color.gray);
                    }
                }

                if (rectTerrainGrid.width > 50 || rectTerrainGrid.height > 50)
                {
                    Handles.color = Handles.xAxisColor;
                    Handles.DrawLine(
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                        new Vector3(rectTerrainGrid.max.x, rectTerrainGrid.max.y),
                        3);
                    Rect rectWorldWidth = new Rect()
                    {
                        size = new Vector2(100, lineHeight),
                        center = new Vector2(rectTerrainGrid.center.x, rectTerrainGrid.max.y - lineHeight * 0.5f)
                    };
                    GUI.contentColor = Handles.xAxisColor * 1.2f;
                    EditorGUI.LabelField(rectWorldWidth, $"{worldSize.x}m", GEditorCommon.CenteredBoldLabel);

                    Handles.color = Handles.zAxisColor;
                    Handles.DrawLine(
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.min.y),
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                        3);
                    Rect rectWorldLength = new Rect()
                    {
                        size = new Vector2(100, lineHeight),
                        position = new Vector2(rectTerrainGrid.min.x + spacing, rectTerrainGrid.center.y - lineHeight * 0.5f)
                    };
                    GUI.contentColor = Handles.zAxisColor * 1.2f;
                    EditorGUI.LabelField(rectWorldLength, $"{worldSize.z}m", GEditorCommon.BoldLabel);
                }
                #endregion

                #region Fields below terrain grid
                GUI.contentColor = Color.white;
                Rect rectOriginField = new Rect()
                {
                    size = new Vector2(Mathf.Clamp(rectTerrainGrid.width, 250, 300), lineHeight),
                    position = new Vector2(Mathf.Max(rectTerrainGridBase.min.x, rectTerrainGrid.min.x), rectTerrainGrid.max.y + spacing * 3)
                };
                Rect rectTileWidthField = new Rect()
                {
                    size = rectOriginField.size,
                    position = new Vector2(rectOriginField.min.x, rectOriginField.max.y + spacing)
                };
                Rect rectTileLengthField = new Rect()
                {
                    size = rectOriginField.size,
                    position = new Vector2(rectTileWidthField.min.x, rectTileWidthField.max.y + spacing)
                };
                Rect rectTileHeightField = new Rect()
                {
                    size = rectOriginField.size,
                    position = new Vector2(rectTileLengthField.min.x, rectTileLengthField.max.y + spacing)
                };

                EditorGUIUtility.wideMode = true;
                const string ORIGIN_FIELD_NAME = "OriginField";
                GUI.SetNextControlName(ORIGIN_FIELD_NAME);
                Vector3 origin = EditorGUI.Vector3Field(rectOriginField, GCreateTerrainGUI.ORIGIN, settings.origin);
                const string TILE_WIDTH_FIELD_NAME = "TileWidthField";
                GUI.SetNextControlName(TILE_WIDTH_FIELD_NAME);
                float tileWidth = EditorGUI.DelayedFloatField(rectTileWidthField, GCreateTerrainGUI.TILE_WIDTH, settings.tileSize.x);
                GUI.enabled = !settings.linkTileSize;
                const string TILE_LENGTH_FIELD_NAME = "TileLengthField";
                GUI.SetNextControlName(TILE_LENGTH_FIELD_NAME);
                float tileLength = EditorGUI.DelayedFloatField(rectTileLengthField, GCreateTerrainGUI.TILE_LENGTH, settings.tileSize.z);
                GUI.enabled = true;
                float tileHeight = EditorGUI.DelayedFloatField(rectTileHeightField, GCreateTerrainGUI.TILE_MAX_HEIGHT, settings.tileSize.y);

                if (string.Equals(ORIGIN_FIELD_NAME, GUI.GetNameOfFocusedControl()) ||
                    rectOriginField.Contains(Event.current.mousePosition))
                {
                    Handles.color = new Color(1, 1, 1, 0.7f);
                    Handles.DrawSolidDisc(
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                        Vector3.forward, 4);
                }

                if (string.Equals(TILE_WIDTH_FIELD_NAME, GUI.GetNameOfFocusedControl()) ||
                    rectTileWidthField.Contains(Event.current.mousePosition))
                {
                    Handles.color = new Color(1, 1, 1, 0.7f);
                    Handles.DrawLine(
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                        new Vector3(rectTerrainGrid.min.x + rectTerrainGrid.width / settings.tileCountX, rectTerrainGrid.max.y),
                        3);
                }

                if (string.Equals(TILE_LENGTH_FIELD_NAME, GUI.GetNameOfFocusedControl()) ||
                  rectTileLengthField.Contains(Event.current.mousePosition))
                {
                    Handles.color = new Color(1, 1, 1, 0.7f);
                    Handles.DrawLine(
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                        new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y - rectTerrainGrid.height / settings.tileCountZ),
                        3);
                }

                #region Link tile size
                Vector3[] linkTileSizeLine = new Vector3[]
                {
                    new Vector2(rectTileWidthField.max.x + spacing, rectTileWidthField.min.y),
                    new Vector2(rectTileWidthField.max.x + spacing*6, rectTileWidthField.min.y),
                    new Vector2(rectTileLengthField.max.x + spacing*6, rectTileLengthField.max.y),
                    new Vector2(rectTileLengthField.max.x + spacing, rectTileLengthField.max.y),
                };
                int[] linkTileSizeLineIndices = new int[] { 0, 1, 1, 2, 2, 3 };
                Handles.color = Color.gray;
                Handles.DrawLines(linkTileSizeLine, linkTileSizeLineIndices);

                Rect linkTileSizeButtonRect = new Rect()
                {
                    size = Vector2.one * lineHeight,
                    center = (linkTileSizeLine[1] + linkTileSizeLine[2]) * 0.5f
                };
                Texture2D linkIcon = null;
                if (settings.linkTileSize)
                {
                    linkIcon = GEditorSkin.Instance.GetTexture("ChainIcon");
                }
                else
                {
                    linkIcon = GEditorSkin.Instance.GetTexture("ChainBreakIcon");
                }
                bool linkTileSize = settings.linkTileSize;
                GUI.color = EditorStyles.label.normal.textColor;
                if (GUI.Button(linkTileSizeButtonRect, linkIcon, EditorStyles.iconButton))
                {
                    linkTileSize = !linkTileSize;
                }
                if (linkTileSize)
                {
                    tileLength = tileWidth;
                }
                GUI.color = Color.white;
                #endregion
                #endregion

                #region Tile Count fields
                float tileCountFieldWidth = 72;
                Rect rectTileCountZField = new Rect()
                {
                    size = new Vector2(tileCountFieldWidth, lineHeight),
                    position = new Vector2(Mathf.Max(rectTerrainGridBase.min.x, rectTerrainGrid.min.x), rectTerrainGrid.min.y - lineHeight - spacing * 2)
                };
                Rect rectTileCountZLabel = new Rect()
                {
                    size = new Vector2(tileCountFieldWidth, lineHeight),
                    position = new Vector2(rectTileCountZField.min.x, rectTileCountZField.min.y - lineHeight)
                };
                EditorGUI.LabelField(rectTileCountZLabel, GCreateTerrainGUI.TILE_Z);
                int tileCountZ = EditorGUI.DelayedIntField(rectTileCountZField, settings.tileCountZ);

                Rect rectTileCountXField = new Rect()
                {
                    size = new Vector2(tileCountFieldWidth, lineHeight),
                    position = new Vector2(Mathf.Min(rectTerrainGridBase.max.x - tileCountFieldWidth, rectTerrainGrid.max.x + spacing * 2), rectTerrainGrid.max.y - lineHeight)
                };
                Rect rectTileCountXLabel = new Rect()
                {
                    size = new Vector2(tileCountFieldWidth, lineHeight),
                    position = new Vector2(rectTileCountXField.min.x, rectTileCountXField.min.y - lineHeight)
                };
                EditorGUI.LabelField(rectTileCountXLabel, GCreateTerrainGUI.TILE_X);
                int tileCountX = EditorGUI.DelayedIntField(rectTileCountXField, settings.tileCountX);
                #endregion
                #endregion

                GEditorCommon.Space();

                #region Material settings
                //if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Builtin)
                //{
                //    EditorGUILayout.LabelField(GCreateTerrainGUI.LIGHTING_HEADER, GEditorCommon.LargeLabel);
                //    EditorGUILayout.BeginHorizontal();
                //    GUILayout.FlexibleSpace();
                //    if (GUILayout.Button("Physical Based", GUILayout.Width(100), GUILayout.Height(200)))
                //    {

                //    }
                //    if (GUILayout.Button("Lambert", GUILayout.Width(100), GUILayout.Height(200)))
                //    {

                //    }
                //    if (GUILayout.Button("Blinn-Phong", GUILayout.Width(100), GUILayout.Height(200)))
                //    {

                //    }

                //    GUILayout.FlexibleSpace();
                //    EditorGUILayout.EndHorizontal();
                //}

                #endregion

                EditorGUIUtility.wideMode = false;
                EditorGUIUtility.labelWidth = originalLabelWidth;
                EditorGUI.indentLevel = originalIndent;
                Handles.EndGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(GEditorSettings.Instance, "Change terrain wizard settings");
                    settings.origin = origin;
                    settings.tileSize = new Vector3(
                        Mathf.Max(1, tileWidth),
                        Mathf.Max(1, tileHeight),
                        Mathf.Max(1, tileLength));
                    settings.tileCountX = Mathf.Max(1, tileCountX);
                    settings.tileCountZ = Mathf.Max(1, tileCountZ);
                    settings.linkTileSize = linkTileSize;
                }

            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.BeginVertical(GUILayout.Width(300));

            //settings.tileSize = GEditorCommon.InlineVector3Field(GCreateTerrainGUI.TILE_SIZE, settings.tileSize);
            //settings.tileSize = new Vector3(
            //    Mathf.Max(1, settings.tileSize.x),
            //    Mathf.Max(1, settings.tileSize.y),
            //    Mathf.Max(1, settings.tileSize.z));

            //settings.tileCountX = EditorGUILayout.IntField(GCreateTerrainGUI.TILE_X, settings.tileCountX);
            //settings.tileCountX = Mathf.Max(1, settings.tileCountX);

            //settings.tileCountZ = EditorGUILayout.IntField(GCreateTerrainGUI.TILE_Z, settings.tileCountZ);
            //settings.tileCountZ = Mathf.Max(1, settings.tileCountZ);

            //float worldSizeX = settings.tileCountX * settings.tileSize.x;
            //float worldSizeY = settings.tileSize.y;
            //float worldSizeZ = settings.tileCountZ * settings.tileSize.z;
            //GUIContent worldSizeContent = new GUIContent($"{worldSizeX}m x {worldSizeY}m x {worldSizeZ}m");
            //EditorGUILayout.LabelField(GCreateTerrainGUI.WORLD_SIZE, worldSizeContent);
            //EditorGUILayout.EndVertical();
            //Rect rPhysicalVis = EditorGUILayout.GetControlRect(GUILayout.Width(EditorGUIUtility.singleLineHeight * 4), GUILayout.Height(EditorGUIUtility.singleLineHeight*4));
            //EditorGUI.DrawRect(rPhysicalVis, Color.red);
            //EditorGUILayout.EndHorizontal();

            //GEditorCommon.Header(GCreateTerrainGUI.MATERIAL_HEADER);
            //GWizardEditorCommon.DrawMaterialSettingsGUI();

            //GEditorCommon.Header(GCreateTerrainGUI.UTILITIES_HEADER);
            //settings.terrainNamePrefix = EditorGUILayout.TextField(GCreateTerrainGUI.NAME_PREFIX, settings.terrainNamePrefix);

            //settings.groupId = EditorGUILayout.IntField(GCreateTerrainGUI.NAME_PREFIX, settings.groupId);

            //GEditorCommon.Header(GCreateTerrainGUI.DATA_HEADER);

            //string dir = settings.dataDirectory;
            //GEditorCommon.BrowseFolder(GCreateTerrainGUI.DIRECTORY, ref dir);
            //if (string.IsNullOrEmpty(dir))
            //{
            //    dir = "Assets/";
            //}
            //settings.dataDirectory = dir;

            if (GUILayout.Button(GCreateTerrainGUI.CREATE_BTN))
            {
                GameObject environmentRoot = null;
                if (menuCmd != null && menuCmd.context != null)
                {
                    environmentRoot = menuCmd.context as GameObject;
                }
                if (environmentRoot == null)
                {
                    environmentRoot = new GameObject("Low Poly Environment");
                    environmentRoot.transform.position = settings.origin;
                }
                GWizard.CreateTerrains(environmentRoot);
            }
            //});
        }

        private class GTerrainManagementGUI
        {
            public static readonly string LABEL = "2. Terrains Management";
            public static readonly string ID = "wizard-terrains-management";
            public static readonly string INSTRUCTION_1 = "Edit properties of an individual terrain by selecting it and use the Inspector.";
            public static readonly string INSTRUCTION_2 = string.Format("Use context menus ({0}) in the terrain Inspector to perform additional tasks.", GEditorCommon.contextIconText);
            public static readonly string INSTRUCTION_3 = "Use the Group Tool to edit properties of multiple terrains at once.";
            public static readonly GUIContent CREATE_BTN = new GUIContent("Create Group Tool");
        }

        private static void DrawTerrainsManagementGUI()
        {
            GEditorCommon.Foldout(GTerrainManagementGUI.LABEL, false, GTerrainManagementGUI.ID, () =>
            {
                EditorGUILayout.LabelField(
                    GTerrainManagementGUI.INSTRUCTION_1,
                    GEditorCommon.WordWrapLeftLabel);
                EditorGUILayout.LabelField(
                    GTerrainManagementGUI.INSTRUCTION_2,
                    GEditorCommon.WordWrapLeftLabel);
                EditorGUILayout.LabelField(
                    GTerrainManagementGUI.INSTRUCTION_3,
                    GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GTerrainManagementGUI.CREATE_BTN))
                {
                    GTerrainGroup group = GWizard.CreateGroupTool();
                    EditorGUIUtility.PingObject(group);
                    Selection.activeGameObject = group.gameObject;
                }
            });
        }

        private class GSculptingGUI
        {
            public static readonly string LABEL = "3. Sculpting";
            public static readonly string ID = "wizard-sculpting";
            public static readonly GUIContent SELECT_WORKFLOW = new GUIContent("Select the workflow you prefer.");

            public static readonly string PAINTING_HEADER = "Painting";
            public static readonly GUIContent PAINTING_INSTRUCTION = new GUIContent("Use a set of painters for hand sculpting terrain shape.");
            public static readonly GUIContent CREATE_PAINTER_BTN = new GUIContent("Create Geometry - Texture Painter");

            public static readonly string STAMPING_HEADER = "Stamping";
            public static readonly GUIContent STAMPING_INSTRUCTION = new GUIContent("Use grayscale textures to stamp mountains, plateaus, rivers, etc. and blend using some math operations.");
            public static readonly GUIContent CREATE_STAMPER_BTN = new GUIContent("Create Geometry Stamper");
        }

        private static void DrawSculptingGUI()
        {
            GEditorCommon.Foldout(GSculptingGUI.LABEL, false, GSculptingGUI.ID, () =>
            {
                EditorGUILayout.LabelField(GSculptingGUI.SELECT_WORKFLOW, GEditorCommon.WordWrapLeftLabel);

                GEditorCommon.Header(GSculptingGUI.PAINTING_HEADER);
                EditorGUILayout.LabelField(GSculptingGUI.PAINTING_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GSculptingGUI.CREATE_PAINTER_BTN))
                {
                    GTerrainTexturePainter painter = GWizard.CreateGeometryTexturePainter();
                    EditorGUIUtility.PingObject(painter.gameObject);
                    Selection.activeGameObject = painter.gameObject;
                }

                GEditorCommon.Header(GSculptingGUI.STAMPING_HEADER);
                EditorGUILayout.LabelField(GSculptingGUI.STAMPING_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GSculptingGUI.CREATE_STAMPER_BTN))
                {
                    GGeometryStamper stamper = GWizard.CreateGeometryStamper();
                    EditorGUIUtility.PingObject(stamper.gameObject);
                    Selection.activeGameObject = stamper.gameObject;
                }
            });
        }

        private class GTexturingGUI
        {
            public static readonly string LABEL = "4. Texturing";
            public static readonly string ID = "wizard-texturing";
            public static readonly GUIContent SELECT_WORKFLOW = new GUIContent("Select the workflow you prefer.");

            public static readonly string PAINTING_HEADER = "Painting";
            public static readonly GUIContent PAINTING_INSTRUCTION = new GUIContent("Use a set of painters for hand painting terrain color.");
            public static readonly GUIContent CREATE_PAINTER_BTN = new GUIContent("Create Geometry - Texture Painter");

            public static readonly string STAMPING_HEADER = "Stamping";
            public static readonly GUIContent STAMPING_INSTRUCTION = new GUIContent("Use stamper to color the terrain procedurally with some rules such as height, normal vector and noise.");
            public static readonly GUIContent CREATE_STAMPER_BTN = new GUIContent("Create Texture Stamper");
        }

        private static void DrawTexturingGUI()
        {
            GEditorCommon.Foldout(GTexturingGUI.LABEL, false, GTexturingGUI.ID, () =>
            {
                EditorGUILayout.LabelField(GTexturingGUI.SELECT_WORKFLOW, GEditorCommon.WordWrapLeftLabel);

                GEditorCommon.Header(GTexturingGUI.PAINTING_HEADER);
                EditorGUILayout.LabelField(GTexturingGUI.PAINTING_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GTexturingGUI.CREATE_PAINTER_BTN))
                {
                    GTerrainTexturePainter painter = GWizard.CreateGeometryTexturePainter();
                    EditorGUIUtility.PingObject(painter.gameObject);
                    Selection.activeGameObject = painter.gameObject;
                }

                GEditorCommon.Header(GTexturingGUI.STAMPING_HEADER);
                EditorGUILayout.LabelField(GTexturingGUI.STAMPING_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GTexturingGUI.CREATE_STAMPER_BTN))
                {
                    GTextureStamper stamper = GWizard.CreateTextureStamper();
                    EditorGUIUtility.PingObject(stamper.gameObject);
                    Selection.activeGameObject = stamper.gameObject;
                }
            });
        }

        private class GVertexColorTexturingGUI
        {
            public static readonly string LABEL = "4.1. Vertex Color Texturing";
            public static readonly string ID = "wizard-vertex-color-texturing";

            public static readonly GUIContent INSTRUCTION_1 = new GUIContent("To enable vertex coloring, do the following steps.");
            public static readonly GUIContent INSTRUCTION_2 = new GUIContent("Set <i>terrain> Geometry> Albedo To Vertex Color</i> to Sharp or Smooth");
            public static readonly GUIContent INSTRUCTION_3 = new GUIContent("For Painting workflow: Select the Geometry - Texture Painter and enable <i>Force Update Geometry</i>, then use Albedo mode to paint.");
            public static readonly GUIContent INSTRUCTION_4 = new GUIContent("For Stamping workflow: Stamp to Albedo map and regenerate terrain meshes by select <i>terrain> Geometry> CONTEXT (≡)> Update</i>");
        }

        private static void DrawVertexColorTexturingGUI()
        {
            GEditorCommon.Foldout(GVertexColorTexturingGUI.LABEL, false, GVertexColorTexturingGUI.ID, () =>
            {
                EditorGUILayout.LabelField(
                    GVertexColorTexturingGUI.INSTRUCTION_1, GEditorCommon.WordWrapLeftLabel);
                EditorGUILayout.LabelField(
                    GVertexColorTexturingGUI.INSTRUCTION_2, GEditorCommon.RichTextLabel);
                EditorGUILayout.LabelField(
                    GVertexColorTexturingGUI.INSTRUCTION_3, GEditorCommon.RichTextLabel);
                EditorGUILayout.LabelField(
                    GVertexColorTexturingGUI.INSTRUCTION_4, GEditorCommon.RichTextLabel);
            });
        }

        private class GSimulationGUI
        {
            public static readonly string LABEL = "5. Simulation";
            public static readonly string ID = "wizard-simulation";
            public static readonly string INSTRUCTION = "Simulate natural effect such as hydraulic and thermal erosion on the terrain surface.";
            public static readonly string CREATE_SIMULATOR_LABEL = "Create Erosion Simulator";
        }

        private static void DrawSimulationGUI()
        {
            GEditorCommon.Foldout(GSimulationGUI.LABEL, false, GSimulationGUI.ID, () =>
            {
                EditorGUILayout.LabelField(GSimulationGUI.INSTRUCTION, GEditorCommon.WordWrapItalicLabel);
                if (GUILayout.Button(GSimulationGUI.CREATE_SIMULATOR_LABEL))
                {
                    GErosionSimulator simulator = GWizard.CreateErosionSimulator();
                    EditorGUIUtility.PingObject(simulator);
                    Selection.activeGameObject = simulator.gameObject;
                }
            });
        }

        private class GSpawningGUI
        {
            public static readonly string LABEL = "6. Foliage & Object Spawning";
            public static readonly string ID = "wizard-foliage-object-spawning";
            public static readonly GUIContent SELECT_WORKFLOW = new GUIContent("Select the workflow you prefer.");

            public static readonly string PAINTING_HEADER = "Painting";
            public static readonly GUIContent PAINTING_INSTRUCTION = new GUIContent("Place trees, grasses and game objects by painting.");
            public static readonly GUIContent CREATE_PAINTER_BTN = new GUIContent("Create Foliage Painter & Object Painter");

            public static readonly string STAMPING_HEADER = "Stamping";
            public static readonly GUIContent STAMPING_INSTRUCTION = new GUIContent("Procedurally spawn trees, grasses and game objects using some rules such as height, normal vector and noise.");
            public static readonly GUIContent CREATE_STAMPER_BTN = new GUIContent("Create Foliage Stamper & Object Stamper");
        }

        private static void DrawFoliageAndObjectSpawningGUI()
        {
            GEditorCommon.Foldout(GSpawningGUI.LABEL, false, GSpawningGUI.ID, () =>
            {
                EditorGUILayout.LabelField(GSpawningGUI.SELECT_WORKFLOW, GEditorCommon.WordWrapLeftLabel);

                GEditorCommon.Header(GSpawningGUI.PAINTING_HEADER);
                EditorGUILayout.LabelField(GSpawningGUI.PAINTING_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GSpawningGUI.CREATE_PAINTER_BTN))
                {
                    GFoliagePainter fPainter = GWizard.CreateFoliagePainter();
                    GObjectPainter oPainter = GWizard.CreateObjectPainter();
                    EditorGUIUtility.PingObject(fPainter);
                    Selection.objects = new GameObject[] { fPainter.gameObject, oPainter.gameObject };
                    Selection.activeGameObject = fPainter.gameObject;
                }

                GEditorCommon.Header(GSpawningGUI.STAMPING_HEADER);
                EditorGUILayout.LabelField(GSpawningGUI.STAMPING_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GSpawningGUI.CREATE_STAMPER_BTN))
                {
                    GFoliageStamper fStamper = GWizard.CreateFoliageStamper();
                    GObjectStamper oStamper = GWizard.CreateObjectStamper();
                    EditorGUIUtility.PingObject(fStamper);
                    Selection.objects = new GameObject[] { fStamper.gameObject, oStamper.gameObject };
                    Selection.activeGameObject = fStamper.gameObject;
                }
            });
        }

        private class GCreateSplineGUI
        {
            public static readonly string LABEL = "7. Create Roads, Ramps, Rivers, etc.";
            public static readonly string ID = "wizard-spline";

            public static readonly GUIContent INSTRUCTION = new GUIContent("Use Spline Tool to paint roads, make ramps and riverbeds, etc.");
            public static readonly GUIContent CREATE_BTN = new GUIContent("Create Spline Tool");
        }

        private static void DrawCreateSplineGUI()
        {
            GEditorCommon.Foldout(GCreateSplineGUI.LABEL, false, GCreateSplineGUI.ID, () =>
            {
                EditorGUILayout.LabelField(GCreateSplineGUI.INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GCreateSplineGUI.CREATE_BTN))
                {
                    GSplineCreator spline = GWizard.CreateSplineTool();
                    EditorGUIUtility.PingObject(spline);
                    Selection.activeGameObject = spline.gameObject;
                }
            });
        }

        private class GWaterSkyGUI
        {
            public static readonly string LABEL = "8. Adding Water & Sky";
            public static readonly string ID = "wizard-id";

            public static readonly string WATER_HEADER = "Water";
            public static readonly GUIContent WATER_INSTRUCTION = new GUIContent("Poseidon is a low poly water system with high visual quality and performance.");
            public static readonly GUIContent GET_POSEIDON_BTN = new GUIContent("Get Poseidon");

            public static readonly string SKY_HEADER = "Sky";
            public static readonly GUIContent SKY_INSTRUCTION = new GUIContent("Jupiter is a single pass sky shader with day night cycle support.");
            public static readonly GUIContent GET_JUPITER_BTN = new GUIContent("Get Jupiter");
        }

        private static void DrawWaterSkyGUI()
        {
            GEditorCommon.Foldout(GWaterSkyGUI.LABEL, false, GWaterSkyGUI.ID, () =>
            {
                GEditorCommon.Header(GWaterSkyGUI.WATER_HEADER);
                EditorGUILayout.LabelField(GWaterSkyGUI.WATER_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GWaterSkyGUI.GET_POSEIDON_BTN))
                {
                    Application.OpenURL(GAssetLink.POSEIDON);
                }

                GEditorCommon.Header(GWaterSkyGUI.SKY_HEADER);
                EditorGUILayout.LabelField(
                    GWaterSkyGUI.SKY_INSTRUCTION, GEditorCommon.WordWrapLeftLabel);
                if (GUILayout.Button(GWaterSkyGUI.GET_JUPITER_BTN))
                {
                    Application.OpenURL(GAssetLink.JUPITER);
                }
            });
        }

        private class GUtilitiesGUI
        {
            public static readonly string LABEL = "9. Utilities";
            public static readonly string ID = "wizard-utilities";

            public static readonly string WIND_ZONE_HEADER = "Wind Zone";
            public static readonly GUIContent WIND_ZONE_INSTRUCTION = new GUIContent("Adding Wind Zone to customize how grass react to wind in this level.");
            public static readonly GUIContent CREATE_WIND_ZONE_BTN = new GUIContent("Create Wind Zone");
        }

        private static void DrawUtilitiesGUI()
        {
            GEditorCommon.Foldout(GUtilitiesGUI.LABEL, false, GUtilitiesGUI.ID, () =>
            {
                GEditorCommon.Header(GUtilitiesGUI.WIND_ZONE_HEADER);
                EditorGUILayout.LabelField(GUtilitiesGUI.WIND_ZONE_INSTRUCTION);
                if (GUILayout.Button(GUtilitiesGUI.CREATE_WIND_ZONE_BTN))
                {
                    GWindZone wind = GWizard.CreateWindZone();
                    EditorGUIUtility.PingObject(wind.gameObject);
                    Selection.activeGameObject = wind.gameObject;
                }
            });
        }
    }
}
#endif
