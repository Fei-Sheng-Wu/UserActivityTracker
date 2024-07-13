# UserActivityTracker

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Subsystem](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf)
[![Lincense](https://img.shields.io/badge/Lincense-MIT-orange.svg?style=flat-square)](https://github.com/Fei-Sheng-Wu/UserActivityTracker/blob/main/LICENSE)

> A lightweight real-time tracker of user interactions for WPF. Support both mouse and keyboard actions. Able to save the tracked recording to a string value and play the recorded actions for UI/UX analysis. Support full window monitoring or a specified focus on a particular element. Support saving the initial size and other states upon starting.

## Main Features

- [x] Record user actions without worrying about different types of elements
- [x] Record user actions with relative positionings
- [x] Save the recording data to a string representation
- [x] Play the recorded user actions at anytime
- [x] Specify a particular element for recording
- [x] Save the initial size of the element upon starting
- [x] Save additional customized configurations upon starting

## How to Use

Use `UserActivityTracker.Recorder` to record user actions.

```c#
//Start upon the window is ready. The ContentRendered event can be used, for example, to start the recording automatically.
UserActivityTracker.Recorder recorder = new UserActivityTracker.Recorder(this); //Set the element to be recorded to the window.
recorder.FrameRate = 30; //The frame rate can be set optionally. This only affects basic user actions such as moving the mouse.
recorder.Start(); //Start the recording. Returns true if the recording was started successfully.
```
```c#
//Stop upon the window is being closed. The Closing event can be used, for example, to stop the recording automatically.
recorder.Stop(); //Stop the recording. Returns true if the recording was stopped successfully.
string session = recorder.Save(); //Retrieve the string representation of the recording.
```

Use `UserActivityTracker.Player` to play user actions.

```c#
MainWindow window = new MainWindow(); //Create a new window for the user actions to be played.
UserActivityTracker.Player player = new UserActivityTracker.Player(window); //Set the element to play the user actions to the new window.
window.Show(); //Show the new window.
window.ContentRendered += async (obj, args) => //Play the user actions when the new window is ready.
{
    if (!player.IsPlaying) //Check whether the playing has been started yet.
    {
        await player.Play(session); //Play the recorded user actions from the string representation.
        window.Close(); //Close the new window as the playing is done.
    }
};
```

### Availability Status

Check whether the recording has started yet:

```c#
bool isRecording = recorder.IsRecording; //Indicates whether the recording has started.
```

Check whether the playing has started yet:

```c#
bool isPlaying = player.IsPlaying; //Indicates whether the playing has started.
```

Check whether the operation was executed successfully:

```c#
if (!recorder.Start()) //Start(), Stop(), and Play() all return a boolean value, with true indicating success.
{
    //Something wrong happened. The operation failed to be executed successfully.
}
```

### Customizable Starting Configuration

Store a customizable string without the character ";" that can be used again upon playing:

```c#
string startingConfig = textRandom.Text; //Use the content of the TextBlock as the starting configuration. This configuration cannot include the character ";" in it.
recorder.Start(startingConfig); //Start the recording with a customized configuration that can be used upon playing. Returns true if the recording was started successfully.
```

Play the user actions with a callback that uses the configuration string stored:

```c#
//Play the recorded user actions from the string representation along with a callback that retrieves the saved starting configuration.
await player.Play(session, (startingConfig) =>
{
    if (window.FindName("textRandom") is TextBlock textBlock)
    {
        textBlock.Text = startingConfig; //Use the retrieved configuration on the TextBlock.
    }
});
```

## Recording Data

The saved string representation is in a format where attributes are separated with semicolons and include `f`(`FrameRate`), `w`(`StartingWidth`), `h`(`StartingHeight`), `c`(`StartingConfig`), and `a`(`Actions`). Each attribute uses a single-letter indication of the attribute that is followed by the value directly, minimizing the length of the string representation.

> f30;w800;h500;c;aw170m351.2,328w14m384,256.8w14m397.6,237.6w14m408,220w14m412,212.8w14m414.4,208.8w14m416,205.6w13m416.8,202.4w14m418.4,197.6w14m418.4,192.8w14m418.4,188.8w14m419.2,183.2w29m419.2,180p419.2,180,0w14m419.2,180w14r419.2,180,0w186d8w60u8w45d8w77u8w14d8w60u8w14d8w61u8w30d8w60u8w124d8u8d8w46u8w248d160w186d84w13u160w14u84w61d69w45d83w61u69u83w107d84w45u84w717m419.2,178.4w14m419.2,172w14m419.2,166.4w14m418.4,162.4w14m417.6,160w14m416.8,158.4w60p416.8,157.6,0w46r416.8,157.6,0w326m417.6,157.6w14m433.6,157.6w14m453.6,157.6w14m468.8,158.4w13m480.8,158.4w14m487.2,158.4w14m493.6,158.4w14m499.2,158.4w30m504,159.2w13m507.2,159.2w14m508.8,160w77p508.8,160.8,0w232m508.8,161.6w14m508.8,168w14m508.8,174.4w14m508.8,178.4w14m508.8,180.8r508.8,181.6,0w186m506.4,181.6w14m481.6,182.4w14m467.2,182.4w201s465.6,182.4,74s465.6,182.4,17s465.6,182.4,13s465.6,182.4,14s465.6,182.4,16s465.6,182.4,14s465.6,182.4,7s465.6,182.4,7s465.6,182.4,5s465.6,182.4,5s465.6,182.4,4s465.6,182.4,2s465.6,182.4,1s465.6,182.4,2s465.6,182.4,2s465.6,182.4,2s465.6,182.4,1s465.6,182.4,4s465.6,182.4,2w263m457.6,198.4w14m421.6,254.4w14m392,293.6w14m384.8,301.6w14m377.6,314.4w14m372.8,320w14m369.6,324w14m367.2,327.2w13m364.8,328.8p364.8,328.8,0w46r364.8,328.8,0

`Actions` is a string representing a list of `UserAction` that uses single letters to indicate the type of action, then action-specific parameters seperated by commas. All coordinates are based on relative positioning to the recorded element.

```
w170 //Pause for 170 milliseconds.
```
```
m351.2,328 //Move the mouse cursor to 351.2 on the x-axis and 328 on the y-axis.
```
```
p419.2,180,0 //Press down the left mouse button at 419.2 on the x-axis and 180 on the y-axis.
```
```
r419.2,180,0 //Release up the left mouse button at 419.2 on the x-axis and 180 on the y-axis.
```
```
s465.6,182.4,74 //Scroll the mouse wheel by 74 at 465.6 on the x-axis and 182.4 on the y-axis.
```
```
d8 //Press down the backspace key.
```
```
u8 //Release up the backspace key.
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/UserActivityTracker/blob/main/LICENSE).
