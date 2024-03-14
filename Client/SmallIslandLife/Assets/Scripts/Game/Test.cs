using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Update()
    {
        meshRenderer.material.SetFloat("_SliceAmount", Random.Range(0f, 1f));
    }
}
