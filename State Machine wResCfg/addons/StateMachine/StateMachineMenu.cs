#if TOOLS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using StateMachine.Resources;

[Tool]
public partial class StateMachineMenu : EditorPlugin
{
    enum CreateType { none, TransitionTable, State, ActionScript, ConditionScript, Actions, Conditions }

    PopupMenu stateMachineMenu = new PopupMenu();
    PopupMenu actionsMenu = new PopupMenu();
    PopupMenu conditionsMenu = new PopupMenu();

    public class ClassPath
    {
        public string fullPath;
        public string className;

        public ClassPath(string fullPath, string className)
        {
            this.fullPath = fullPath;
            this.className = className;
        }
    }

    List<ClassPath> actionsList = new List<ClassPath>();
    List<ClassPath> conditionsList = new List<ClassPath>();
    
    long menuItemId;

    public override void _EnterTree()
    {
        // GD.Print("Start StateMachine Plugin");

        StopSmRestarterPlugin();

        #region Create ToolsMenu

        // Searching for State Action Scripts & State Condition Scripts into toolsmenu
        string editorDirectory = ProjectSettings.GlobalizePath("res://");
        List<string> allCsharpFile = Directory.GetFiles(editorDirectory, "*.cs", SearchOption.AllDirectories).ToList<string>();
        allCsharpFile = allCsharpFile.FindAll(p => !p.Contains(".godot"));

        actionsList.Clear();
        conditionsList.Clear();

        // check all *.cs file
        foreach (string csPath in allCsharpFile)
        {
            string className = Path.GetFileNameWithoutExtension(csPath);

            // GD.Print(className + " - " + csPath);

            Type type = Type.GetType(className);
            if (type != null)
            {
                string baseType = type.BaseType.ToString();
                // GD.Print(type.ToString());
                // GD.Print(baseType);

                if (baseType == "StateMachine.Resources.StateActionRES")
                {
                    actionsList.Add(new ClassPath(csPath, className));
                    // GD.Print("Add to actionsList");
                }
                else if (baseType == "StateMachine.Resources.StateConditionRES")
                {
                    conditionsList.Add(new ClassPath(csPath, className));
                    // GD.Print("Add to conditionsList");
                }
            }
        }

        // create base PopupMenu
        stateMachineMenu.AddItem("Create TransitionTable", 1);
        stateMachineMenu.AddItem("Create State", 2);
        stateMachineMenu.AddSeparator();
        stateMachineMenu.AddItem("Create Action Script", 3);
        stateMachineMenu.AddItem("Create Condition Script", 4);
        stateMachineMenu.AddSeparator();

        // create action PopupSubMenu
        int idx = 50;
        foreach (ClassPath actionPath in actionsList)
        {
            idx++;

            string menuLabel = actionPath.className;
            if (menuLabel.Substring(menuLabel.Length - 3, 3) == "RES")
                menuLabel = menuLabel.Substring(0, menuLabel.Length - 3);

            actionsMenu.AddItem(menuLabel, idx);
            // GD.Print("Add to Menu " + menuLabel + ", " + idx.ToString());
            // GD.Print(actionPath.className + " - " + actionPath.fullPath);
        }

        if (actionsList.Count > 0)
        {
            stateMachineMenu.AddChild(actionsMenu);
            actionsMenu.Name = "actionsMenu";
            // GD.Print(actionsMenu.Name);
            stateMachineMenu.AddSubmenuItem("Create Actions", "actionsMenu", 5);

            actionsMenu.IdPressed += OnPressedActionsMenu;
        }

        // create condition PopupSubMenu
        idx = 80;
        foreach (ClassPath conditionPath in conditionsList)
        {
            idx++;

            string menuLabel = conditionPath.className;
            if (menuLabel.Substring(menuLabel.Length - 3, 3) == "RES")
                menuLabel = menuLabel.Substring(0, menuLabel.Length - 3);

            conditionsMenu.AddItem(menuLabel, idx);
            // GD.Print("Add to Menu " + menuLabel + ", " + idx.ToString());
            // GD.Print(conditionPath.className + " - " + conditionPath.fullPath);
        }

        if (conditionsList.Count > 0)
        {
            stateMachineMenu.AddChild(conditionsMenu);
            conditionsMenu.Name = "conditionsMenu";
            // GD.Print(conditionsMenu.Name);
            stateMachineMenu.AddSubmenuItem("Create Conditions", "conditionsMenu", 6);

            conditionsMenu.IdPressed += OnPressedConditionsMenu;
        }

        AddToolSubmenuItem("State Machine", stateMachineMenu);

        // subscription to clickevents of popupmenu
        stateMachineMenu.IdPressed += OnPressedStateMachineMenu;

        #endregion Create ToolsMenu
    }

