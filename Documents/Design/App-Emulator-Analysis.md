# NLib Serial Emulator App - Architecture Analysis

**Application Type:** WPF Desktop Application (.NET Framework 4.7.x)
**Purpose:** Serial device emulator for testing and development
**Location:** `09.App/NLib.Serial.Emulator.App/`

---

## Overview

The Serial Emulator App is a WPF-based desktop application that simulates serial devices for testing purposes. It allows developers to test serial communication without physical hardware by emulating device protocols.

### Key Features

- **Multi-Device Support:** Emulates 7 different serial devices simultaneously
- **Tabbed Interface:** Separate tab for each device emulator
- **Real-time Data Transmission:** Timer-based continuous data sending (500ms intervals)
- **Configurable Serial Settings:** Port, baud rate, parity, data bits, stop bits, handshake
- **Independent Device Control:** Each device can be started/stopped independently
- **Data Input UI:** Input fields for device-specific parameters

---

## Application Architecture

```mermaid
graph TD
    App[App.xaml.cs<br/>Application Entry] --> AppController[WpfAppController<br/>Singleton Instance]
    App --> MainWindow[MainWindow.xaml<br/>Main UI]

    MainWindow --> Timer[System.Timers.Timer<br/>500ms Interval]
    MainWindow --> TabControl[TabControl<br/>7 Device Tabs]

    TabControl --> TFO1Tab[TFO1 Tab]
    TabControl --> PHTab[PH Meter Tab]
    TabControl --> QATab[Weight QA Tab]
    TabControl --> SPUNTab[Weight SPUN Tab]
    TabControl --> JIKTab[JIK6CAB Tab]
    TabControl --> DEFTab[DEFENDER3000 Tab]
    TabControl --> MettlerTab[Mettler MS204TS00 Tab]

    TFO1Tab --> TFO1Page[TFO1EmulatorPage.xaml]
    PHTab --> PHPage[PHMeterEmulatorPage.xaml]
    QATab --> QAPage[WeightQAEmulatorPage.xaml]
    SPUNTab --> SPUNPage[WeightSPUNEmulatorPage.xaml]
    JIKTab --> JIKPage[JIK6CABEmulatorPage.xaml]
    DEFTab --> DEFPage[CordDEFENDER3000EmulatorPage.xaml]
    MettlerTab --> MettlerPage[MettlerMS204TS00EmulatorPage.xaml]

    TFO1Page --> Setting1[SerialCommSetting]
    QAPage --> Setting2[SerialCommSetting]
    SPUNPage --> Setting3[SerialCommSetting]

    Setting1 --> Device1[TFO1Device.Instance]
    Setting2 --> Device2[WeightQADevice.Instance]
    Setting3 --> Device3[WeightSPUNDevice.Instance]

    Timer -.Triggers every 500ms.-> SyncAll[SyncAndSendAll]
    SyncAll --> AllPages[All Emulator Pages<br/>Sync Method]

    AllPages --> Devices[Device.Instance.Send]

    style App fill:#e1f5ff
    style MainWindow fill:#fff4e6
    style Timer fill:#ffebee
    style TabControl fill:#f3e5f5
    style Setting1 fill:#e8f5e9
    style Device1 fill:#e0f2f1
```

---

## Class Diagram - Emulator Page Structure

```mermaid
classDiagram
    class UserControl {
        <<WPF>>
    }

    class EmulatorPageBase {
        <<abstract pattern>>
        +Setup() void
        +Sync() void
        -onSync bool
    }

    class WeightQAEmulatorPage {
        -onSync bool
        -txtW TextBox
        -txtUnit TextBox
        -txtMode TextBox
        -ctrlSetting SerialCommSetting
        +Setup() void
        +Sync() void
    }

    class SerialCommSetting {
        -_device ISerialDeviceEmulator
        -cbPortNames ComboBox
        -txtBoadRate TextBox
        -cbParities ComboBox
        -txtDataBit TextBox
        -cbStopBits ComboBox
        -cbHandshakes ComboBox
        -cmdTogleConnect Button
        +Setup(ISerialDeviceEmulator) void
        -InitControls() void
        -UpdateCurrentSetting() void
        -EnableInputs(bool) void
        -cmdTogleConnect_Click() void
    }

    class ISerialDeviceEmulator {
        <<interface>>
        +Start() void
        +Shutdown() void
        +Send(byte[]) void
        +LoadConfig() void
        +SaveConfig() void
        +Config SerialPortConfig
        +IsOpen bool
    }

    class WeightQADevice {
        <<singleton>>
        +Instance$ WeightQADevice
        +Value WeightQAData
        +DeviceName string
    }

    UserControl <|-- EmulatorPageBase
    EmulatorPageBase <|.. WeightQAEmulatorPage
    WeightQAEmulatorPage *-- SerialCommSetting
    SerialCommSetting --> ISerialDeviceEmulator
    ISerialDeviceEmulator <|.. WeightQADevice
```

