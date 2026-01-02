using Newtonsoft.Json;
using UnityEngine;

namespace Services.AI
{
    public class AIProcessorOutputResponseAction : AIOutputResponse
    {
        [SerializeField]
        private DemoInteractionController demoInteractionController;

        private void Reset()
        {
            Type = AIResponseType.Json_extraData;
        }
        public override void ProcessAIResponse(AIAPIResponse response)
        {
            DemoActionResponse demoResponse = JsonConvert.DeserializeObject<DemoActionResponse>(response.Text);
            if (demoResponse != null)
            {
                Color color = GetColorByResponse(demoResponse.Color);
                demoInteractionController.GetModelByName(demoResponse.Model).ChangeColor(color);
            }
        }
        private Color GetColorByResponse(string colorResponse)
        {
            switch (colorResponse)
            {
                case "blue":
                    return Color.blue;
                case "red":
                    return Color.red;
                case "green":
                    return Color.green;
                case "yellow":
                    return Color.yellow;
                default:
                    return Color.black;
            }
        }
    }
}
