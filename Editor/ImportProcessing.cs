using UnityEngine;
using UnityEditor;
using bTools.CodeExtensions;
using System.IO;

namespace bTools.ImportPresets
{
	public class ImportProcessing : AssetPostprocessor
	{
		private char[] filterSep = new char[] { ';' };
		private static SavedImportSettings m_importSettingsData;
		public static SavedImportSettings importSettingsData
		{
			get
			{
				if ( m_importSettingsData == null )
				{
					SavedImportSettings loadedSettings;

					loadedSettings = EditorGUIExtensions.LoadAssetWithName<SavedImportSettings>( "ImportSettingsPresets" ) as SavedImportSettings;

					if ( loadedSettings == null )
					{
						loadedSettings = ScriptableObject.CreateInstance<SavedImportSettings>();
						string dataPath = bToolsResources.PathTo_bData;

						AssetDatabase.CreateAsset( loadedSettings, dataPath + "/ImportSettingsPresets.asset" );
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}

					m_importSettingsData = loadedSettings;
				}

				return m_importSettingsData;
			}
		}

		private void OnPreprocessModel()
		{
			// Make sure Editor is not compiling
			if ( EditorApplication.isCompiling ) return;

			//Make sure there are at least one settings file
			if ( importSettingsData.meshImportSettingsList.Count <= 0 ) return;

			//Check config conditions
			for ( int i = 0 ; i < importSettingsData.meshImportSettingsList.Count ; i++ )
			{
				if ( FilterTest( importSettingsData.meshImportSettingsList[i] ) )
				{
					//Check that the asset doesn't already exist (Apply triggers this script)
					if ( AssetDatabase.LoadAssetAtPath<Object>( assetPath ) == null )
					{
						ApplyMeshConfig( importSettingsData.meshImportSettingsList[i] );
						break;
					}
				}
			}
		}

		private void OnPreprocessAudio()
		{
			// Make sure Editor is not compiling
			if ( EditorApplication.isCompiling ) return;

			//Make sure there are at least one settings file
			if ( importSettingsData.audioImportSettingsList.Count <= 0 ) return;

			for ( int i = 0 ; i < importSettingsData.audioImportSettingsList.Count ; i++ )
			{
				if ( FilterTest( importSettingsData.audioImportSettingsList[i] ) )
				{
					//Check that the asset doesn't already exist (Apply triggers this script)
					if ( AssetDatabase.LoadAssetAtPath<Object>( assetPath ) == null )
					{
						ApplyAudioConfig( importSettingsData.audioImportSettingsList[i] );
						break;
					}
				}
			}
		}

		private void OnPreprocessTexture()
		{
			// Make sure Editor is not compiling
			if ( EditorApplication.isCompiling ) return;

			//Make sure there are at least one settings file
			if ( importSettingsData.textureImportSettingsList.Count <= 0 ) return;

			//Check config conditions
			for ( int i = 0 ; i < importSettingsData.textureImportSettingsList.Count ; i++ )
			{
				if ( FilterTest( importSettingsData.textureImportSettingsList[i] ) )
				{
					//Check that the asset doesn't already exist (Apply triggers this script)
					if ( AssetDatabase.LoadAssetAtPath<Object>( assetPath ) == null )
					{
						ApplyTextureConfig( importSettingsData.textureImportSettingsList[i] );
						break;
					}
				}
			}
		}

		private void ApplyMeshConfig( MeshImportConfig cfg )
		{
			ModelImporter importer = assetImporter as ModelImporter;
			//Meshes
			importer.globalScale = cfg.ScaleFactor;
			importer.meshCompression = cfg.MeshCompression;
			importer.isReadable = cfg.ReadWriteEnabled;
			importer.optimizeMesh = cfg.OptimizeMesh;
			importer.importBlendShapes = cfg.ImportBlendShapes;
			importer.addCollider = cfg.GenerateColliders;
			importer.keepQuads = cfg.KeepQuads;
			importer.weldVertices = cfg.WeldVertices;
			importer.importVisibility = cfg.ImportVisibility;
			importer.importCameras = cfg.ImportCameras;
			importer.importLights = cfg.ImportLights;
			importer.swapUVChannels = cfg.SwapUVs;
			importer.generateSecondaryUV = cfg.GenerateLightmapUV;
			if ( cfg.GenerateLightmapUV )
			{
				importer.secondaryUVHardAngle = cfg.LightmapHardAngle;
				importer.secondaryUVPackMargin = cfg.LightmapPadding;
				importer.secondaryUVAreaDistortion = cfg.LightmapAreaError;
				importer.secondaryUVAngleDistortion = cfg.LightmapAngleError;
			}

			//Normals/tangent
			importer.importNormals = cfg.ImportNormals;
			if ( cfg.ImportNormals == ModelImporterNormals.Calculate )
			{
				importer.normalCalculationMode = cfg.NormalCalculationMode;
				importer.normalSmoothingAngle = cfg.SmoothingAngle;
			}

			importer.importTangents = cfg.TangentMode;

			//Mats
			importer.importMaterials = cfg.ImportMats;
			importer.materialName = cfg.MaterialNaming;
			importer.materialSearch = cfg.MaterialSearch;

			//Animation
			importer.importAnimation = cfg.ImportAnim;

			if ( cfg.ForceNoRig ) importer.animationType = ModelImporterAnimationType.None;
		}

