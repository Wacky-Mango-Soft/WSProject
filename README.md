### This is a project to implement an open world in Unity.
<hr/>
<br></br>

> Complete System
0. Real-time open world implementation and use of various shaders
1. Inventory & quick slot and convenience features
2. Various gathering and crafting systems
3. Map level design and minimap synchronization system
4. Dungeon leveling design and optimization
5. NPC behavioral AI implementation
6. Free switching between first and third person
7. Save and load auto-save function
<br></br>

> RoadMap
- Stealth system
- Non-first strike monsters
- Animal and monster training
- Talkable LLM applied NPC
- Add story
- World optimization
- Character status and level system
- Dialog system
- Equipment visualization
- Multiplayer implementation
- Combat system (parrying, skills that open at each level)
- Apply effects such as character footsteps
- Implementation of items that change status
- Detailed map design
- Weather and temperature system
- Creation of unique modeling Blender
- Cutscene creation
- Main menu option setting system
- Post Processing Shader
- Add addressable
<br></br>

> History
#### ㅇ As development scale expands and the number of sources to handle increases, time resource waste occurs due to human error.
  - Solved by documentation of the relevant part, taking inspiration from the writing of the API specification of the RESTFulAPI development process
  - Eliminate redundant work that increases exponentially according to workload by converting highly available classes into singletons and managers.
  - Minimize the creation and use of string scripts that cause human errors by using Enum Class and Serializer Attribute

#### ㅇ As the size of the project increases unintentionally due to the use of the Asset Store, issues continue to arise in the shape maintenance system and the use of collaboration programs such as GitHub.
  - Limit Asset Store resource push via Git Ingore
        (Missing metadata in asset settings resulting from this can be resolved by sending and receiving specific data files to the cloud)
  - Use of backup repository to suppress merge conflicts
Attempting to resolve potential issues through the use of Unity’s own Organization policy system and Cloud system

#### ㅇ Unintended bugs continue to occur between repetitive routines depending on the frame of game development.
  - Refer to Unity official documentation and methods
  - Active adoption of coroutines and synchronized and asynchronized classes and methods
  - Control statement conditional branching refinement

ㅇ Attempts to develop using MaxOS resulted in compatibility issues between UnityEngine and Apple Silicon.
  - Unity Documentation, StackOverflow, Unity Forum, Apple
  - Documentation reference resolution
