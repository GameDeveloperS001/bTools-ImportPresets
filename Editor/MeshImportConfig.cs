using UnityEngine;
using UnityEditor;
using bTools.CodeExtensions;

namespace bTools.ImportPresets
{
	[System.Serializable]
	public class MeshImportConfig : ImportConfigBase
	{
		#region SavedSettings
		//Saved settings
		[SerializeField] public float ScaleFactor;
		[SerializeField] public ModelImporterMeshCompression MeshCompression;
		[SerializeField] public bool ReadWriteEnabled;
		[SerializeField] public bool OptimizeMesh;
		[SerializeField] public bool ImportBlendShapes;
		[SerializeField] public bool GenerateColliders;
		[SerializeField] public bool KeepQuads;
		[SerializeField] public bool SwapUVs;
		[SerializeField] public bool GenerateLightmapUV;
		[SerializeField] public int LightmapHardAngle;
		[SerializeField] public int LightmapPadding;
		[SerializeField] public int LightmapAreaError;
		[SerializeField] public int LightmapAngleError;
		[SerializeField] public ModelImporterNormals ImportNormals;
		[SerializeField] public ModelImporterNormalCalculationMode NormalCalculationMode;
		[SerializeField] public int SmoothingAngle;
		[SerializeField] public ModelImporterTangents TangentMode;
		[SerializeField] public bool ImportMats;
		[SerializeField] public ModelImporterMaterialName MaterialNaming;
		[SerializeField] public ModelImporterMaterialSearch MaterialSearch;
		[SerializeField] public bool ForceNoRig;
		[SerializeField] public bool ImportAnim;
		[SerializeField] public bool ImportCameras;
		[SerializeField] public bool ImportVisibility;
		[SerializeField] public bool ImportLights;
		[SerializeField] public bool WeldVertices;
		#endregion

		public override void DrawInnerGUI()
		{
			DrawFilterGUI();

			EditorGUILayout.LabelField("Meshes", EditorStyles.boldLabel);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
			GUILayout.Space(3);

			ScaleFactor = EditorGUILayout.FloatField("Scale Factor", ScaleFactor);
			MeshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", MeshCompression);
			ReadWriteEnabled = EditorGUILayout.Toggle("Read/Write Enabled", ReadWriteEnabled);
			OptimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", OptimizeMesh);
			ImportBlendShapes = EditorGUILayout.Toggle("Import BlendShapes", ImportBlendShapes);
			GenerateColliders = EditorGUILayout.Toggle("Generate Colliders", GenerateColliders);
			KeepQuads = EditorGUILayout.Toggle("Keep Quads", KeepQuads);
			WeldVertices = EditorGUILayout.Toggle("Weld Vertices", WeldVertices);
			ImportVisibility = EditorGUILayout.Toggle("Import Visibility", ImportVisibility);
			ImportCameras = EditorGUILayout.Toggle("Import Cameras", ImportCameras);
			ImportLights = EditorGUILayout.Toggle("Import Lights", ImportLights);
			SwapUVs = EditorGUILayout.Toggle("Swap UVs", SwapUVs);
			GenerateLightmapUV = EditorGUILayout.Toggle("Generate Lightmap UVs", GenerateLightmapUV);

			if (GenerateLightmapUV)
			{
				EditorGUI.indentLevel++;
				LightmapHardAngle = EditorGUILayout.IntSlider("Hard Angle", LightmapHardAngle, 0, 180);
				LightmapPadding = EditorGUILayout.IntSlider("Pack Margin", LightmapPadding, 1, 64);
				LightmapAngleError = EditorGUILayout.IntSlider("Angle Error", LightmapAngleError, 1, 75);
				LightmapAreaError = EditorGUILayout.IntSlider("Area Error", LightmapAreaError, 1, 75);
				EditorGUI.indentLevel--;
			}

			GUILayout.Space(6);
			EditorGUILayout.LabelField("Normals & Tangent", EditorStyles.boldLabel);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
			GUILayout.Space(3);

			ImportNormals = (ModelImporterNormals)EditorGUILayout.EnumPopup("Normals", ImportNormals);
			EditorGUI.BeginDisabledGroup(ImportNormals == ModelImporterNormals.Calculate ? false : true);
			NormalCalculationMode = (ModelImporterNormalCalculationMode)EditorGUILayout.EnumPopup("Normals Mode", NormalCalculationMode);
			SmoothingAngle = EditorGUILayout.IntSlider("Smoothing Angle", SmoothingAngle, 0, 180);
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(ImportNormals == ModelImporterNormals.None ? true : false);
			TangentMode = (ModelImporterTangents)EditorGUILayout.EnumPopup("Tangents", TangentMode);
			EditorGUI.EndDisabledGroup();

			GUILayout.Space(6);
			EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
			GUILayout.Space(3);

			ImportMats = EditorGUILayout.Toggle("Import Materials", ImportMats);
			if (ImportMats)
			{
				MaterialNaming = (ModelImporterMaterialName)EditorGUILayout.EnumPopup("Material Naming", MaterialNaming);
				MaterialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Material Search", MaterialSearch);
			}

			GUILayout.Space(6);
			EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
			GUILayout.Space(3);

			ImportAnim = EditorGUILayout.Toggle("Import Animation", ImportAnim);
			ForceNoRig = EditorGUILayout.Toggle("Force no rig", ForceNoRig);

			GUILayout.Space(8);
		}
	}
}