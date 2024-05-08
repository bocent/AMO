using UnityEngine;

public class PlateMove : MonoBehaviour
{
    [SerializeField]
    private float offsetIncreaseRate = 1.0f; // 조절하고자 하는 증가 속도
    private Renderer renderer;
    private string offsetProperty = "_MainTex";

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer.sharedMaterial.HasProperty("_BaseMap"))
        {
            offsetProperty = "_BaseMap";
        }
        else if (renderer.sharedMaterial.HasProperty("_MainTex"))
        {
            offsetProperty = "_MainTex";
        }
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sharedMaterial
            .SetTextureOffset(
            offsetProperty,
            new Vector2(
                renderer.material.mainTextureOffset.x - offsetIncreaseRate * Time.deltaTime, renderer.material.mainTextureOffset.y
                )
            );
    }
}
