using UnityEngine;
using UnityEditor;

public class HorizontalTextureSlicer : EditorWindow
{
    [MenuItem("Tools/VFX/Horizontal 9-Slice to Array")]
    public static void ShowWindow() => GetWindow<HorizontalTextureSlicer>("Strip Slicer");

    public Texture2D sourceTexture;
    public string assetName = "VFX_StrisciaNuvolette";

    void OnGUI()
    {
        GUILayout.Label("Configurazione Striscia Orizzontale (1x9)", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Texture Sorgente", sourceTexture, typeof(Texture2D), false);
        assetName = EditorGUILayout.TextField("Nome Asset", assetName);

        if (sourceTexture != null)
        {
            int cellW = sourceTexture.width / 9;
            EditorGUILayout.HelpBox($"Risoluzione: {sourceTexture.width}x{sourceTexture.height}px\nOgni fetta sarà: {cellW}x{sourceTexture.height}px", MessageType.Info);
        }

        if (GUILayout.Button("Genera Texture2DArray") && sourceTexture != null)
        {
            SliceHorizontal();
        }
    }

    void SliceHorizontal()
    {
        // 1. Calcoli basati sulla riga unica
        int cellWidth = sourceTexture.width / 9;
        int cellHeight = sourceTexture.height; // Altezza intera
        int totalSlices = 9;

        // 2. Creazione Array
        Texture2DArray texArray = new Texture2DArray(cellWidth, cellHeight, totalSlices, sourceTexture.format, true);
        texArray.filterMode = sourceTexture.filterMode;
        texArray.wrapMode = TextureWrapMode.Clamp;

        // 3. Ciclo unico orizzontale (da sinistra a destra)
        for (int i = 0; i < totalSlices; i++)
        {
            // Prendiamo i pixel partendo da x = i * cellWidth
            Color[] pixels = sourceTexture.GetPixels(i * cellWidth, 0, cellWidth, cellHeight);
            texArray.SetPixels(pixels, i, 0);
        }

        texArray.Apply();

        // 4. Salvataggio
        string path = EditorUtility.SaveFilePanelInProject("Salva Texture Array", assetName, "asset", "Scegli destinazione");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(texArray, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=orange>VFX:</color> Array orizzontale creato! Indice 0 (Sinistra) -> Indice 8 (Destra).");
        }
    }
}