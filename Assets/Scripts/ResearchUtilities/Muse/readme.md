# Muse EEG Utilities

This folder contains core scripts for handling Muse EEG data streaming and recording in Unity.

## Components

### MuseLslManager
- **Purpose:**
  - Handles connection to Muse EEG device via LSL (Lab Streaming Layer).
  - Pulls EEG samples from the LSL stream and buffers them for use in Unity.
  - Provide optimized access to lastest EEG samples.
- **Key Public Members:**
  - `public static MuseLslManager Instance { get; }` — Singleton instance for global access.
  - `public static void GetOrCreateInstance(out MuseLslManager instance)` — Ensures a manager exists in the scene.
  - `public bool IsConnected` — Indicates if the LSL stream is connected.
  - `public bool EstablishConnection()` — Attempts to connect to the Muse LSL stream.
  - `public ReadOnlySpan<MuseEEGSample> GetSamplesSinceLastPull()` — Returns all new EEG samples since the last call.
- **Notice**
  - The `MuseLslManager` is designed to be used as a singleton. It will automatically create itself if not present in the scene, ensuring that there is always a manager available for EEG data handling.
  - The GameObject containing `MuseLslManager` will be set to DontDestroyOnLoad, allowing it to persist across scene loads.

### MuseEEGFileRecorder
- **Purpose:**
  - Buffers EEG samples and writes them to a CSV file for later analysis.
  - Supports timed and untimed recording, buffer management, and efficient file writing strategy.
- **Key Public Members:**
  - (Most methods are internal/private and managed by Unity lifecycle.)
  - Recording is controlled via methods like `TryStartRecording` and `StopRecording` (exposed as needed). If duration is less than 0, it will record until `StopRecording` is called.
  - Default file path is set to `Application.persistentDataPath` with a timestamped filename. If you want to have a specified name, you will have to make sure that is unique or the recording will not start. This is to prevent unexpected overwriting of files.
- **Notice**
  - The `MuseEEGFileRecorder` is designed to be used in conjunction with `MuseLslManager`.
  - Please don't use too slow disk drives, as the buffer will grow and consume memory if the disk write speed is not sufficient to keep up with the incoming data rate.

## Usage
1. Add `MuseLslManager` to your Unity scene (or call `GetOrCreateInstance` to make sure there exist one).
2. Use `MuseEEGFileRecorder` to record EEG data to disk. You can access it by `MuseLslManager.Instance.FileRecorder` Recording can be started/stopped programmatically with `TryStartRecording` and `StopRecording`.
3. All data is written in CSV format for easy analysis.

## Dependencies
- Unity
- LSL (Lab Streaming Layer) C# bindings

## File Structure
- `MuseLslManager.cs` — Handles LSL connection and data buffering.
- `MuseEEGFileRecorder.cs` — Handles data buffering and file writing.
- `MuseEEGSample.cs` — Data structure for a single EEG sample.

---
For more details, see code comments in each script.

