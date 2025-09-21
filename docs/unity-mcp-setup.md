# Unity MCP Setup Guide for Coalition Desktop Shell

## Overview
This guide provides step-by-step instructions for setting up the Coalition Desktop Shell using Unity MCP tools once Unity Editor is running.

## Prerequisites
1. Unity 6.0.0f1 or later installed
2. Unity MCP Bridge running
3. Project opened in Unity Editor
4. GitHub repository cloned to local machine

## MCP-Assisted Setup Process

### Step 1: Verify Unity Editor State
```bash
# Check if Unity Editor is running and responsive
manage_editor action='get_state'
```

### Step 2: Load Main Scene
```bash
# Load the desktop scene we created
manage_scene action='load' name='Desktop' path='Assets/Scenes/'
```

### Step 3: Create Missing Meta Files
Unity may need to generate .meta files for all our assets. Let the editor process all imports first.

### Step 4: Verify Script Compilation
```bash
# Check console for any compilation errors
read_console action='get' types=['error'] format='detailed'
```

### Step 5: Create Prefabs for Reusable Components

#### Window Prefab
```bash
# Create a base window prefab that can be reused
manage_asset action='create' path='Assets/Prefabs/BaseWindow.prefab' asset_type='Prefab'
```

#### Application Prefabs
Create prefabs for each of the 7 applications:
```bash
manage_asset action='create' path='Assets/Prefabs/Applications/MailApp.prefab' asset_type='Prefab'
manage_asset action='create' path='Assets/Prefabs/Applications/ChatApp.prefab' asset_type='Prefab'
manage_asset action='create' path='Assets/Prefabs/Applications/PollingApp.prefab' asset_type='Prefab'
manage_asset action='create' path='Assets/Prefabs/Applications/CalendarApp.prefab' asset_type='Prefab'
manage_asset action='create' path='Assets/Prefabs/Applications/CabinetApp.prefab' asset_type='Prefab'
manage_asset action='create' path='Assets/Prefabs/Applications/PolicyApp.prefab' asset_type='Prefab'
manage_asset action='create' path='Assets/Prefabs/Applications/MediaApp.prefab' asset_type='Prefab'
```

### Step 6: Configure Scene GameObjects

#### Create Main Camera
```bash
# Ensure scene has a main camera
manage_gameobject action='find' search_term='Main Camera' search_method='by_name'
# If not found, create one:
manage_gameobject action='create' name='Main Camera' components_to_add=['Camera', 'AudioListener']
```

#### Setup Coalition Desktop GameObject
```bash
# Find and configure the main desktop controller
manage_gameobject action='find' search_term='Coalition Desktop' search_method='by_name'
# If found, verify components are attached:
manage_gameobject action='get_components' target='Coalition Desktop' search_method='by_name'
```

#### Configure UI Document
```bash
# Verify UIDocument component has the correct Visual Tree Asset assigned
manage_gameobject action='modify' target='Coalition Desktop' search_method='by_name' component_properties='{
  "UIDocument": {
    "visualTreeAsset": "Assets/UI/Desktop/Desktop.uxml",
    "panelSettings": "Assets/UI/Settings/PanelSettings.asset"
  }
}'
```

### Step 7: Create Material Assets
```bash
# Create materials for UI elements if needed
manage_asset action='create' path='Assets/Materials/UIBackground.mat' asset_type='Material' properties='{
  "shader": "UI/Default",
  "color": [0.2, 0.25, 0.3, 1.0]
}'
```

### Step 8: Test Scene Functionality

#### Enter Play Mode
```bash
manage_editor action='play'
```

#### Monitor Console for Errors
```bash
read_console action='get' types=['error', 'warning'] count=10 format='detailed'
```

#### Exit Play Mode
```bash
manage_editor action='stop'
```

### Step 9: Save Scene and Project
```bash
# Save the scene with all configurations
manage_scene action='save'

# Save the project
manage_editor action='save_project'
```

