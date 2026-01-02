using System.Collections.Generic;
using UnityEngine;

public class DemoInteractionController : MonoBehaviour
{
    [SerializeField]
    private List<ModelData> _shapesList;

    public ModelData GetModelByName(ModelType modelType)
    {
        return _shapesList.Find(x => x.ModelType == modelType);
    }
}
