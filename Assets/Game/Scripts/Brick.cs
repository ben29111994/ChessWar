using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public LayerMask filledBrick;
    public Color defaultColor;
    public bool isFilled;
    private bool isHole;
    private Renderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    private Collider boxCollider;
    public Color white;
    public bool isBlack;
    public bool isTest;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        isFilled = false;
        boxCollider.enabled = true;

    }

    private void Init()
    {
        boxCollider = GetComponent<BoxCollider>();

        propBlock = new MaterialPropertyBlock();

        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();
    }

    public void SetBrickColor(bool _isBlack)
    {
        if (_isBlack)
        {
            meshRenderer.material = ColorManager.instance.material_Caro1;
        }
        else
        {
            meshRenderer.material = ColorManager.instance.material_Caro2;
        }
    }

    public IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        boxCollider.enabled = true;
    }

    public void DisableCollider()
    {
        boxCollider.enabled = false;
    }

}
