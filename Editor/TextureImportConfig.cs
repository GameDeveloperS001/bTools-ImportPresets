using UnityEngine;
using UnityEditor;
using bTools.CodeExtensions;

namespace bTools.ImportPresets
{
    [System.Serializable]
    public class TextureImportConfig : ImportConfigBase
    {
        public enum MaxTextureSizeEnum
        {
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096,
            _8192 = 8192
        }
        public enum FixedTextureType
        {
            Default = 0,
            NormalMap = 1,
            EditorGUIAndLegacyGUI = 2,
            Sprite = 8,
            Cursor = 7,
            Cookie = 4,
            LightMap = 6,
            SingleChannel = 10
        }

        #region SavedSettings
        [SerializeField] public FixedTextureType TextureType = FixedTextureType.Default;
        [SerializeField] public TextureImporterShape TextureShape = TextureImporterShape.Texture2D;
        //Cube map settings
        [SerializeField] public TextureImporterGenerateCubemap GenerateCubemap;
        [SerializeField] public TextureImporterCubemapConvolution CubemapConvolution;
        [SerializeField] public bool FixupEdgeSeams;
        //Default texture settings
        [SerializeField] public bool sRGB;
        [SerializeField] public TextureImporterAlphaSource AlphaSource;
        [SerializeField] public bool AlphaIsTransparency;
        [SerializeField] public TextureImporterNPOTScale NpotScale;
        [SerializeField] public bool ReadWriteEnabled;
        [SerializeField] public bool GenerateMipMaps;
        [SerializeField] public bool BorderMipMaps;
        [SerializeField] public TextureImporterMipFilter MipFilter;
        [SerializeField] public bool FadeoutMipMaps;
        [SerializeField] public float MipFadeDistanceStart;
        [SerializeField] public float MipFadeDistanceEnd;
        [SerializeField] public TextureWrapMode WrapMode;
        [SerializeField] public TextureWrapMode WrapMode_U;
        [SerializeField] public TextureWrapMode WrapMode_V;
        [SerializeField] public FilterMode FilterMode;
        [SerializeField] public int AnisoLevel = 1;
        [SerializeField] public MaxTextureSizeEnum MaxTextureSize = MaxTextureSizeEnum._2048;
        [SerializeField] public TextureImporterCompression Compression;
        [SerializeField] public int CompressionQuality;
        [SerializeField] public bool UseCrunchCompression;
        //Normal maps
        [SerializeField] public bool NormalFromGrayscale;
        [SerializeField] public float NormalBumpiness;
        [SerializeField] public TextureImporterNormalFilter NormalFilter;
        //Sprite 		 
        [SerializeField] public SpriteImportMode SpriteImportMode;
        [SerializeField] public string PackingTag;
        [SerializeField] public float PixelsPerUnit;
        [SerializeField] public SpriteMeshType SpriteMeshType;
        [SerializeField] public int ExtrudeEdges;
        [SerializeField] public SpriteAlignment SpriteAlignment;
        [SerializeField] public Vector2 SpritePivot;
        //Cookie
        [SerializeField] public LightType LightType;

        #endregion

        // public override void DrawInnerGUI(Rect area)
        // {
        //     //DrawFilterGUI();

        //     EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
        //     EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
        //     GUILayout.Space(3);

        //     TextureType = (FixedTextureType)EditorGUILayout.EnumPopup("Texture Type", TextureType);
        //     bool nextFieldIsDisabled = false;
        //     nextFieldIsDisabled = (TextureType == FixedTextureType.Default || TextureType == FixedTextureType.SingleChannel) ? false : true;
        //     EditorGUI.BeginDisabledGroup(nextFieldIsDisabled);
        //     TextureShape = (TextureImporterShape)EditorGUILayout.EnumPopup("Texture Shape", TextureShape);
        //     EditorGUI.EndDisabledGroup();

        //     if (nextFieldIsDisabled)
        //         TextureShape = TextureImporterShape.Texture2D;

        //     GUILayout.Space(6);

        //     //Cubemap parameters
        //     if (TextureShape == TextureImporterShape.TextureCube)
        //     {
        //         GenerateCubemap = (TextureImporterGenerateCubemap)EditorGUILayout.EnumPopup("Mapping", GenerateCubemap);
        //         EditorGUI.indentLevel++;
        //         if (TextureType == FixedTextureType.Default)
        //         {
        //             CubemapConvolution = (TextureImporterCubemapConvolution)EditorGUILayout.EnumPopup("Convolution Type", CubemapConvolution);
        //         }
        //         FixupEdgeSeams = EditorGUILayout.Toggle("Fixup Edge Seams", FixupEdgeSeams);
        //         EditorGUI.indentLevel--;
        //         GUILayout.Space(6);
        //     }

        //     //Default type settings
        //     if (TextureType == FixedTextureType.Default || TextureType == FixedTextureType.Sprite)
        //     {
        //         sRGB = EditorGUILayout.Toggle("sRGB (Color Texture)", sRGB);
        //     }


        //     //Normal map
        //     if (TextureType == FixedTextureType.NormalMap)
        //     {
        //         NormalFromGrayscale = EditorGUILayout.Toggle("Create from Grayscale", NormalFromGrayscale);
        //         if (NormalFromGrayscale)
        //         {
        //             NormalBumpiness = EditorGUILayout.Slider("Bumpiness", NormalBumpiness, 0f, 0.3f);
        //             NormalFilter = (TextureImporterNormalFilter)EditorGUILayout.EnumPopup("Filtering", NormalFilter);
        //         }
        //     }