		private void ApplyAudioConfig( AudioImportConfig cfg )
		{
			AudioImporter importer = assetImporter as AudioImporter;

			importer.forceToMono = cfg.ForceToMono;
			importer.loadInBackground = cfg.LoadInBackground;
			importer.preloadAudioData = cfg.PreloadAudioData;

			AudioImporterSampleSettings otherSettings = new AudioImporterSampleSettings()
			{
				compressionFormat = cfg.AudioCompressionFormat,
				loadType = cfg.AudioClipLoadType,
				quality = (float)cfg.AudioQuality / 100f,
				sampleRateSetting = cfg.SampleRateSetting,
			};

			if ( cfg.SampleRateSetting == AudioSampleRateSetting.OverrideSampleRate )
			{
				otherSettings.sampleRateOverride = (uint)cfg.SampleRateOverride;
			}

			importer.defaultSampleSettings = otherSettings;

		}

		private void ApplyTextureConfig( TextureImportConfig cfg )
		{
			TextureImporter importer = assetImporter as TextureImporter;
			TextureImporterSettings importSettings = new TextureImporterSettings();

			// Common parameters.
			importSettings.textureType = (TextureImporterType)cfg.TextureType;
			importSettings.textureShape = cfg.TextureShape;

			importSettings.readable = cfg.ReadWriteEnabled;

			importSettings.mipmapEnabled = cfg.GenerateMipMaps;
			if ( cfg.GenerateMipMaps )
			{
				//importer.mipMapBias
				importSettings.borderMipmap = cfg.BorderMipMaps;
				importSettings.mipmapFilter = cfg.MipFilter;
				importSettings.fadeOut = cfg.FadeoutMipMaps;

				if ( cfg.FadeoutMipMaps )
				{
					importSettings.mipmapFadeDistanceStart = (int)cfg.MipFadeDistanceStart;
					importSettings.mipmapFadeDistanceEnd = (int)cfg.MipFadeDistanceEnd;
				}
			}

			// sRGB.
			if ( cfg.TextureType == TextureImportConfig.FixedTextureType.Default || cfg.TextureType == TextureImportConfig.FixedTextureType.Sprite )
			{
				importSettings.sRGBTexture = cfg.sRGB;
			}

			// Cubemap.
			if ( cfg.TextureShape == TextureImporterShape.TextureCube )
			{
				importSettings.generateCubemap = cfg.GenerateCubemap;
			}
			// NormalMaps.
			if ( cfg.TextureType == TextureImportConfig.FixedTextureType.NormalMap )
			{
				importer.convertToNormalmap = cfg.NormalFromGrayscale;
				if ( cfg.NormalFromGrayscale )
				{
					importSettings.heightmapScale = cfg.NormalBumpiness;
					importSettings.normalMapFilter = cfg.NormalFilter;
				}
			}

			// Alpha source.
			if ( cfg.TextureType != TextureImportConfig.FixedTextureType.NormalMap && cfg.TextureType != TextureImportConfig.FixedTextureType.LightMap )
			{
				importSettings.alphaSource = cfg.AlphaSource;
				if ( cfg.AlphaSource != TextureImporterAlphaSource.None )
				{
					importSettings.alphaIsTransparency = cfg.AlphaIsTransparency;
				}
			}

			// Sprites.
			if ( cfg.TextureType != TextureImportConfig.FixedTextureType.Sprite )
			{
				importSettings.npotScale = cfg.NpotScale;
			}
			else
			{
				importSettings.spriteMode = (int)cfg.SpriteImportMode;
				importSettings.spritePixelsPerUnit = cfg.PixelsPerUnit;
				importSettings.spriteAlignment = (int)cfg.SpriteAlignment;
				if ( cfg.SpriteAlignment == SpriteAlignment.Custom )
				{
					importSettings.spritePivot = cfg.SpritePivot;
				}
				importSettings.spriteExtrude = (uint)cfg.ExtrudeEdges;
				importer.spritePackingTag = cfg.PackingTag;
				importSettings.spriteMeshType = cfg.SpriteMeshType;
				//importer.spriteBorder
			}

			// Default platform settings.
			importSettings.wrapMode = cfg.WrapMode;
			if ( cfg.WrapMode_U != cfg.WrapMode_V )
			{
				importSettings.wrapModeU = cfg.WrapMode_U;
				importSettings.wrapModeV = cfg.WrapMode_V;
			}
			importSettings.filterMode = cfg.FilterMode;
			importSettings.aniso = cfg.AnisoLevel;
			importer.maxTextureSize = (int)cfg.MaxTextureSize;
			importer.textureCompression = cfg.Compression;
			importer.crunchedCompression = cfg.UseCrunchCompression;
			importer.compressionQuality = cfg.CompressionQuality;

			importer.SetTextureSettings( importSettings );
		}

		private bool FilterTest( ImportConfigBase cfg )
		{
			if ( !cfg.isEnabled ) return false;

			bool pathTest = false, filenameTest = false;

			string fileName = Path.GetFileName( assetPath );
			string pathNoFilename = Path.GetDirectoryName( assetPath );

			// Split string into multiple filters
			string[] pathFilterSplit = cfg.pathNameFilter.Split( filterSep, System.StringSplitOptions.RemoveEmptyEntries );
			string[] filenameFilterSplit = cfg.fileNameFilter.Split( filterSep, System.StringSplitOptions.RemoveEmptyEntries );

			//Check if filter is empty
			if ( cfg.pathNameFilter.Length == 0 )
			{
				pathTest = true;
			}
			if ( cfg.fileNameFilter.Length == 0 )
			{
				filenameTest = true;
			}

			// Check if path contains what we want
			for ( int i = 0 ; i < pathFilterSplit.Length ; i++ )
			{
				if ( pathNoFilename.Contains( pathFilterSplit[i] ) )
				{
					pathTest = true;
				}
			}

			// Check if filename contains what we want
			for ( int i = 0 ; i < filenameFilterSplit.Length ; i++ )
			{
				// Get last item in path split, aka filename
				if ( fileName.Contains( filenameFilterSplit[i] ) )
				{
					filenameTest = true;
				}
			}

			return pathTest && filenameTest;
		}
	}
}