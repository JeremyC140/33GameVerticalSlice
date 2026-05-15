# GDIM33 Vertical Slice
## Milestone 1 Devlog
1. 
Currently I have one visual scripting graph attached on my GameManager gameObj. The graph intends to decouple the effect performance and UI update based on music note judgment (perfect, good, or miss). Specifically, after the state machine in my c# script NoteVisual evaluates the music note as one of the judgment type, it will call EventBus.trigger on the custom events I established in the graph. These custom events respectively contain "PerfectHit", "GoodHit", and "MissHit". These custom event in visual script each branches into their individual action and perform their individual effects. Currently when they operate, they would update the TextMeshProUGUI in the center of the screen and display the judgment result on screen. This is done by using the SetText node in TextMeshProUGUI option and assigning String Literal with either "Perfect!", "Good!", or "Miss!" to it, updating the UI element which I dragged in as an Object Variable.


2. 
I updated the break-down chart to specify the details of the state machine in my C# method Judge, contained in the script NoteVisual. Specifically, each time when the player attempts to tap the music note, the method EvaluateHit in RhythmManager is called and return a HitGrade result ("perfect", "good", or "miss"). This HitGrade result is then transfered to the Judge method mentioned previously, and the state machine switches its state based on the HitGrade. This is why in my break-down chart, I drew an arrow connecting the note prefab to the state machine, because the state machine logic would always performed in the script of a specific instance of note prefab. 

Then, the state machine alters and performs corresponding action by calling EventBus.trigger on their designated event to trigger in Visual Scripting decoupling graph, as described in question 1. The visual scripting would receive the call from state machine and perform the "visual juices" based on the state. This will include the UI update on the judgment result, a different SFX based on hit result, and a different color of VFX based on hit result, but these effects are still working in progress. As a result, the combination of c# state machine and visual scripting graph helps me clearly distinguish and decouple the relationship between complicated logics (the actual judgment and calculation) and the visual performance (the visual juices to execute based on result). This helps me a lot when comprehensing and visualizing my entire project. It's much more clear in imagining the functionalities of each part and I think the decoupling makes a lot of sense. 

![New Break-down Graph with State Machine Details](image.png)

## Milestone 2 Devlog
1. 


--- Task 1: Implementing Song Selection Menu ---

    1. Create a new scene named SongSelectionScene, outlining the basic GameObjects like UI and SceneManager in the hierarchy

    2. Establish a SceneManager script that handles the transition of scene to GamePlayScene 

    3. Create the scroller UI on the left-hand side of the screen that displays a list of scriptableObjects that represent each individual song. The scroller should be a list of scrollable objects with song names.

    4. Set-up the song detail panel on the right-hand side of the screen that display the details store in the scriptableObject when that individual song is selected (create a songSelectionManager to update the metadata to UI texts).

    5. Create prefab button that conduct the transition from SongSelectionScene to GamePlayScene while also passing the current song selected to the GameController.


--- Task 2: Polish UI, Visual Indication, and Features ---

    1. On each star lane in game, label them with their corresponding keyboard to strengthen the mental connection of lane and keyboard. Specifically, add a text label in the object container of each star sprite, adjusted with appropriate size that's both visible and not visually distracting.

    2. Search for fonts online and apply it in game to polish the visual. Cite the font in external assets (also cite the other components I have used).

    3. Implement the feature of pausing and restarting the game. Specifically, when "esc" or "p" is pressed, the time scale of the game should be toggled to either zero (paused) or one (unpaused); this will be a function in the GameController singleton. As for restarting, it will be another function that's called on "z" pressed and reload the scene. 


2.  
I think the breakdown is helpful for me in building this personal project because it gives me a clear and logical control on my to-dos rather than starting a new task from totally scratch, without pre-thinking about the entire procedure and knowing what will be done next. It also motivates me on my own tasks because I found myself to be low energy in doing works if I considered that task to be "super difficult" and "don't know where to start". Writing these steps down do help me to focus on the task while understanding my work and progress. Next time, I will narrow down the big step even more because the breakdown I wrote actually look like a "big big step" follow by several other "big steps" XD. However, I didn't dislike this too much because I would also be discouraged if the breakdown is too long and contains way too many steps. I don't think an improvement is in-demand, as I prefer to workout some of the tiny, intricated details as I craft through the logics rather than planning everything out thoroughly in advance. 


3.  



4.  
I would like to be graded for the use of ScriptableObjects, which I used to store the metadata, images, audio, chart, and so on for each individual songs. I have implemented 2 of this song ScriptableObjects in game that can be seen in the song menu, where there's two song choices in the scroller that update its stored metadata to the right-hand panel when it's clicked. The gameplay is only available for "Infinite Heaven" though. 


## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- [Starla Font from DaFont](https://www.dafont.com/starla.font)
- [Song Metadata and Images - Infinite Heaven](https://arcaea.fandom.com/wiki/Infinity_Heaven)
- [Song Metadata and Images - Tempestissmo](https://arcaea.fandom.com/wiki/Tempestissimo)
- [Song Metadata and Images - Aegleseeker](https://arcaea.fandom.com/wiki/Aegleseeker)