---

## Sequence Diagram - Application Startup

```mermaid
sequenceDiagram
    participant User
    participant App as App.xaml.cs
    participant Controller as WpfAppController
    participant LogMgr as LogManager
    participant MainWin as MainWindow
    participant Pages as Emulator Pages
    participant Devices as Device Instances
    participant Timer as System.Timer

    User->>App: Launch Application
    activate App

    App->>App: OnStartup
    App->>Controller: Setup(EnvironmentOptions)
    activate Controller
    Controller->>Controller: Check Single Instance
    Controller-->>App: Instance OK
    deactivate Controller

    App->>LogMgr: Start()
    activate LogMgr
    LogMgr-->>App: Started
    deactivate LogMgr

    App->>MainWin: new MainWindow()
    activate MainWin
    App->>Controller: Run(window)
    Controller->>MainWin: Show()

    MainWin->>MainWin: Window_Loaded
    MainWin->>MainWin: InitDevices()

    loop For each device
        MainWin->>Pages: Setup()
        activate Pages
        Pages->>Devices: LoadConfig()
        activate Devices
        Devices-->>Pages: Config Loaded
        deactivate Devices
        Pages-->>MainWin: Ready
        deactivate Pages
    end

    MainWin->>Timer: InitTimer()
    activate Timer
    Timer->>Timer: Set Interval 500ms
    Timer->>Timer: Start()
    Timer-->>MainWin: Timer Running

    deactivate MainWin
    deactivate App

    Note over Timer: Every 500ms
    Timer->>MainWin: Elapsed Event
    activate MainWin
    MainWin->>MainWin: SyncAndSendAll()

    loop For each active device
        MainWin->>Pages: Sync()
        activate Pages
        Pages->>Pages: Read UI values
        Pages->>Devices: Value.W = txtW.Text
        Pages->>Devices: Value.ToByteArray()
        activate Devices
        Devices-->>Pages: byte[] data
        deactivate Devices
        Pages->>Devices: Send(data)
        Pages-->>MainWin: Sent
        deactivate Pages
    end

    deactivate MainWin
```

---

## Sequence Diagram - Device Start/Stop Flow

```mermaid
sequenceDiagram
    participant User
    participant Setting as SerialCommSetting
    participant Device as Device.Instance
    participant SerialPort as SerialPort
    participant Thread as RX Background Thread

    User->>Setting: Click Start Button
    activate Setting

    Setting->>Setting: UpdateCurrentSetting()
    Note over Setting: Read UI values:<br/>PortName, BaudRate,<br/>Parity, DataBits,<br/>StopBits, Handshake

    Setting->>Device: Config = updated values
    Setting->>Device: SaveConfig()

    Setting->>Device: Start()
    activate Device
    Device->>Device: OpenPort(Config)
    Device->>SerialPort: new SerialPort()
    activate SerialPort
    Device->>SerialPort: Set properties
    Device->>SerialPort: Open()
    SerialPort-->>Device: Port Opened

    Device->>Thread: CreateRXThread()
    activate Thread
    Thread->>Thread: Start background loop
    Thread-->>Device: Thread Running

    Device-->>Setting: IsOpen = true
    deactivate Device

    Setting->>Setting: Update Button Text to "Shutdown"
    Setting-->>User: Device Started
    deactivate Setting

    Note over Thread: Continuous Loop
    loop While IsOpen
        Thread->>SerialPort: BytesToRead
        SerialPort-->>Thread: Read bytes
        Thread->>Thread: Add to RX Queue
        Thread->>Thread: ProcessRXQueue()
        Thread->>Thread: Sleep(10ms)
    end

    User->>Setting: Click Shutdown Button
    activate Setting
    Setting->>Device: Shutdown()
    activate Device
    Device->>Device: ClosePort()
    Device->>Thread: FreeRXThread()
    Thread->>Thread: Stop loop
    Thread->>Thread: Abort thread
    deactivate Thread

    Device->>SerialPort: Close()
    Device->>SerialPort: Dispose()
    deactivate SerialPort

    Device-->>Setting: IsOpen = false
    deactivate Device

    Setting->>Setting: Update Button Text to "Start"
    Setting-->>User: Device Stopped
    deactivate Setting
```