    private void OnPressedActionsMenu(long id)
    {
        menuItemId = id;
        // GD.Print("id = " + menuItemId.ToString());

        if (id > 50 && id < 80)
        {
            FileDialogWindow(CreateType.Actions);
        }
    }

    private void OnPressedConditionsMenu(long id)
    {
        menuItemId = id;
        // GD.Print("id = " + menuItemId.ToString());

        if (id > 80 && id < 110)
        {
            FileDialogWindow(CreateType.Conditions);
        }
    }

    private void OnPressedStateMachineMenu(long id)
    {
        menuItemId = id;
        // GD.Print("id = " + menuItemId.ToString());

        switch (id)
        {
            case 1:
                FileDialogWindow(CreateType.TransitionTable);
                break;

            case 2:
                FileDialogWindow(CreateType.State);
                break;

            case 3:
                FileDialogWindow(CreateType.ActionScript);
                break;

            case 4:
                FileDialogWindow(CreateType.ConditionScript);
                break;
        }
    }

    void FileDialogWindow(CreateType createType)
    {
        string selectedEditorDirectory = GetEditorInterface().GetCurrentDirectory();

        FileDialog fileDialog = new FileDialog();
        fileDialog.Title = "Save TransitionTable Resource File";
        fileDialog.Mode = Window.ModeEnum.Windowed;
        fileDialog.CurrentPath = selectedEditorDirectory;

        Vector2I ScreenSize = GetTree().Root.Size;
        Vector2I dialogSize = new Vector2I(700, 500);
        fileDialog.Position = ScreenSize / 2 - dialogSize / 2;
	    fileDialog.Size = dialogSize;

        fileDialog.OkButtonText = "Create";

        switch (createType)
        {
            case CreateType.TransitionTable:
                fileDialog.Title = "Create TransitionTable Resource File";
                fileDialog.AddFilter("*.tres", "resource");
                fileDialog.FileSelected += CreateTransitionTable;
                break;

            case CreateType.State:
                fileDialog.Title = "Create State Resource File";
                fileDialog.AddFilter("*.tres", "resource");
                fileDialog.FileSelected += CreateState;
                break;

            case CreateType.ActionScript:
                fileDialog.Title = "Create Action Script File";
                fileDialog.AddFilter("*.cs", "csharp");
                fileDialog.FileSelected += CreateActionScript;
                break;

            case CreateType.ConditionScript:
                fileDialog.Title = "Create Condition Script File";
                fileDialog.AddFilter("*.cs", "csharp");
                fileDialog.FileSelected += CreateConditionScript;
                break;

            case CreateType.Actions:
                fileDialog.Title = "Create Action Resource File";
                fileDialog.AddFilter("*.tres", "resource");
                fileDialog.FileSelected += CreateAction;
                break;

            case CreateType.Conditions:
                fileDialog.Title = "Create Condition Resource File";
                fileDialog.AddFilter("*.tres", "resource");
                fileDialog.FileSelected += CreateCondition;
                break;
        }
        AddChild(fileDialog);
        fileDialog.Show();
    }