        //     //Sprite
        //     if (TextureType == FixedTextureType.Sprite)
        //     {
        //         SpriteImportMode = (SpriteImportMode)EditorGUILayout.EnumPopup("Sprite Mode", SpriteImportMode);
        //         if (SpriteImportMode == SpriteImportMode.None)
        //             SpriteImportMode = SpriteImportMode.Single;
        //         PackingTag = EditorGUILayout.TextField("Packing Tag", PackingTag);
        //         PixelsPerUnit = EditorGUILayout.FloatField("Pixels per Unit", PixelsPerUnit);
        //         SpriteMeshType = (SpriteMeshType)EditorGUILayout.EnumPopup("Mesh Type", SpriteMeshType);
        //         ExtrudeEdges = EditorGUILayout.IntSlider("Extrude Edges", ExtrudeEdges, 0, 32);
        //         if (SpriteImportMode == SpriteImportMode.Single)
        //         {
        //             SpriteAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot", SpriteAlignment);
        //             if (SpriteAlignment == SpriteAlignment.Custom)
        //             {
        //                 SpritePivot = EditorGUILayout.Vector2Field(string.Empty, SpritePivot);
        //             }
        //         }
        //     }

        //     //Cookie
        //     if (TextureType == FixedTextureType.Cookie)
        //     {
        //         //LightType = (LightType)EditorGUILayout.EnumPopup( "Light Type", LightType ); //parameter not exposed in importer

        //         // Following code is unity behaviour but makes asset crash.
        //         //if ( LightType == LightType.Point )
        //         //{
        //         //    TextureShape = TextureImporterShape.TextureCube;
        //         //}
        //         //else
        //         //{
        //         //    TextureShape = TextureImporterShape.Texture2D;
        //         //}
        //         //if ( LightType == LightType.Area )
        //         //{
        //         //    EditorGUILayout.HelpBox( "Area is not supported", MessageType.Error );
        //         //}
        //     }

        //     //ADVANCED PARAMETERS
        //     GUILayout.Space(6);
        //     EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
        //     EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
        //     GUILayout.Space(3);

        //     if (TextureType != FixedTextureType.NormalMap && TextureType != FixedTextureType.LightMap)
        //     {
        //         AlphaSource = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup("Alpha Source", AlphaSource);

        //         nextFieldIsDisabled = (AlphaSource == TextureImporterAlphaSource.None) ? true : false;
        //         EditorGUI.BeginDisabledGroup(nextFieldIsDisabled);
        //         AlphaIsTransparency = EditorGUILayout.Toggle("Alpha Is Transparency", AlphaIsTransparency);
        //         EditorGUI.EndDisabledGroup();
        //     }

        //     if (TextureType != FixedTextureType.Sprite)
        //     {
        //         NpotScale = (TextureImporterNPOTScale)EditorGUILayout.EnumPopup("Non Power of 2", NpotScale);
        //     }

        //     ReadWriteEnabled = EditorGUILayout.Toggle("Read/Write Enabled", ReadWriteEnabled);
        //     GenerateMipMaps = EditorGUILayout.Toggle("Generate Mip Maps", GenerateMipMaps);

        //     if (GenerateMipMaps)
        //     {
        //         EditorGUI.indentLevel++;
        //         BorderMipMaps = EditorGUILayout.Toggle("Border Mip Maps", BorderMipMaps);
        //         MipFilter = (TextureImporterMipFilter)EditorGUILayout.EnumPopup("Non Power of 2", MipFilter);
        //         FadeoutMipMaps = EditorGUILayout.Toggle("Fadeout Mip Maps", FadeoutMipMaps);
        //         if (FadeoutMipMaps)
        //         {
        //             EditorGUILayout.MinMaxSlider("Fade Range", ref MipFadeDistanceStart, ref MipFadeDistanceEnd, 0, 10);
        //         }

        //         EditorGUI.indentLevel--;
        //     }

        //     //Final common settings
        //     GUILayout.Space(6);
        //     WrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Wrap Mode", WrapMode);
        //     EditorGUI.indentLevel++;
        //     WrapMode_U = (TextureWrapMode)EditorGUILayout.EnumPopup("U axis", WrapMode_U);
        //     WrapMode_V = (TextureWrapMode)EditorGUILayout.EnumPopup("V axis", WrapMode_V);
        //     EditorGUI.indentLevel--;
        //     FilterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", FilterMode);
        //     nextFieldIsDisabled = FilterMode == FilterMode.Point
        //         || TextureShape == TextureImporterShape.TextureCube
        //         || TextureType != FixedTextureType.Default && TextureType != FixedTextureType.NormalMap ? true : false;
        //     EditorGUI.BeginDisabledGroup(nextFieldIsDisabled);
        //     AnisoLevel = EditorGUILayout.IntSlider("Aniso Level", AnisoLevel, 0, 16);
        //     EditorGUI.EndDisabledGroup();

        //     GUILayout.Space(6);
        //     EditorGUILayout.LabelField("Default Platform Settings", EditorStyles.boldLabel);
        //     EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Colors.DarkGrayX11);
        //     GUILayout.Space(3);

        //     MaxTextureSize = (MaxTextureSizeEnum)EditorGUILayout.EnumPopup("Max Size", MaxTextureSize);
        //     Compression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", Compression);

        //     if (Compression != TextureImporterCompression.Uncompressed)
        //     {
        //         UseCrunchCompression = EditorGUILayout.Toggle("Use Crunch Compression", UseCrunchCompression);
        //         if (UseCrunchCompression)
        //         {
        //             CompressionQuality = EditorGUILayout.IntSlider("Compression", CompressionQuality, 0, 100);
        //         }
        //     }
        // }
    }
}