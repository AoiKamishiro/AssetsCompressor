/*
 *  Zlib License
 *  
 *  Copyright (c) 2020 AoiKamishiro/神城葵
 *  
 *  
 *  This software is provided 'as-is', without any express or implied warranty.
 *  In no event will the authors be held liable for any damages arising from the use of this software.
 *  
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 *  
 *     1. The origin of this software must not be misrepresented; you must not
 *           claim that you wrote the original software. If you use this software
 *           in a product, an acknowledgment in the product documentation would be
 *           appreciated but is not required.
 *           
 *     2. Altered source versions must be plainly marked as such, and must not be
 *          misrepresented as being the original software.
 *          
 *     3. This notice may not be removed or altered from any source distribution. 
 *     
 */

//LastUpdate:2020/07/24(JST) 

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AKAssetsCompressor : EditorWindow
{
    [MenuItem("Tools/Kamishiro/AssetsCompressor", priority = 150)]
    private static void OnEnable()
    {
        AKAssetsCompressor window = GetWindow<AKAssetsCompressor>("AssetsCompressor");
        window.minSize = new Vector2(320, 360);
        window.Show();
    }

    //GUI Component
    private const string version = "AssetCompresser V1.3.1 forked by SHIMAKI Ayako";
    private const string linktext = "オリジナル様はこちら";
    private const string link = "https://github.com/AoiKamishiro/UnityCustomEditor_AssetsCompressor";
    private int toolberSelection = 0;

    private Vector2 texture_ScrollPosition = Vector2.zero;
    private bool texture_ShowOPDirectory = true;
    private bool texture_ShowOPExtension = false;
    private string[] texture_Directories = new string[] { };
    private bool[] texture_DirectoriesBool = new bool[] { };
    private bool texture_EditStreaminMipMap = true;
    private bool texture_EditTextureCompressionQuality = false;
    private bool texture_EditClunchCompression = true;
    private bool texture_ShowOPDefault = true;
    private bool texture_ShowOPNoralmap = true;
    private bool texture_ShowOPUISprite = true;

    private Vector2 model_ScrollPosition = Vector2.zero;
    private bool model_ShowOPDirectory = true;
    private bool model_ShowOPExtension = false;
    private string[] model_Directories = new string[] { };
    private bool[] model_DirectoriesBool = new bool[] { };
    private bool model_EditScene = true;
    private bool model_EditMeshes = true;
    private bool model_EditGeometry = false;

    private Vector2 audio_ScrollPosition = Vector2.zero;
    private bool audio_ShowOPDirectory = true;
    private bool audio_ShowOPExtension = false;
    private string[] audio_Directories = new string[] { };
    private bool[] audio_DirectoriesBool = new bool[] { };
    private bool audio_EditGeneral = true;
    private bool audio_EditCompression = true;

    //Default Settings
    private bool skipcompresion = true;
    private bool useStreamingMipMap = true;
    private int textureCompressionQuality = 0;
    private bool useCrunchCompression = true;
    private int compressionQaulity = 100;
    private bool UseMaxsizeAdjuster = true;
    private bool useNormalmapOp = true;

    private bool importCameras = false;
    private bool importLights = false;
    private bool readWriteEnabled = true;
    private bool optimizeMesh = true;
    private bool generateLightmapUVs = true;

    private bool forceToMono = true;
    private int quality = 100;

    //File Extensions
    private bool jpg = true;
    private bool png = true;
    private bool tif = true;
    private bool tga = true;
    private bool psd = true;
    private bool bmp = true;
    private bool gif = false;
    private bool fbx = true;
    private bool obj = true;
    private bool blend = true;
    private bool mp3 = true;
    private bool ogg = true;

    //Textures Enums
    private enum MAXSIZE
    {
        MaxSize32x32 = 0,
        MaxSize64x64 = 1,
        MaxSize128x128 = 2,
        MaxSize256x256 = 3,
        MaxSize512x512 = 4,
        MaxSize1024x1024 = 5,
        MaxSize2048x2048 = 6,
        MaxSize4096x4096 = 7,
        MaxSize8192x8192 = 8
    }
    private MAXSIZE defaultMaxsize = MAXSIZE.MaxSize2048x2048;
    private MAXSIZE normalMaxsize = MAXSIZE.MaxSize1024x1024;
    private MAXSIZE spriteMaxsize = MAXSIZE.MaxSize256x256;
    private int defaultMaxsizeInt = 2048;
    private int normalMaxsizeInt = 1024;
    private int spriteMaxsizeInt = 256;

    private static GUIContent[] TEXTURE_COMPRESSION_QUALITY_OPTIONS =
    {
        new GUIContent("None"),
        new GUIContent("Low Quality"),
        new GUIContent("Normal Quality"),
        new GUIContent("High Quality")
    };

    //Models Enums
    private enum COMPRESS
    {
        OFF = 0,
        Low = 1,
        Medium = 2,
        Heigh = 3
    }
    private COMPRESS compression = COMPRESS.Low;

    //Audios Enums
    private enum FORMAT
    {
        PCM = 0,
        Vorbis = 1,
        ADPCM = 2
    }
    private enum RATETYPE
    {
        PreserveSampleRate = 0,
        OptimizeSampleRate = 1,
        OverrideSampleRate = 2
    }
    private enum RATE
    {
        SampleRate8000Hz = 0,
        SampleRate11025Hz = 1,
        SampleRate22050Hz = 2,
        SampleRate44100Hz = 3,
        SampleRate48000Hz = 4,
        SampleRate96000Hz = 5,
        SampleRate192000Hz = 6
    }
    private FORMAT format = FORMAT.Vorbis;
    private RATETYPE sampleRateSetting = RATETYPE.OptimizeSampleRate;
    private RATE sampleRate = RATE.SampleRate44100Hz;


    private void OnGUI()
    {
        //Listup Directories
        string[] directories = new string[] { Application.dataPath };
        directories = directories.Concat(Directory.GetDirectories(Application.dataPath)).ToArray();
        if (directories.Length != texture_Directories.Length)
        {
            texture_Directories = new string[] { };
            model_Directories = new string[] { };
            audio_Directories = new string[] { };
            texture_DirectoriesBool = new bool[] { };
            model_DirectoriesBool = new bool[] { };
            audio_DirectoriesBool = new bool[] { };
            foreach (string directory in directories)
            {
                texture_Directories = texture_Directories.Concat(new string[] { directory }).ToArray();
                model_Directories = model_Directories.Concat(new string[] { directory }).ToArray();
                audio_Directories = audio_Directories.Concat(new string[] { directory }).ToArray();
                texture_DirectoriesBool = texture_DirectoriesBool.Concat(new bool[] { false }).ToArray();
                model_DirectoriesBool = model_DirectoriesBool.Concat(new bool[] { false }).ToArray();
                audio_DirectoriesBool = audio_DirectoriesBool.Concat(new bool[] { false }).ToArray();
            }
        }

        //Editor Window
        using (new GUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField(version);
            if (GUILayout.Button(linktext)) { OpenLink(link); }

            EditorGUILayout.Space();

            toolberSelection = GUILayout.Toolbar(toolberSelection, new string[] { "Texture", "Model", "Audio" });

            if (toolberSelection == 0)
            {
                texture_ScrollPosition = EditorGUILayout.BeginScrollView(texture_ScrollPosition);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Texture インポート設定", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Low Quality")) { SetLowQuality();  }
                if (GUILayout.Button("High Quality")) { SetHighQuality(); }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                skipcompresion = EditorGUILayout.ToggleLeft("既にCrunchCompression済みのファイルはスキップ", skipcompresion);
                EditorGUILayout.Space();
                texture_ShowOPDirectory = EditorGUILayout.Foldout(texture_ShowOPDirectory, "対象フォルダ");
                if (texture_ShowOPDirectory)
                {
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("全部にチェックを入れる")) { CheckOn_AllFolders(); }
                    for (int i = 0; i < texture_Directories.Length; i++)
                    {
                        string dirname = i == 0 ? "./" : "./" + Path.GetFileName(texture_Directories[i]);
                        texture_DirectoriesBool[i] = EditorGUILayout.ToggleLeft(dirname, texture_DirectoriesBool[i]);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                texture_ShowOPExtension = EditorGUILayout.Foldout(texture_ShowOPExtension, "対象ファイル（拡張子別）");
                if (texture_ShowOPExtension)
                {
                    EditorGUI.indentLevel++;
                    jpg = EditorGUILayout.ToggleLeft("JPEG (.jpg, .jpeg)", jpg);
                    png = EditorGUILayout.ToggleLeft("PNG (.png)", png);
                    bmp = EditorGUILayout.ToggleLeft("BMP (.bmp)", bmp);
                    psd = EditorGUILayout.ToggleLeft("PSD (.psd)", psd);
                    tga = EditorGUILayout.ToggleLeft("TGA (.tga)", tga);
                    tif = EditorGUILayout.ToggleLeft("TIFF (.tif, .tiff)", tif);
                    gif = EditorGUILayout.ToggleLeft("GIF (.gif)", gif);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                //Edit Mip Maps Setting
                texture_EditStreaminMipMap = EditorGUILayout.BeginToggleGroup("Mip Maps の設定を変更する", texture_EditStreaminMipMap);
                EditorGUI.indentLevel++;
                useStreamingMipMap = EditorGUILayout.ToggleLeft("Streaming Mip Maps", useStreamingMipMap);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();

                //Edit Compression Setting
                texture_EditTextureCompressionQuality = EditorGUILayout.BeginToggleGroup("Compression の設定を変更する", texture_EditTextureCompressionQuality);
                EditorGUI.indentLevel++;
                textureCompressionQuality = EditorGUILayout.Popup(textureCompressionQuality, TEXTURE_COMPRESSION_QUALITY_OPTIONS);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();

                //Edit CrunchCompression Setting
                texture_EditClunchCompression = EditorGUILayout.BeginToggleGroup("Crunch Compression の設定を変更する", texture_EditClunchCompression);
                EditorGUI.indentLevel++;
                useCrunchCompression = EditorGUILayout.ToggleLeft("Use Crunch Compresion", useCrunchCompression);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("圧縮品質", GUILayout.Width(80));
                    compressionQaulity = (int)EditorGUILayout.Slider(compressionQaulity, 0, 100);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();

                //Edit Maxsize Setting
                UseMaxsizeAdjuster = EditorGUILayout.BeginToggleGroup("MaxSize の設定を変更する", UseMaxsizeAdjuster);
                EditorGUI.indentLevel++;
                texture_ShowOPDefault = EditorGUILayout.Foldout(texture_ShowOPDefault, "Default Texture");
                if (texture_ShowOPDefault)
                {
                    EditorGUI.indentLevel++;
                    defaultMaxsize = (MAXSIZE)EditorGUILayout.EnumPopup("MaxSizeの最大値", defaultMaxsize);
                    EditorGUI.indentLevel--;
                }
                texture_ShowOPNoralmap = EditorGUILayout.Foldout(texture_ShowOPNoralmap, "NormalMap Texture");
                if (texture_ShowOPNoralmap)
                {
                    EditorGUI.indentLevel++;
                    normalMaxsize = (MAXSIZE)EditorGUILayout.EnumPopup("MaxSizeの最大値", normalMaxsize);
                    useNormalmapOp = EditorGUILayout.ToggleLeft("MaxSizeを一段階下げる", useNormalmapOp);
                    EditorGUI.indentLevel--;
                }
                texture_ShowOPUISprite = EditorGUILayout.Foldout(texture_ShowOPUISprite, "Sprite Texture");
                if (texture_ShowOPUISprite)
                {
                    EditorGUI.indentLevel++;
                    spriteMaxsize = (MAXSIZE)EditorGUILayout.EnumPopup("MaxSizeの最大値", spriteMaxsize);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.Space();
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("設定の一括変更")) { BatchProcess_Texture(); }
                EditorGUILayout.Space();
            }
            if (toolberSelection == 1)
            {
                model_ScrollPosition = EditorGUILayout.BeginScrollView(model_ScrollPosition);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Model インポート設定", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                model_ShowOPDirectory = EditorGUILayout.Foldout(model_ShowOPDirectory, "対象フォルダ");
                if (model_ShowOPDirectory)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < model_Directories.Length; i++)
                    {
                        string dirname = i == 0 ? "./" : "./" + Path.GetFileName(texture_Directories[i]);
                        model_DirectoriesBool[i] = EditorGUILayout.ToggleLeft(dirname, model_DirectoriesBool[i]);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                model_ShowOPExtension = EditorGUILayout.Foldout(model_ShowOPExtension, "対象ファイル（拡張子別）");
                if (model_ShowOPExtension)
                {
                    EditorGUI.indentLevel++;
                    fbx = EditorGUILayout.ToggleLeft("FBX (.fbx)", fbx);
                    obj = EditorGUILayout.ToggleLeft("OBJ (.obj)", obj);
                    blend = EditorGUILayout.ToggleLeft("Blend (.blend)", blend);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                //Edit Scene Setting
                model_EditScene = EditorGUILayout.BeginToggleGroup("Scene の設定を変更する", model_EditScene);
                EditorGUI.indentLevel++;
                importCameras = EditorGUILayout.ToggleLeft("Import Cameras", importCameras);
                importLights = EditorGUILayout.ToggleLeft("Import Lights", importLights);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();

                //Edit Meshes Setting
                model_EditMeshes = EditorGUILayout.BeginToggleGroup("Meshes の設定を変更する", model_EditMeshes);
                EditorGUI.indentLevel++;
                compression = (COMPRESS)EditorGUILayout.EnumPopup("Mesh Compression", compression);
                readWriteEnabled = EditorGUILayout.ToggleLeft("Read/Write Enabled", readWriteEnabled);
                optimizeMesh = EditorGUILayout.ToggleLeft("Optimize Mesh", optimizeMesh);
                //generateColliders = EditorGUILayout.ToggleLeft("Generate Colliders", generateColliders);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();

                //Edit Geometry Setting
                model_EditGeometry = EditorGUILayout.BeginToggleGroup("Geometry の設定を変更する", model_EditGeometry);
                EditorGUI.indentLevel++;
                generateLightmapUVs = EditorGUILayout.ToggleLeft("Generate Lightmap UVs", generateLightmapUVs);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();


                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("設定の一括変更")) { BatchProcess_Model(); }
                EditorGUILayout.Space();
            }
            if (toolberSelection == 2)
            {
                audio_ScrollPosition = EditorGUILayout.BeginScrollView(audio_ScrollPosition);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Audio インポート設定", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                audio_ShowOPDirectory = EditorGUILayout.Foldout(audio_ShowOPDirectory, "対象フォルダ");
                if (audio_ShowOPDirectory)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < audio_Directories.Length; i++)
                    {
                        string dirname = i == 0 ? "./" : "./" + Path.GetFileName(texture_Directories[i]);
                        audio_DirectoriesBool[i] = EditorGUILayout.ToggleLeft(dirname, audio_DirectoriesBool[i]);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                audio_ShowOPExtension = EditorGUILayout.Foldout(audio_ShowOPExtension, "対象ファイル（拡張子別）");
                if (audio_ShowOPExtension)
                {
                    EditorGUI.indentLevel++;
                    mp3 = EditorGUILayout.ToggleLeft("MP3 (.mp3)", mp3);
                    ogg = EditorGUILayout.ToggleLeft("Ogg (.ogg)", ogg);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                //Edit General Setting
                audio_EditGeneral = EditorGUILayout.BeginToggleGroup("General の設定を変更する", audio_EditGeneral);
                EditorGUI.indentLevel++;
                forceToMono = EditorGUILayout.ToggleLeft("Force to Mono", forceToMono);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();

                //Edit Compressionl Setting
                audio_EditCompression = EditorGUILayout.BeginToggleGroup("Compresion の設定を変更する", audio_EditCompression);
                EditorGUI.indentLevel++;
                format = (FORMAT)EditorGUILayout.EnumPopup("Compression Format", format);
                if (format == FORMAT.Vorbis)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Qaulity", GUILayout.Width(80));
                        quality = (int)EditorGUILayout.Slider(quality, 0, 100);
                    }
                }
                sampleRateSetting = (RATETYPE)EditorGUILayout.EnumPopup("Sample Rate Setting", sampleRateSetting);
                if (sampleRateSetting == RATETYPE.OverrideSampleRate)
                {
                    sampleRate = (RATE)EditorGUILayout.EnumPopup("Sample Rate", sampleRate);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("設定の一括変更")) { BatchProcess_Audio(); }
                EditorGUILayout.Space();
            }
        }
    }

    private void CheckOn_AllFolders()
    {
        for (int i = 0; i < texture_DirectoriesBool.Length; i++)
        {
            texture_DirectoriesBool[i] = true;
        }
    }

    private void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
    private void BatchProcess_Texture()
    {
        int editedFilesCount = 0;
        TextureImporterCompression tic = ConvertTextureImporterCompressionEnum();
        defaultMaxsizeInt = ConvertTextMaxsizeEnum(defaultMaxsize);
        normalMaxsizeInt = ConvertTextMaxsizeEnum(normalMaxsize);
        spriteMaxsizeInt = ConvertTextMaxsizeEnum(spriteMaxsize);

        string[] files = new string[] { };
        for (int i = 0; i < texture_Directories.Length; i++)
        {
            if (texture_DirectoriesBool[i])
            {
                if (i == 0)
                {
                    if (bmp) { files = files.Concat(GetRootAssetsListByExt(".bmp")).ToArray(); }
                    if (gif) { files = files.Concat(GetRootAssetsListByExt(".gif")).ToArray(); }
                    if (jpg) { files = files.Concat(GetRootAssetsListByExt(".jpg")).Concat(GetRootAssetsListByExt(".jpeg")).ToArray(); }
                    if (png) { files = files.Concat(GetRootAssetsListByExt(".png")).ToArray(); }
                    if (psd) { files = files.Concat(GetRootAssetsListByExt(".psd")).ToArray(); }
                    if (tga) { files = files.Concat(GetRootAssetsListByExt(".tga")).ToArray(); }
                    if (tif) { files = files.Concat(GetRootAssetsListByExt(".tif")).Concat(GetRootAssetsListByExt(".tiff")).ToArray(); }
                }
                else
                {
                    if (bmp) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".bmp")).ToArray(); }
                    if (gif) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".gif")).ToArray(); }
                    if (jpg) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".jpg")).Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".jpeg")).ToArray(); }
                    if (png) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".png")).ToArray(); }
                    if (psd) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".psd")).ToArray(); }
                    if (tga) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".tga")).ToArray(); }
                    if (tif) { files = files.Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".tif")).Concat(GetAssetsListByExt(texture_Directories[i] + "/", ".tiff")).ToArray(); }
                }
            }
        }
        Debug.Log(files.Length + " Files Found.");
        if (files.Length == 0)
        {
            NoFiles();
            return;
        }

        for (int i = 0; i < files.Length; i++)
        {
            bool streamingMipMapChanged = false;
            bool crunchComnpressionChanged = false;
            bool compressionQaulityChanged = false;
            bool maxSizeChanged = false;

            TextureImporter Ti = AssetImporter.GetAtPath(files[i]) as TextureImporter;
            if (Ti.textureShape != TextureImporterShape.Texture2D) { continue; }
            if (Ti.textureType != TextureImporterType.Default && Ti.textureType != TextureImporterType.NormalMap && Ti.textureType!=TextureImporterType.Sprite) { continue; }
            if (skipcompresion && Ti.crunchedCompression == true) { continue; }

            if (UseMaxsizeAdjuster)
            {
                int originalTextureSize = GetOriginalTextureSize(Ti);
                int compressedTextureSize = AdjustTextureMaxsizeInStage(originalTextureSize, 0);
                if (Ti.textureType == TextureImporterType.Default)
                {
                    if (compressedTextureSize > defaultMaxsizeInt) { compressedTextureSize = defaultMaxsizeInt; }
                    if (Ti.maxTextureSize != compressedTextureSize)
                    {
                        Ti.maxTextureSize = compressedTextureSize;
                        maxSizeChanged = true;
                    }
                }
                if (Ti.textureType == TextureImporterType.NormalMap)
                {
                    if (useNormalmapOp) { compressedTextureSize = AdjustTextureMaxsizeInStage(originalTextureSize, 1); }
                    if (compressedTextureSize > normalMaxsizeInt) { compressedTextureSize = normalMaxsizeInt; }
                    if (Ti.maxTextureSize != compressedTextureSize)
                    {
                        Ti.maxTextureSize = compressedTextureSize;
                        maxSizeChanged = true;
                    }
                }
                if (Ti.textureType == TextureImporterType.Sprite)
                {
                    if (compressedTextureSize > spriteMaxsizeInt) { compressedTextureSize = spriteMaxsizeInt; }
                    if (Ti.maxTextureSize != compressedTextureSize)
                    {
                        Ti.maxTextureSize = compressedTextureSize;
                        maxSizeChanged = true;
                    }
                }
            }
            if (texture_EditStreaminMipMap)
            {
                if (Ti.streamingMipmaps != useStreamingMipMap)
                {
                    Ti.streamingMipmaps = useStreamingMipMap;
                    streamingMipMapChanged = true;
                }
            }
            if (texture_EditTextureCompressionQuality)
            {
                Ti.textureCompression = tic;
            }
            if (texture_EditClunchCompression)
            {
                if (Ti.crunchedCompression != useCrunchCompression)
                {
                    Ti.crunchedCompression = useCrunchCompression;
                    crunchComnpressionChanged = true;
                }
                if (Ti.compressionQuality != compressionQaulity)
                {
                    Ti.compressionQuality = compressionQaulity;
                    compressionQaulityChanged = true;
                }
            }
            if (streamingMipMapChanged || crunchComnpressionChanged || compressionQaulityChanged || maxSizeChanged)
            {
                editedFilesCount++;
                EditorUtility.SetDirty(Ti);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(files[i], ImportAssetOptions.ForceUpdate);
            }
        }
        AssetDatabase.Refresh();
        Complete(editedFilesCount);
        Debug.Log("Compression Finished.");
    }
    private void BatchProcess_Model()
    {
        int editedFilesCount = 0;
        string[] files = new string[] { };
        for (int i = 0; i < model_Directories.Length; i++)
        {
            if (model_DirectoriesBool[i])
            {
                if (i == 0)
                {
                    if (fbx) { files = files.Concat(GetRootAssetsListByExt(".fbx")).ToArray(); }
                    if (obj) { files = files.Concat(GetRootAssetsListByExt(".obj")).ToArray(); }
                    if (blend) { files = files.Concat(GetRootAssetsListByExt(".blend")).ToArray(); }
                }
                else
                {
                    if (fbx) { files = files.Concat(GetAssetsListByExt(model_Directories[i] + "/", ".fbx")).ToArray(); }
                    if (obj) { files = files.Concat(GetAssetsListByExt(model_Directories[i] + "/", ".obj")).ToArray(); }
                    if (blend) { files = files.Concat(GetAssetsListByExt(model_Directories[i] + "/", ".blend")).ToArray(); }
                }
            }
        }
        Debug.Log(files.Length + " Files Found.");
        if (files.Length == 0)
        {
            NoFiles();
            return;
        }

        for (int i = 0; i < files.Length; i++)
        {
            bool sceneSettingChanged = false;
            bool meshesSettingChanged = false;
            bool geometrySettingChanged = false;
            ModelImporter Mi = AssetImporter.GetAtPath(files[i]) as ModelImporter;

            if (model_EditScene)
            {
                if (Mi.importCameras != importCameras)
                {
                    Mi.importCameras = importCameras;
                    sceneSettingChanged = true;
                }
                if (Mi.importLights != importLights)
                {
                    Mi.importLights = importLights;
                    sceneSettingChanged = true;
                }
            }
            if (model_EditMeshes)
            {
                if (Mi.meshCompression != ConvertMeshCompressionEnum(compression))
                {
                    Mi.meshCompression = ConvertMeshCompressionEnum(compression);
                    meshesSettingChanged = true;
                };
                if (Mi.isReadable != readWriteEnabled)
                {
                    Mi.isReadable = readWriteEnabled;
                    meshesSettingChanged = true;
                }
                if (Mi.optimizeMesh != optimizeMesh)
                {
                    Mi.optimizeMesh = optimizeMesh;
                    meshesSettingChanged = true;
                }
            }
            if (model_EditGeometry)
            {
                if (Mi.generateSecondaryUV != generateLightmapUVs)
                {
                    Mi.generateSecondaryUV = generateLightmapUVs;
                    geometrySettingChanged = true;
                }
            }
            if (sceneSettingChanged || meshesSettingChanged || geometrySettingChanged)
            {
                editedFilesCount++;
                EditorUtility.SetDirty(Mi);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(files[i], ImportAssetOptions.ForceUpdate);
            }
        }
        AssetDatabase.Refresh();
        Complete(editedFilesCount);
        Debug.Log("Compression Finished.");
    }
    private void BatchProcess_Audio()
    {
        int editedFilesCount = 0;
        string[] files = new string[] { };
        for (int i = 0; i < model_Directories.Length; i++)
        {
            if (audio_DirectoriesBool[i])
            {
                if (i == 0)
                {
                    if (mp3) { files = files.Concat(GetRootAssetsListByExt(".mp3")).ToArray(); }
                    if (ogg) { files = files.Concat(GetRootAssetsListByExt(".ogg")).ToArray(); }
                }
                else
                {
                    if (mp3) { files = files.Concat(GetAssetsListByExt(audio_Directories[i] + "/", ".mp3")).ToArray(); }
                    if (ogg) { files = files.Concat(GetAssetsListByExt(audio_Directories[i] + "/", ".ogg")).ToArray(); }
                }
            }
        }
        Debug.Log(files.Length + " Files Found.");
        if (files.Length == 0)
        {
            NoFiles();
            return;
        }

        for (int i = 0; i < files.Length; i++)
        {
            bool generalSettingChanged = false;
            bool compressionSettingChanged = false;

            AudioImporter Ai = AssetImporter.GetAtPath(files[i]) as AudioImporter;
            if (audio_EditGeneral)
            {
                if (Ai.forceToMono != forceToMono)
                {
                    Ai.forceToMono = forceToMono;
                    generalSettingChanged = true;
                }
            }
            if (audio_EditCompression)
            {
                AudioImporterSampleSettings Ais = new AudioImporterSampleSettings
                {
                    compressionFormat = ConvertAudioCompressionFormatEnum(format),
                    sampleRateSetting = ConvertAudioSampleRateSettingEnum(sampleRateSetting),
                    sampleRateOverride = ConvertOverrideSampleRateEnum(sampleRate),
                    quality = quality
                };
                Ai.defaultSampleSettings = Ais;
                compressionSettingChanged = true;
            }
            if (generalSettingChanged || compressionSettingChanged)
            {
                editedFilesCount++;
                EditorUtility.SetDirty(Ai);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(files[i], ImportAssetOptions.ForceUpdate);
            }
        }
        AssetDatabase.Refresh();
        Complete(editedFilesCount);
        Debug.Log("Compression Finished.");
    }

    private string[] GetAssetsListByExt(string path, string ext)
    {
        if (!Directory.Exists(path)) return new string[0];
        string[] files = Directory.GetFiles(path, "*" + ext, SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace("\\", "/");
            files[i] = files[i].Replace(Application.dataPath, "Assets");
        }
        return files;
    }
    private string[] GetRootAssetsListByExt(string ext)
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*" + ext, SearchOption.TopDirectoryOnly); for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace("\\", "/");
            files[i] = files[i].Replace(Application.dataPath, "Assets");
        }
        return files;
    }
    private int GetOriginalTextureSize(TextureImporter Ti)
    {
        object[] size = new object[2] { 0, 0 };
        MethodInfo method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(Ti, size);
        return Math.Max((int)size[0], (int)size[1]);
    }
    private int AdjustTextureMaxsizeInStage(int originalSize, int stageDown = 0)
    {
        int[] maxsizeList = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
        if (originalSize <= maxsizeList[0])
        {
            return maxsizeList[DownStage(0, stageDown)];
        }
        else if (originalSize <= maxsizeList[1])
        {
            return maxsizeList[DownStage(1, stageDown)];
        }
        else if (originalSize <= maxsizeList[2])
        {
            return maxsizeList[DownStage(2, stageDown)];
        }
        else if (originalSize <= maxsizeList[3])
        {
            return maxsizeList[DownStage(3, stageDown)];
        }
        else if (originalSize <= maxsizeList[4])
        {
            return maxsizeList[DownStage(4, stageDown)];
        }
        else if (originalSize <= maxsizeList[5])
        {
            return maxsizeList[DownStage(5, stageDown)];
        }
        else if (originalSize <= maxsizeList[6])
        {
            return maxsizeList[DownStage(6, stageDown)];
        }
        else if (originalSize <= maxsizeList[7])
        {
            return maxsizeList[DownStage(7, stageDown)];
        }
        else
        {
            return maxsizeList[DownStage(8, stageDown)];
        }

        int DownStage(int a, int b)
        {
            int x = a - b;
            if (x >= 0)
            {
                return x;
            }
            else
            {
                return 0;
            }
        }
    }
    private int ConvertTextMaxsizeEnum(MAXSIZE maxsize)
    {
        switch (maxsize)
        {
            case MAXSIZE.MaxSize32x32:
                return 32;
            case MAXSIZE.MaxSize64x64:
                return 64;
            case MAXSIZE.MaxSize128x128:
                return 128;
            case MAXSIZE.MaxSize256x256:
                return 256;
            case MAXSIZE.MaxSize512x512:
                return 512;
            case MAXSIZE.MaxSize1024x1024:
                return 1024;
            case MAXSIZE.MaxSize2048x2048:
                return 2048;
            case MAXSIZE.MaxSize4096x4096:
                return 4096;
            case MAXSIZE.MaxSize8192x8192:
                return 8192;
            default:
                return 2048;
        }
    }
    private ModelImporterMeshCompression ConvertMeshCompressionEnum(COMPRESS enu)
    {
        switch (enu)
        {
            case COMPRESS.OFF:
                return ModelImporterMeshCompression.Off;
            case COMPRESS.Low:
                return ModelImporterMeshCompression.Low;
            case COMPRESS.Medium:
                return ModelImporterMeshCompression.Medium;
            case COMPRESS.Heigh:
                return ModelImporterMeshCompression.High;
            default:
                return ModelImporterMeshCompression.Off;
        }
    }
    private AudioCompressionFormat ConvertAudioCompressionFormatEnum(FORMAT enu)
    {
        switch (enu)
        {
            case FORMAT.ADPCM:
                return AudioCompressionFormat.ADPCM;
            case FORMAT.PCM:
                return AudioCompressionFormat.PCM;
            case FORMAT.Vorbis:
                return AudioCompressionFormat.Vorbis;
            default:
                return AudioCompressionFormat.Vorbis;
        }

    }
    private AudioSampleRateSetting ConvertAudioSampleRateSettingEnum(RATETYPE enu)
    {
        switch (enu)
        {
            case RATETYPE.OptimizeSampleRate:
                return AudioSampleRateSetting.OptimizeSampleRate;
            case RATETYPE.OverrideSampleRate:
                return AudioSampleRateSetting.OverrideSampleRate;
            case RATETYPE.PreserveSampleRate:
                return AudioSampleRateSetting.PreserveSampleRate;
            default:
                return AudioSampleRateSetting.OptimizeSampleRate;
        }
    }
    private uint ConvertOverrideSampleRateEnum(RATE enu)
    {
        switch (enu)
        {
            case RATE.SampleRate11025Hz:
                return 11025;
            case RATE.SampleRate192000Hz:
                return 192000;
            case RATE.SampleRate22050Hz:
                return 22050;
            case RATE.SampleRate44100Hz:
                return 44100;
            case RATE.SampleRate48000Hz:
                return 48000;
            case RATE.SampleRate8000Hz:
                return 8000;
            case RATE.SampleRate96000Hz:
                return 96000;
            default:
                return 44100;
        }
    }
    private void Complete(int filecount)
    {
        _ = EditorUtility.DisplayDialog("Compression Finished!", filecount.ToString() + "ファイルの処理が完了しました。", "閉じる");
    }
    private void NoFiles()
    {
        _ = EditorUtility.DisplayDialog("No Files.", "条件を満たすファイルが存在しませんでした。", "閉じる");
    }

    private TextureImporterCompression ConvertTextureImporterCompressionEnum()
    {
        TextureImporterCompression result = TextureImporterCompression.Compressed;

        switch (this.textureCompressionQuality)
        {
            case 0:
                result = TextureImporterCompression.Uncompressed;
                break;
            case 1:
                result = TextureImporterCompression.CompressedLQ;
                break;
            case 2:
                result = TextureImporterCompression.Compressed;
                break;
            case 3:
                result = TextureImporterCompression.CompressedHQ;
                break;

            default:
                break;
        }

        return result;
    }

    private void SetLowQuality()
    {
        this.texture_EditStreaminMipMap = true;
        this.texture_EditTextureCompressionQuality = true;
        this.texture_EditClunchCompression = true;
        this.skipcompresion = false;
        this.useStreamingMipMap = true;
        this.textureCompressionQuality = 2;
        this.useCrunchCompression = true;
        this.compressionQaulity = 50;
        this.UseMaxsizeAdjuster = true;
        this.useNormalmapOp = true;
        this.defaultMaxsize = MAXSIZE.MaxSize2048x2048;
        this.normalMaxsize = MAXSIZE.MaxSize1024x1024;
        this.spriteMaxsize = MAXSIZE.MaxSize256x256;
}

    private void SetHighQuality()
    {
        this.texture_EditStreaminMipMap = true;
        this.texture_EditTextureCompressionQuality = true;
        this.texture_EditClunchCompression = true;
        this.skipcompresion = false;
        this.useStreamingMipMap = true;
        this.textureCompressionQuality = 3;
        this.useCrunchCompression = true;
        this.compressionQaulity = 100;
        this.UseMaxsizeAdjuster = true;
        this.useNormalmapOp = false;
        this.defaultMaxsize = MAXSIZE.MaxSize8192x8192;
        this.normalMaxsize = MAXSIZE.MaxSize8192x8192;
        this.spriteMaxsize = MAXSIZE.MaxSize8192x8192;
    }
}