---

## Component Structure

### 1. Application Entry (App.xaml.cs)

**Responsibilities:**
- Application initialization
- Environment setup
- Log manager lifecycle
- Single instance enforcement
- Main window creation

**Key Code:**
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    EnvironmentOptions option = new EnvironmentOptions()
    {
        AppInfo = new NAppInformation()
        {
            CompanyName = "Vseg",
            ProductName = "NLib Device Emulator",
            Version = "1.1.115"
        },
        Behaviors = new NAppBehaviors()
        {
            IsSingleAppInstance = true,
            EnableDebuggers = true
        }
    };

    WpfAppContoller.Instance.Setup(option);
    LogManager.Instance.Start();

    Window window = new MainWindow();
    WpfAppContoller.Instance.Run(window);
}
```

### 2. Main Window (MainWindow.xaml.cs)

**Responsibilities:**
- Central coordinator for all device emulators
- Timer management for periodic data transmission
- Device lifecycle management (Init/Free)
- UI composition with TabControl

**Key Features:**
- **Timer-based Sync:** 500ms interval timer triggers `SyncAndSendAll()`
- **Symmetric Init/Free:** Each device has matching Init/Free methods
- **Sequential Sync:** Calls Sync() on each page in order

**Key Code:**
```csharp
private void InitTimer()
{
    _timer = new Timer();
    _timer.Interval = 500; // 500 ms
    _timer.Elapsed += _timer_Elapsed;
    _timer.Start();
}

private void _timer_Elapsed(object sender, ElapsedEventArgs e)
{
    if (_onSync) return;
    _onSync = true;
    Dispatcher.Invoke(new Action(() => { SyncAndSendAll(); }));
    _onSync = false;
}

private void SyncAndSendAll()
{
    SendTFO1();
    SendPHMeter();
    SendWeightQA();
    SendWeightSPUN();
    SendJIK6CAB();
    SendDEFENDER3000();
    SendMettlerMS204TS00();
}
```

### 3. Emulator Pages (e.g., WeightQAEmulatorPage.xaml.cs)

**Responsibilities:**
- Device-specific UI inputs
- Data synchronization from UI to device
- Integration with SerialCommSetting control

**Pattern:**
```csharp
public void Setup()
{
    WeightQADevice.Instance.LoadConfig();
    ctrlSetting.Setup(WeightQADevice.Instance);
}

