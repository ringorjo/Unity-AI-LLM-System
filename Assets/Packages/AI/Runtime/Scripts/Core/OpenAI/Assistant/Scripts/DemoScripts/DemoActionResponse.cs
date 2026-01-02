using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoActionResponse 
{
    public ModelType Model;
    public string Color;
}

public enum ModelType
{
    cube,
    sphere
}

[System.Serializable]
public class ModelData
{
    public ModelType ModelType;
    public MeshRenderer MeshRenderer;

    public void ChangeColor(Color color)
    {
        MeshRenderer.material.color = color;
    }
}
