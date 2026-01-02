# AI 

SETUP
-You must change the .NET version to the latest one from Player Settings > Other Settings and set Scripting Backend to IL2CPP.
-Download the Samples of the AI Model to be used.
-Download the Samples of the TaskManager Demo.

HOW TO START
-Drag the AI Model prefab from Samples/[AI Model]/Prefabs/AIModelPrefab.
-Drag the Task Validator prefab from Samples/TaskManagerDemo/Prefabs/IAProcedureValidation.
-Inside the Resources folder, right-click and create:
-Xennial Digital/AI/AIConfigData
-Fill in the configuration details, such as Model URL, Token, System Prompt, etc.
-In the AIHandler model, enable the option "Override AI Configuration" and drag the ScriptableObject created in the previous step.
-Click Update AI Config.

CREATE AI ACTION INTERACTION
-Create a class that inherits from AIActionInteraction.

CREATE AI DIALOG INTERACTION
-Create a class that inherits from SemanticDialogInteraction.

Dependencies:
http://186.28.57.76:3000/Xennial/XennialAPI.git
http://186.28.57.76:3000/Xennial/Services.git
http://186.28.57.76:3000/Xennial/TaskManager.git