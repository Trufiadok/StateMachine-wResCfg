# StateMachine-wResCfg
This plugin implements the basics of a state machine that you can configure with resource files.

*The state machine implemented in this plugin is based on the state machine of the 'Unity Open Project #1: Chop Chop' project.
https://github.com/UnityTechnologies/open-project-1*

*This guide is based on the 'State Machine' description in the 'Unity Open Project #1: Chop Chop' project. https://github.com/UnityTechnologies/open-project-1/wiki/State-machine*

A state machine is an object that can be in one of multiple **states**. Each of these **states** performs a specific set of **actions**, and can **transition** to different **states** based on predefined **conditions**.
In plugin, we implemented a custom state machine solution based on Resource objects.

### Two-Parts Implementation
This implementation is divided into two parts: the configuration, which happens at edit time and is based on  **Resource objects**, and the  **runtime**  part.

#### Resource objects
The Resource object part of the implementation is a designer-friendly, modular way of building state machines. The Resource objects are used to initialise the state machine with data, which means that by creating more instances of the same type you can give different values to them and create quantitative variations of the same behaviour.

-   **StateRES**: A  `Resource object`  that can hold a reference to one or more actions (`StateActionRES`).
-   **StateActionRES**: A  `Resource object`  that wraps a  **StateAction**.
-   **StateConditionRES**: A  `Resource object`  that wraps a  **Condition**.
-   **TransitionTableRES**: A  `Resource object`  that defines the  **Transitions**  of a state machine. This table "recaps" the structure of the state machine, and in fact we use it to display a custom Inspector window which allows you to have a complete overview of the state machine and all of its states, actions, transitions and conditions.

As an example, you can find the Resource objects for the 'State Machine wResCfg Demo' state machine in the folder  `\Demo StateMachine\Resources\`.

#### Runtime
The runtime part of the implementation has its name from being the one that contains the code that gets executed when in Play mode. Runtime objects are instances of plain C# classes, created from a corresponding  `Resource object`  (see above).

-   **State**: A class that contains multiple instances of  `Action`  and  `Transition`. The  `State`  is responsible for routing the callbacks from the  `StateMachine`  to each  `Action`  and  `Transition`. This class is only used internally.
-   **StateAction**: A class that represents an action to perform. This is the base class all custom actions must inherit from.
-   **StateTransition**: A class that contains a target  `State`, and a set of  `Condition`  that must be evaluated to transition to it. Each  `State`  holds its own  `Transitions`  list. This class is only used internally.
-   **Condition**: A class that represents a conditional statement. This is the base class all custom conditions must inherit from.
-   **StateCondition**: A struct that holds a  `Condition`  and the expected result of the condition. This allows for reusability for when a  `Condition`  should sometimes evaluate to  `true`, and sometimes to  `false`. This struct is only used internally.
-   **StateMachine**: Should be attatched to any  `Node`  that wants to make use of this implementation. It takes in a  `TransitionTableRES`, and, during its  `Awake`  call, each  `ActionRES`,  `ConditionRES`, and  `StateRES`  in it is resolved into its runtime counterpart, and the  `Transitions`  lists are generated. In  `Update`,  `Actions`  of the current  `State`  are performed and then  `Conditions`  of every  `Transition`  of the current  `State`  are checked. The first  `Transition`  that evaluates to  `true`  gets triggered.

### How-Tos

#### Creating a State
To create a new  `StateRES`, simply open the Godot Editor menu and navigate to Project > Tools > State Machine > Create State. Name your new State and then you can start adding actions to it. Note that the order in which the actions are displayed corresponds to the order in which they are performed; the custom Inspector allows for easy sorting so you can organize them accordingly.

#### Creating Actions and Conditions
To create a new  `Action`  or  `Condition`  script, in the Assets menu go to Project > Tools > State Machine > Create Action/Condition Script. This will create a new script from a template that will contain both the  **RES**  class and the  **runtime**  class. You can modify both classes to your needs. Remember that after creating the script, you need to create at least one instance of the RES to use it in your state machine. The template already adds the Godot Editor menu item for you, under Project > Tools > State Machine > Create Actions/Conditions > Your New Script.

#### Creating a Transition Table
You can create a new Transition Table by opening the Project > Tools > State Machine > Create TransitionTable. As a result, a file with the extension `.tres` is created.

Selecting the **TransitionTable** file will display it in the Inspector. **TransitionTable** is an array whose elements are of type *TransitionItem*. Add an element to an array by increasing the size of the array by one. Use the arrow next to `<empty>` to select `New TransitionItem`. Clicking on the *TransitionItem* opens the object. If you want to name the *TransitionItem* you can override the `Resource.Name` property.
Each transition needs a 'From State' and a 'To State'. 'From State' refers to the state the state machine should be in so that it can transition to the 'To State'. You can drag already created **States** into 'From State' and 'To State'. After setting them, you also need to add the conditions that must be met to make the transition happen. **Conditions** is an array whose elements are of type *ConditionUsage*. Add an element to an array by increasing the size of the array by one. Use the arrow next to `<empty>` to select `New ConditionUsage`. Clicking on the *ConditionUsage* opens the object. If you want to name the *ConditionUsage* you can override the `Resource.Name` property. You can chain conditions with the And/Or option, and set whether the condition should be  `true`  or  `false`, allowing for multiple scenarios in which the transition should take place.  You can drag already created **Conditions** into 'Condition'.

The transition table is organized by 'TransitionItem'. Transitions that take priority within a 'TransitionItem' should be sorted first, as they are checked sequentially and the first one that is met will trigger the transition. You can sort the 'TransitionItems' them by moving hamburger icon. You can also remove a transition with the trash bin icon. Removing all the 'To State's from a 'From State' will remove the 'From State' from the table. You can also sort the 'TransitionItems. However, only the first one is important to set up correctly as it will be the initial state of your state machine; subsequent ordering is purely preferential.

Transition (TransitionItem) in Inspector

![enter image description here](https://github.com/Trufiadok/StateMachine-wResCfg/blob/main/Docs/OpenedTransitionItem.png)

State in Inspector

![enter image description here](https://github.com/Trufiadok/StateMachine-wResCfg/blob/main/Docs/OpenedState.png)

Condition (ConditionUsage) in Inspector

![enter image description here](https://github.com/Trufiadok/StateMachine-wResCfg/blob/main/Docs/OpenedConditionUsage.png)