    private void CreateTransitionTable(string path)
    {
        TransitionTableRES transitionTable = new TransitionTableRES();
        transitionTable.ResourceName = Path.GetFileNameWithoutExtension(path);
        ResourceSaver.Save(transitionTable, path, ResourceSaver.SaverFlags.None);
    }

    void CreateState(string path)
    {
        StateRES state = new StateRES();
        state.ResourceName = Path.GetFileNameWithoutExtension(path);
        ResourceSaver.Save(state, path, ResourceSaver.SaverFlags.None);
    }

    private void CreateActionScript(string path)
    {
        string className = Path.GetFileNameWithoutExtension(path);
        path = path.Insert(path.Length - 3, "RES");
        string editorDirectory = ProjectSettings.GlobalizePath("res://");
        string absolutePath = ProjectSettings.GlobalizePath(path);
        string actionScriptFile = File.ReadAllText(editorDirectory + "/addons/StateMachine/ActionScriptSnippet.snp");
        actionScriptFile = actionScriptFile.Replace("${1:SCRIPTNAME}", className);
        File.WriteAllText(absolutePath, actionScriptFile);
        GetEditorInterface().GetResourceFilesystem().Scan();

        RestartSmRestarterPlugin();
    }

    private void CreateConditionScript(string path)
    {
        string className = Path.GetFileNameWithoutExtension(path);
        path = path.Insert(path.Length - 3, "RES");
        string editorDirectory = ProjectSettings.GlobalizePath("res://");
        string absolutePath = ProjectSettings.GlobalizePath(path);
        string conditionScriptFile = File.ReadAllText(editorDirectory + "/addons/StateMachine/ConditionScriptSnippet.snp");
        conditionScriptFile = conditionScriptFile.Replace("${1:SCRIPTNAME}", className);
        File.WriteAllText(absolutePath, conditionScriptFile);
        GetEditorInterface().GetResourceFilesystem().Scan();

        RestartSmRestarterPlugin();
    }

    private void CreateAction(string path)
    {
        // GD.Print(menuItemId);

        string fullPath = actionsList[(int)menuItemId - 51].fullPath;
        string className = actionsList[(int)menuItemId - 51].className;

        // GD.Print(className + " - " + fullPath);
        Type type = Type.GetType(className);
        // GD.Print(type.ToString());
        
        var actionInstance = (Resource)Activator.CreateInstance(type);

        actionInstance.ResourceName = Path.GetFileNameWithoutExtension(path);
        ResourceSaver.Save(actionInstance, path, ResourceSaver.SaverFlags.None);
    }

    private void CreateCondition(string path)
    {
        // GD.Print(menuItemId);

        // foreach (ClassPath classPath in conditionsList)
        // {
        //     GD.Print(classPath.className + " - " + classPath.fullPath);
        // }

        string fullPath = conditionsList[(int)menuItemId - 81].fullPath;
        string className = conditionsList[(int)menuItemId - 81].className;

        Type type = Type.GetType(className);
        var conditionInstance = (Resource)Activator.CreateInstance(type);

        conditionInstance.ResourceName = Path.GetFileNameWithoutExtension(path);
        ResourceSaver.Save(conditionInstance, path, ResourceSaver.SaverFlags.None);
    }
    
    public override void _ExitTree()
    {
        RemoveToolMenuItem("State Machine");     
    }

    void RestartSmRestarterPlugin()
    {
        if (GetEditorInterface().IsPluginEnabled("SmRestarter"))
        {
            // GD.Print("SmRestarter Plugin Running");
            GetEditorInterface().SetPluginEnabled("SmRestarter", false);
            // GD.Print("Stop SmRestarter Plugin");
        }
        GetEditorInterface().SetPluginEnabled("SmRestarter", true);
    }

    void StopSmRestarterPlugin()
    {
        if (GetEditorInterface().IsPluginEnabled("SmRestarter"))
        {
            // GD.Print("SmRestarter Plugin Running");
            GetEditorInterface().SetPluginEnabled("SmRestarter", false);
            // GD.Print("Stop SmRestarter Plugin");
        }
    }
}
#endif