DATALOGGER FOR GFG

SETUP
1. Create an empty GameObject in your first scene (e.g., MainMenu)
2. Attach DataLogger.cs to it
3. It persists across all scenes automatically


STARTING A SESSION
For GaitwayArcade, the session start is already set up for you. 

The first scene in the game is called FIRST_SCENE (Assets/Hub/Scenes/FIRST_SCENE). It has an input box where you can type in participant ID and either skip or start your session. If you want to see how to start a session, the script handling this scene is called DataLoggerInput. 

If you want to test logging a session with just one game, you can. To start a session, call this when the participant begins (e.g., after entering their ID in your menu):

    DataLogger.Instance.StartSession("GA001");

- Session number is auto-detected (first session = 1, increments from there)
- Uses existing files in the output folder to determine the next session number


LOGGING DATA
From any script, anywhere:

    DataLogger.Instance.LogData("Category", "EventName", "optional extra data");

Examples:
    DataLogger.Instance.LogData("Input", "StepDetected", "LeftFoot");
    DataLogger.Instance.LogData("Golf", "SwingComplete", "Power:0.8,Accuracy:0.95");

Shorthand methods:
    DataLogger.Instance.LogInput("StepDetected", "RightFoot");
    DataLogger.Instance.LogGameEvent("ScoreUpdate", "Points:500");
    DataLogger.Instance.LogMinigameEvent("Golf", "HoleComplete", "Strokes:3");

It will automatically check for you if a session is active, but you can also use the public boolean IsSessionActive. 


AUTOMATIC LOGGING
DataLogger automatically logs:
- Session start (when you call StartSession)
- Scene changes (with timestamp so we can later calculate the change)
- Session end (when application quits)


OUTPUT
Reminder that the file will only save automatically on ApplicationQuit, meaning that you need a build, unless you create a temporary measure that will call:
  DataLogger.Instance.SaveToFile();

I made a prefab button that saves to file, it’s in the DataCollection folder in Assets. I also put an editor-only save button in the GameSelect.

Files save to: [Application.persistentDataPath]/GameData/

Filename format: GA_[ParticipantID]_[SessionNumber]_[DateTime].csv

Example: GA_GA001_1_20250203_143052.csv

CSV columns:
    ParticipantID, SessionNumber, Timestamp, SessionTime, Scene, EventType, EventData


FINDING YOUR DATA
Windows: C:\Users\[Username]\AppData\LocalLow\[CompanyName]\[ProductName]\GameData\
Mac: ~/Library/Application Support/[CompanyName]/[ProductName]/GameData/

Or call DataLogger.Instance.GetOutputPath() to get the exact path.


