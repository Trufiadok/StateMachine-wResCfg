#if TOOLS
using System.Collections.Generic;
using Godot;


[Tool]
public partial class SmRestarter : EditorPlugin
{
    bool FirstRun = true;

    EditorFileSystem editorFileSystem;
    EditorInterface editorInterface;

    public override void _EnterTree()
    {
        // GD.Print("Start SmRestarter Plugin");

        editorInterface = GetEditorInterface();
        editorFileSystem = editorInterface.GetResourceFilesystem();

        BuildSolution();
    }

    public override void _Process(double delta)
    {
        if (FirstRun)
        {
            FirstRun = false;            
            editorFileSystem.Scan();
            RestartStateMachinePlugin();
        }
    }

    void RestartStateMachinePlugin()
    {
        if (GetEditorInterface().IsPluginEnabled("StateMachine"))
        {
            // GD.Print("StateMachine Plugin Running");
            GetEditorInterface().SetPluginEnabled("StateMachine", false);
            // GD.Print("Stop StateMachine Plugin");
        }
        GetEditorInterface().SetPluginEnabled("StateMachine", true);
    }

    void BuildSolution()
    {
        Godot.Collections.Array output = new Godot.Collections.Array();
        OS.Execute("powershell.exe", new string[] {"dotnet build /property:GenerateFullPaths=true"}, output, readStderr : true);
        for (int i = 0; i < output.Count; i++)
        {
            string[] lines = output[i].AsString().Split("\r\n"); 
            foreach (string line in lines)
            {
                GD.Print(line);
            }
        }
    }
}
#endif