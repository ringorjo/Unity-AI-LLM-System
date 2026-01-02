using Sirenix.OdinInspector;
using Services.AI;

public abstract class AIOutputResponse : SerializedMonoBehaviour
{
    public AIResponseType Type;
    public abstract void ProcessAIResponse(AIAPIResponse response);

}

