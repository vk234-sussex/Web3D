using UnityEngine;

public class ModelMaterialModifier : MonoBehaviour
{
    [SerializeField] Material wireframeMat;
    [SerializeField] Material litMat;

    private void Start()
    {
        SetLit();
    }

    public void SetLit()
    {
        GetComponent<MeshRenderer>().sharedMaterial = litMat;
    }

    public void SetWireframe()
    {
        GetComponent<MeshRenderer>().sharedMaterial = wireframeMat;
    }
}

