# UserActivityTracker

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Subsystem](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf)
[![Lincense](https://img.shields.io/badge/Lincense-MIT-orange.svg?style=flat-square)](https://github.com/Fei-Sheng-Wu/UserActivityTracker/blob/main/LICENSE)

> A lightweight tracker on user actions for WPF. Support both mouse and keyboard actions. Able to record the user actions, save it to a compressed string, and play the recorded actions for analysis. Support full window monitoring or specified focus on a particular element. Support saving the initial size upon recording.

## Main Features

- [x] Record user actions without worries on the different types of elements
- [x] Record user actions with relative positionings
- [x] Play the recorded user actions at anytime
- [x] Save the recording data with compression
- [x] Specify a particular element for recording
- [x] Save the initial size of the element upon recording

## How to Use

Use `UserActivityTracker.Recorder` to record user actions.

```c#
//Start upon the window is ready.
UserActivityTracker.Recorder recorder = new UserActivityTracker.Recorder(this); //Set the element to be recorded to the window.
recorder.Start(); //Start the recording. Returns true if the recording was started successfully.

//Stop upon the window is being closed.
recorder.Stop(); //Stop the recording. Returns true if the recording was stopped successfully.
string session = recorder.Save(); //Retrieve the string representation of the recording. Use Save(false) if compression is not wanted.
```

Use `UserActivityTracker.Player player` to play user actions.

```c#
MainWindow window = new MainWindow(); //Create a new window for the user actions to be played.
UserActivityTracker.Player player = new UserActivityTracker.Player(window); //Set the element to play the user actions to the new window.
window.Show(); //Show the new window.
window.ContentRendered += async (obj, args) => //Play the user actions when the new window is ready.
{
    if (!player.IsPlaying) //Check whether the playing has been started yet.
    {
        await player.Play(session); //Play the recorded user actions from the string representation.
        await Task.Delay(500); //Pause for 500 milliseconds before closing the new window.
        window.Close(); //Close the new window as the playing is done.
    }
};
```

## Recording Data

The saved string representation includes a single digit indicating whether compression is used and then the recorded data.

```
1,eyJGcmFtZVJhdGUiOjMwLCJTdGFydGluZ1dpZHRoIjo4MDAsIlN0YXJ0aW5nSGVpZ2h0Ijo1MDAsIkFjdGlvbnMiOlsidzE3MCIsIm0zNTEuMiwzMjgiLCJ3MTQiLCJtMzg0LDI1Ni44IiwidzE0IiwibTM5Ny42LDIzNy42IiwidzE0IiwibTQwOCwyMjAiLCJ3MTQiLCJtNDEyLDIxMi44IiwidzE0IiwibTQxNC40LDIwOC44IiwidzE0IiwibTQxNiwyMDUuNiIsIncxMyIsIm00MTYuOCwyMDIuNCIsIncxNCIsIm00MTguNCwxOTcuNiIsIncxNCIsIm00MTguNCwxOTIuOCIsIncxNCIsIm00MTguNCwxODguOCIsIncxNCIsIm00MTkuMiwxODMuMiIsIncyOSIsIm00MTkuMiwxODAiLCJwNDE5LjIsMTgwLDAiLCJ3MTQiLCJtNDE5LjIsMTgwIiwidzE0IiwicjQxOS4yLDE4MCwwIiwidzE4NiIsImQ4IiwidzYwIiwidTgiLCJ3NDUiLCJkOCIsInc3NyIsInU4IiwidzE0IiwiZDgiLCJ3NjAiLCJ1OCIsIncxNCIsImQ4IiwidzYxIiwidTgiLCJ3MzAiLCJkOCIsInc2MCIsInU4IiwidzEyNCIsImQ4IiwidTgiLCJkOCIsInc0NiIsInU4IiwidzI0OCIsImQxNjAiLCJ3MTg2IiwiZDg0IiwidzEzIiwidTE2MCIsIncxNCIsInU4NCIsInc2MSIsImQ2OSIsInc0NSIsImQ4MyIsInc2MSIsInU2OSIsInU4MyIsIncxMDciLCJkODQiLCJ3NDUiLCJ1ODQiLCJ3NzE3IiwibTQxOS4yLDE3OC40IiwidzE0IiwibTQxOS4yLDE3MiIsIncxNCIsIm00MTkuMiwxNjYuNCIsIncxNCIsIm00MTguNCwxNjIuNCIsIncxNCIsIm00MTcuNiwxNjAiLCJ3MTQiLCJtNDE2LjgsMTU4LjQiLCJ3NjAiLCJwNDE2LjgsMTU3LjYsMCIsInc0NiIsInI0MTYuOCwxNTcuNiwwIiwidzMyNiIsIm00MTcuNiwxNTcuNiIsIncxNCIsIm00MzMuNiwxNTcuNiIsIncxNCIsIm00NTMuNiwxNTcuNiIsIncxNCIsIm00NjguOCwxNTguNCIsIncxMyIsIm00ODAuOCwxNTguNCIsIncxNCIsIm00ODcuMiwxNTguNCIsIncxNCIsIm00OTMuNiwxNTguNCIsIncxNCIsIm00OTkuMiwxNTguNCIsInczMCIsIm01MDQsMTU5LjIiLCJ3MTMiLCJtNTA3LjIsMTU5LjIiLCJ3MTQiLCJtNTA4LjgsMTYwIiwidzc3IiwicDUwOC44LDE2MC44LDAiLCJ3MjMyIiwibTUwOC44LDE2MS42IiwidzE0IiwibTUwOC44LDE2OCIsIncxNCIsIm01MDguOCwxNzQuNCIsIncxNCIsIm01MDguOCwxNzguNCIsIncxNCIsIm01MDguOCwxODAuOCIsInI1MDguOCwxODEuNiwwIiwidzE4NiIsIm01MDYuNCwxODEuNiIsIncxNCIsIm00ODEuNiwxODIuNCIsIncxNCIsIm00NjcuMiwxODIuNCIsIncyMDEiLCJzNDY1LjYsMTgyLjQsNzQiLCJzNDY1LjYsMTgyLjQsMTciLCJzNDY1LjYsMTgyLjQsMTMiLCJzNDY1LjYsMTgyLjQsMTQiLCJzNDY1LjYsMTgyLjQsMTYiLCJzNDY1LjYsMTgyLjQsMTQiLCJzNDY1LjYsMTgyLjQsNyIsInM0NjUuNiwxODIuNCw3IiwiczQ2NS42LDE4Mi40LDUiLCJzNDY1LjYsMTgyLjQsNSIsInM0NjUuNiwxODIuNCw0IiwiczQ2NS42LDE4Mi40LDIiLCJzNDY1LjYsMTgyLjQsMSIsInM0NjUuNiwxODIuNCwyIiwiczQ2NS42LDE4Mi40LDIiLCJzNDY1LjYsMTgyLjQsMiIsInM0NjUuNiwxODIuNCwxIiwiczQ2NS42LDE4Mi40LDQiLCJzNDY1LjYsMTgyLjQsMiIsIncyNjMiLCJtNDU3LjYsMTk4LjQiLCJ3MTQiLCJtNDIxLjYsMjU0LjQiLCJ3MTQiLCJtMzkyLDI5My42IiwidzE0IiwibTM4NC44LDMwMS42IiwidzE0IiwibTM3Ny42LDMxNC40IiwidzE0IiwibTM3Mi44LDMyMCIsIncxNCIsIm0zNjkuNiwzMjQiLCJ3MTQiLCJtMzY3LjIsMzI3LjIiLCJ3MTMiLCJtMzY0LjgsMzI4LjgiLCJwMzY0LjgsMzI4LjgsMCIsInc0NiIsInIzNjQuOCwzMjguOCwwIl19
```

After decompression, the recorded data is in a JSON format and includes `FrameRate`, `StartingWidth`, `StartingHeight`, and `UserActions`.

```
{"FrameRate":30,"StartingWidth":800,"StartingHeight":500,"Actions":["w170","m351.2,328","w14","m384,256.8","w14","m397.6,237.6","w14","m408,220","w14","m412,212.8","w14","m414.4,208.8","w14","m416,205.6","w13","m416.8,202.4","w14","m418.4,197.6","w14","m418.4,192.8","w14","m418.4,188.8","w14","m419.2,183.2","w29","m419.2,180","p419.2,180,0","w14","m419.2,180","w14","r419.2,180,0","w186","d8","w60","u8","w45","d8","w77","u8","w14","d8","w60","u8","w14","d8","w61","u8","w30","d8","w60","u8","w124","d8","u8","d8","w46","u8","w248","d160","w186","d84","w13","u160","w14","u84","w61","d69","w45","d83","w61","u69","u83","w107","d84","w45","u84","w717","m419.2,178.4","w14","m419.2,172","w14","m419.2,166.4","w14","m418.4,162.4","w14","m417.6,160","w14","m416.8,158.4","w60","p416.8,157.6,0","w46","r416.8,157.6,0","w326","m417.6,157.6","w14","m433.6,157.6","w14","m453.6,157.6","w14","m468.8,158.4","w13","m480.8,158.4","w14","m487.2,158.4","w14","m493.6,158.4","w14","m499.2,158.4","w30","m504,159.2","w13","m507.2,159.2","w14","m508.8,160","w77","p508.8,160.8,0","w232","m508.8,161.6","w14","m508.8,168","w14","m508.8,174.4","w14","m508.8,178.4","w14","m508.8,180.8","r508.8,181.6,0","w186","m506.4,181.6","w14","m481.6,182.4","w14","m467.2,182.4","w201","s465.6,182.4,74","s465.6,182.4,17","s465.6,182.4,13","s465.6,182.4,14","s465.6,182.4,16","s465.6,182.4,14","s465.6,182.4,7","s465.6,182.4,7","s465.6,182.4,5","s465.6,182.4,5","s465.6,182.4,4","s465.6,182.4,2","s465.6,182.4,1","s465.6,182.4,2","s465.6,182.4,2","s465.6,182.4,2","s465.6,182.4,1","s465.6,182.4,4","s465.6,182.4,2","w263","m457.6,198.4","w14","m421.6,254.4","w14","m392,293.6","w14","m384.8,301.6","w14","m377.6,314.4","w14","m372.8,320","w14","m369.6,324","w14","m367.2,327.2","w13","m364.8,328.8","p364.8,328.8,0","w46","r364.8,328.8,0"]}
```

`UserActions` use a single letter to indicate the type of action, then action-specific parameters seperated by commas.

```
w170 //Pause for 170 milliseconds.
```
```
m351.2,328 //Move the mouse cursor to 351.2 on the x-axis and 328 on the y-axis in relative positioning to the recorded element.
```
```
p419.2,180,0 //Press down the left mouse button at 419.2 on the x-axis and 180 on the y-axis in relative positioning to the recorded element.
```
```
r419.2,180,0 //Release up the left mouse button at 419.2 on the x-axis and 180 on the y-axis in relative positioning to the recorded element.
```
```
s465.6,182.4,74 //Scroll the mouse wheel by a delta of 74 at 465.6 on the x-axis and 182.4 on the y-axis in relative positioning to the recorded element.
```
```
d8 //Press down the backspace key.
```
```
u8 //Release up the backspace key.
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/UserActivityTracker/blob/main/LICENSE).