public void Sync()
{
    if (!WeightQADevice.Instance.IsOpen) return;
    if (onSync) return;

    onSync = true;
    var data = WeightQADevice.Instance.Value;

    data.W = decimal.Parse(txtW.Text);
    data.Unit = txtUnit.Text.Trim().ToUpper();
    data.Mode = txtMode.Text.Trim().ToUpper();

    var buffers = data.ToByteArray();
    WeightQADevice.Instance.Send(buffers);

    onSync = false;
}
```

### 4. Serial Communication Setting Control (SerialCommSetting.xaml.cs)

**Responsibilities:**
- Serial port configuration UI
- Device start/stop control
- Config persistence (load/save)

**UI Elements:**
- Port Name dropdown
- Baud Rate textbox
- Parity dropdown
- Data Bits textbox
- Stop Bits dropdown
- Handshake dropdown
- Start/Shutdown toggle button

**Key Code:**
```csharp
private void cmdTogleConnect_Click(object sender, RoutedEventArgs e)
{
    if (!_device.IsOpen)
    {
        UpdateCurrentSetting();
        _device.Start();
    }
    else
    {
        _device.Shutdown();
    }

    cmdTogleConnect.Content = (!_device.IsOpen) ? "Start" : "Shutdown";
}
```

---

## Data Flow Diagram

```mermaid
flowchart LR
    subgraph UI Layer
        Input[UI Input Fields<br/>txtW, txtUnit, txtMode]
        Button[Start/Shutdown Button]
        Settings[Serial Settings UI]
    end

    subgraph Control Layer
        Page[Emulator Page<br/>Sync Method]
        SettingCtrl[SerialCommSetting<br/>Control]
        Timer[Main Timer<br/>500ms]
    end

    subgraph Device Layer
        DeviceInst[Device.Instance<br/>Singleton]
        DataObj[DeviceData Object<br/>W, Unit, Mode]
        Serial[SerialPort<br/>Communication]
    end

    subgraph External
        Physical[Physical Serial Port<br/>COMx]
    end

    Timer -.Every 500ms.-> Page
    Input --> Page
    Button --> SettingCtrl
    Settings --> SettingCtrl

    Page --> |Read Values| Input
    Page --> |Set Properties| DataObj
    DataObj --> |ToByteArray| Page
    Page --> |Send bytes| DeviceInst

    SettingCtrl --> |Configure| DeviceInst
    SettingCtrl --> |Start/Shutdown| DeviceInst

    DeviceInst --> |Open/Close| Serial
    DeviceInst --> |Write bytes| Serial
    Serial --> |Transmit| Physical

    style Timer fill:#ffebee
    style Page fill:#fff4e6
    style DeviceInst fill:#e0f2f1
    style Physical fill:#f3e5f5
```

---

## Supported Devices

| # | Device Name | Tab Header | Page Class | Device Class |
|---|-------------|------------|------------|--------------|
| 1 | TFO1 | TFO1 | TFO1EmulatorPage | TFO1Device |
| 2 | PH Meter | PH Meter | PHMeterEmulatorPage | PHMeterDevice |
| 3 | Weight QA | Weight QA | WeightQAEmulatorPage | WeightQADevice |
| 4 | Weight SPUN | Weight SPUN | WeightSPUNEmulatorPage | WeightSPUNDevice |
| 5 | JIK6CAB | JIK6CAB | JIK6CABEmulatorPage | JIK6CABDevice |
| 6 | DEFENDER3000 | DEFENDER3000 | CordDEFENDER3000EmulatorPage | CordDEFENDER3000Device |
| 7 | Mettler MS204TS00 | Mettler MS204TS00 | MettlerMS204TS00EmulatorPage | MettlerMS204TS00Device |

---

## Timer-Based Synchronization Flow

```mermaid
stateDiagram-v2
    [*] --> AppStarted: Window_Loaded
    AppStarted --> TimerInit: InitTimer()
    TimerInit --> TimerRunning: Start()

    TimerRunning --> CheckSync: Timer Elapsed (500ms)
    CheckSync --> SkipSync: _onSync = true
    SkipSync --> TimerRunning

    CheckSync --> AcquireLock: _onSync = false
    AcquireLock --> SetFlag: _onSync = true
    SetFlag --> Dispatch: Dispatcher.Invoke

    Dispatch --> SyncAll: SyncAndSendAll()

    SyncAll --> SyncTFO1
    SyncTFO1 --> SyncPH
    SyncPH --> SyncQA
    SyncQA --> SyncSPUN
    SyncSPUN --> SyncJIK
    SyncJIK --> SyncDEF
    SyncDEF --> SyncMettler

    SyncMettler --> ReleaseLock: _onSync = false
    ReleaseLock --> TimerRunning

    TimerRunning --> AppClosing: Window_Closing
    AppClosing --> TimerStop: FreeTimer()
    TimerStop --> DevicesFree: FreeDevices()
    DevicesFree --> [*]