## Asset Creation Strategy

### Folder Structure Verification
The following assets should be automatically imported:

```
Assets/
├── Resources/
│   └── Data/               # JSON data files
├── Scenes/
│   └── Desktop.unity       # Main scene
├── Scripts/
│   ├── Applications/       # App scripts
│   ├── Core/              # Core system scripts
│   ├── Data/              # Data management
│   ├── UI/                # UI components
│   └── Utils/             # Utility scripts
├── UI/
│   ├── Desktop/           # Main UI structure
│   ├── Settings/          # Panel settings
│   ├── Shared/            # Shared stylesheets
│   └── Windows/           # Window templates
├── Materials/             # UI materials (to be created)
└── Prefabs/              # Reusable prefabs (to be created)
    └── Applications/      # App prefabs
```

### Performance Optimization
1. **Batching**: Use UI Toolkit's built-in batching for optimal performance
2. **Texture Atlasing**: Combine UI textures into atlases
3. **LOD**: Implement level-of-detail for complex UI elements
4. **Culling**: Use appropriate culling for off-screen elements

### Testing Checklist

#### Functional Tests
- [ ] All 7 applications launch correctly
- [ ] Window dragging works within bounds
- [ ] Window resizing functions properly
- [ ] Snap-to-grid system activates
- [ ] Dock icons respond to clicks
- [ ] Notifications appear and dismiss correctly
- [ ] FPS counter toggles with F3
- [ ] JSON data loads successfully

#### Performance Tests
- [ ] Maintains 60 FPS during normal operation
- [ ] Smooth window animations
- [ ] Responsive UI interactions
- [ ] Memory usage remains stable

#### Integration Tests
- [ ] Scene loads without errors
- [ ] All scripts compile successfully
- [ ] UI elements render correctly
- [ ] Data persistence works across sessions

## Troubleshooting

### Common Issues and MCP Solutions

#### Script Compilation Errors
```bash
# Check for compilation errors
read_console action='get' types=['error'] format='detailed'

# If errors found, check specific scripts
manage_script action='read' name='CoalitionDesktop' path='Assets/Scripts/Core/'
```

#### Missing UI Elements
```bash
# Verify UI Document configuration
manage_gameobject action='get_components' target='Coalition Desktop' search_method='by_name'

# Check if Visual Tree Asset is assigned
manage_asset action='get_info' path='Assets/UI/Desktop/Desktop.uxml'
```

#### Performance Issues
```bash
# Monitor frame rate during play mode
manage_editor action='play'
# Check console for performance warnings
read_console action='get' filter_text='performance' format='detailed'
```

### Menu Operations
If you need to access Unity menu items:

```bash
# List available menu items
manage_menu_item action='list' search='Build'

# Execute build operations
manage_menu_item action='execute' menu_path='File/Build Settings'

# Package Manager operations
manage_menu_item action='execute' menu_path='Window/Package Manager'
```

## Build Configuration

### macOS Apple Silicon Build
```bash
# Open build settings
manage_menu_item action='execute' menu_path='File/Build Settings'

# Configure player settings via menu
manage_menu_item action='execute' menu_path='Edit/Project Settings'
```

### Recommended Build Settings
- **Platform**: macOS
- **Architecture**: Apple Silicon
- **Scripting Backend**: IL2CPP
- **Graphics API**: Metal
- **Target SDK**: macOS 12.0+

## Next Steps

Once Unity Editor is running with MCP Bridge:

1. **Execute Setup Commands**: Run through the MCP commands above
2. **Test Functionality**: Verify all systems work as expected
3. **Build for Distribution**: Create macOS build for testing
4. **Performance Validation**: Confirm 60 FPS target achievement
5. **Documentation Update**: Record any issues and solutions

This MCP-assisted setup ensures the Coalition Desktop Shell is properly configured and tested within Unity Editor.