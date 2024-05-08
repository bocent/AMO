using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
[InitializeOnLoad]
public class MaxstARMigration : EditorWindow
{
    private static readonly string UNIVERSAL = "Universal";
    private static readonly string STANDARD = "Standard";
    private static readonly string TYPE_MATERIAL = "t:Material";
    private static readonly string MAXST = "Maxst";
    private static readonly string URPLIT = "Universal Render Pipeline/Lit";

    static MaxstARMigration()
    {
        EditorApplication.delayCall += DelayedAction;
    }

    private static void DelayedAction()
    {
        var currentPipelineAsset = GraphicsSettings.defaultRenderPipeline;

        if (currentPipelineAsset == null)
        {
            ConvertMaterialsToStandard();
        }
        else if (currentPipelineAsset != null && currentPipelineAsset.GetType().ToString().Contains(UNIVERSAL))
        {
            ConvertMaterialsToURPLit();
        }
        else
        {
            ConvertMaterialsToStandard();
        }
        EditorApplication.delayCall -= DelayedAction;
    }

    #region ConvertSampleTexture
    [MenuItem("Help/MaxstAR/SampleAsset Tools/Standard Materials to URP Lit")]
    public static void ConvertMaterialsToURPLit()
    {
        string[] materialGuids = AssetDatabase.FindAssets(TYPE_MATERIAL);
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!path.Contains($"{MAXST}")) continue;

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material.shader.name == STANDARD)
            {
                Color albedoColor = material.GetColor("_Color");
                Texture albedoTexture = material.GetTexture("_MainTex");
                Texture occlusionMap = material.GetTexture("_OcclusionMap");

                Shader urpLitShader = Shader.Find(URPLIT);
                if (urpLitShader != null)
                {
                    material.shader = urpLitShader;
                    material.SetColor("_BaseColor", albedoColor);
                    material.SetTexture("_BaseMap", albedoTexture);
                    material.SetTexture("_OcclusionMap", occlusionMap);
                }
                else
                {
                    Debug.LogWarning("URP Lit Shader not found. Please ensure URP is correctly set up in your project.");
                }

                EditorUtility.SetDirty(material);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Finished converting all Standard Materials to URP Lit.");
    }

    [MenuItem("Help/MaxstAR/SampleAsset Tools/URP Lit Materials to Standard")]
    public static void ConvertMaterialsToStandard()
    {
        string[] materialGuids = AssetDatabase.FindAssets(TYPE_MATERIAL);
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!path.Contains($"{MAXST}")) continue;

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material.shader.name == URPLIT)

            {
                Color albedoColor = material.GetColor("_Color");
                Texture albedoTexture = material.GetTexture("_MainTex");
                Texture occlusionMap = material.GetTexture("_OcclusionMap");

                Shader standardShader = Shader.Find(STANDARD);
                if (standardShader != null)
                {
                    material.shader = standardShader;
                    material.SetColor("_BaseColor", albedoColor);
                    material.SetTexture("_BaseMap", albedoTexture);
                    material.SetTexture("_OcclusionMap", occlusionMap);
                }
                else
                {
                    Debug.LogWarning("Standard Shader not found. Please ensure Standard is correctly set up in your project.");
                }

                EditorUtility.SetDirty(material);
                AssetDatabase.SaveAssets();
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Finished converting all Standard Materials to Standard.");
    }
    #endregion
}

#endif