```

---

## Configuration Management

### Config File Storage

**Location:** `{AppPath}/Configs/Devices/{DeviceName}.config.json`

**Example:** `WeightQA.config.json`
```json
{
  "DeviceName": "WeightQA",
  "PortName": "COM3",
  "BaudRate": 9600,
  "Parity": "None",
  "DataBits": 8,
  "StopBits": "One",
  "Handshake": "None"
}
```

### Config Lifecycle

1. **Load:** `Setup()` → `Device.Instance.LoadConfig()`
2. **Modify:** UI changes → `UpdateCurrentSetting()`
3. **Save:** `Device.SaveConfig()` → JSON file written
4. **Restore:** Next app launch → `LoadConfig()`

---

## Thread Safety Considerations

### UI Thread Synchronization

- Timer elapsed event uses `Dispatcher.Invoke` to marshal calls to UI thread
- `_onSync` flag prevents re-entrant timer callbacks
- Each page has its own `onSync` flag to prevent concurrent Sync() calls

### Device Thread Safety

- Serial port operations use internal `_lock` object
- RX queue operations are synchronized with `lock (_lock)`
- Background RX thread runs independently from UI

---

## Error Handling Strategy

### Serial Port Errors

```csharp
try
{
    _comm.Open();
}
catch (Exception ex2)
{
    med.Err(ex2.ToString());
}

if (!_comm.IsOpen)
{
    // Cleanup and return
    _comm.Close();
    _comm.Dispose();
    _comm = null;
    return;
}
```

### Data Parsing Errors

```csharp
try
{
    data.W = decimal.Parse(txtW.Text);
    data.Unit = txtUnit.Text.Trim().ToUpper();
    data.Mode = txtMode.Text.Trim().ToUpper();
}
catch { }  // Silent failure - uses previous values
```

---

## Usage Workflow

### Typical User Workflow

1. **Launch Application**
   - App starts with all devices stopped
   - Previous configurations loaded from JSON

2. **Select Device Tab**
   - Click on desired device tab

3. **Configure Serial Settings**
   - Select COM port from dropdown
   - Adjust baud rate if needed (default 9600)
   - Set parity, data bits, stop bits, handshake

4. **Click Start Button**
   - Device opens serial port
   - Button changes to "Shutdown"
   - Device begins transmitting

5. **Modify Device Data**
   - Enter weight value
   - Change unit (G, kg, etc.)
   - Update mode/status

6. **Auto-Transmission**
   - Timer sends data every 500ms
   - Changes reflected immediately on connected terminal

7. **Stop Emulation**
   - Click "Shutdown" button
   - Port closes, transmission stops

---

## Key Design Patterns

### 1. Singleton Pattern
- All device instances use singleton pattern
- Accessed via `DeviceName.Instance`

### 2. Template Method Pattern
- All emulator pages follow same structure:
  - `Setup()` - Initialize device and UI
  - `Sync()` - Synchronize UI to device and send

### 3. Observer Pattern (Implicit)
- Timer → MainWindow → EmulatorPages
- Event-driven architecture

### 4. Strategy Pattern
- Each device has its own `ToByteArray()` implementation
- Protocol-specific encoding strategies

---

## Testing Scenarios

### 1. Single Device Emulation
- Start one device
- Verify data transmission
- Change values in UI
- Confirm updates sent

### 2. Multi-Device Emulation
- Start multiple devices on different ports
- Verify no interference between devices
- Check timer cycles through all devices

### 3. Configuration Persistence
- Configure settings
- Close app
- Reopen app
- Verify settings restored

### 4. Error Conditions
- Invalid COM port
- Port already in use
- Invalid baud rate
- Disconnected cable

---

## Related Files

- **Application:** `09.App/NLib.Serial.Emulator.App/`
- **Core Devices:** `01.Core/NLib.Serial.Devices/`
- **Device Documentation:** `Documents/Design/Device-*.md`
- **Terminal App:** `09.App/NLib.Serial.Terminal.App/` (counterpart application)

---

## See Also

- [NLib.Serial.Devices Core Library](CODE_ANALYSIS_NLib.Serial.Devices.md)
- [Serial Terminal App Analysis](App-Terminal-Analysis.md)
- [Device Protocol Documentation](Device-01-CordDEFENDER3000.md)
