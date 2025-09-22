# Unity MCP Setup Commands - Coalition Desktop Shell

## Prerequisites
1. Unity 6000.2.5f1 running with the Coalition Desktop project loaded
2. Unity MCP Bridge running and connected
3. Run these commands in order once Unity is responsive

## Step 1: Verify Unity Connection and State

```bash
# Test Unity connection
manage_editor action='get_state'
```

**Expected Output**: Unity should respond with editor state information.

## Step 2: Check Console for Errors

```bash
# Check for compilation errors first
read_console action='get' types=['error'] format='detailed'
```

**Look for**: Script compilation errors, missing references, UI Toolkit errors.

## Step 3: Load and Verify Scene Setup

```bash
# Load the main desktop scene
manage_scene action='load' name='Desktop' path='Assets/Scenes/'

# Verify scene loaded correctly
manage_scene action='get_hierarchy'
```

## Step 4: Verify GameObject Setup

```bash
# Find the Coalition Desktop GameObject
manage_gameobject action='find' search_term='Coalition Desktop' search_method='by_name'

# Get all components on the Coalition Desktop GameObject
manage_gameobject action='get_components' target='Coalition Desktop' search_method='by_name' includeNonPublicSerialized=true
```

**Expected Components**:
- `Transform`
- `UIDocument`
- `CoalitionDesktop` (script)

## Step 5: Create Missing Assets if Needed

### Create Proper Panel Settings
```bash
# Create panel settings with correct configuration
manage_asset action='create' path='Assets/UI/Settings/MainPanelSettings.asset' asset_type='PanelSettings' properties='{
  "referenceResolution": {"x": 1920, "y": 1080},
  "scaleMode": 1,
  "sortingOrder": 1,
  "clearColor": false
}'
```

### Create Test UI Document GameObject
```bash
# Create a simple test UI GameObject
manage_gameobject action='create' name='UI Test' components_to_add=['UIDocument'] component_properties='{
  "UIDocument": {
    "visualTreeAsset": "Assets/UI/Test/SimpleTest.uxml",
    "panelSettings": "Assets/UI/Settings/TestPanelSettings.asset"
  }
}'
```

## Step 6: Configure Main UI Document

```bash
# Configure the Coalition Desktop UIDocument component
manage_gameobject action='modify' target='Coalition Desktop' search_method='by_name' component_properties='{
  "UIDocument": {
    "visualTreeAsset": "Assets/UI/Desktop/Desktop.uxml",
    "panelSettings": "Assets/UI/Settings/PanelSettings.asset"
  }
}'
```

## Step 7: Test UI Functionality

```bash
# Enter Play Mode to test
manage_editor action='play'

# Wait a moment, then check console for runtime errors
read_console action='get' types=['error', 'warning'] count=10 format='detailed'

# Exit Play Mode
manage_editor action='stop'
```

## Step 8: Asset Verification

```bash
# Verify all required assets exist
manage_asset action='get_info' path='Assets/UI/Desktop/Desktop.uxml'
manage_asset action='get_info' path='Assets/UI/Settings/PanelSettings.asset'
manage_asset action='get_info' path='Assets/Scripts/Core/CoalitionDesktop.cs'
```

## Troubleshooting Commands

### If Scripts Won't Compile
```bash
# Check for script errors
read_console action='get' types=['error'] filter_text='error CS' format='detailed'

# Force recompilation
manage_menu_item action='execute' menu_path='Assets/Refresh'
```

### If UI Still Not Appearing
```bash
# Create minimal test setup
manage_gameobject action='create' name='Debug UI Test' components_to_add=['UIDocument']

# Set up with simple test UI
manage_gameobject action='modify' target='Debug UI Test' search_method='by_name' component_properties='{
  "UIDocument": {
    "visualTreeAsset": "Assets/UI/Test/SimpleTest.uxml",
    "panelSettings": "Assets/UI/Settings/TestPanelSettings.asset"
  }
}'

# Test in Play Mode
manage_editor action='play'
```

### Check Camera Setup
```bash
# Verify Main Camera exists
manage_gameobject action='find' search_term='Main Camera' search_method='by_name'

# If not found, create one
manage_gameobject action='create' name='Main Camera' components_to_add=['Camera', 'AudioListener'] component_properties='{
  "Camera": {
    "clearFlags": 1,
    "backgroundColor": [0.2, 0.25, 0.3, 1.0],
    "orthographic": false
  }
}'
```

## Complete Setup Validation

```bash
# Run all validation checks
manage_gameobject action='find' search_term='Coalition Desktop' search_method='by_name'
manage_gameobject action='find' search_term='Main Camera' search_method='by_name'
read_console action='get' types=['error'] count=5 format='detailed'
manage_scene action='save'
```

## Menu Operations for Package Management

```bash
# List available menu items
manage_menu_item action='list' search='Package'

# Open Package Manager if needed
manage_menu_item action='execute' menu_path='Window/Package Manager'

# Refresh assets if needed
manage_menu_item action='execute' menu_path='Assets/Refresh'
```

## Build and Deploy Commands

```bash
# Open Build Settings
manage_menu_item action='execute' menu_path='File/Build Settings'

# Configure for macOS (requires manual configuration in Build Settings window)
# Set target to macOS, Architecture to Apple Silicon, Scripting Backend to IL2CPP

# Save project
manage_editor action='save_project'
```

## Emergency Reset Commands

If everything fails, try this sequence:

```bash
# Clear console
read_console action='clear'

# Refresh all assets
manage_menu_item action='execute' menu_path='Assets/Refresh'

# Force recompile scripts
manage_menu_item action='execute' menu_path='Assets/Reimport All'

# Load basic scene
manage_scene action='load' name='UITest' path='Assets/Scenes/'

# Test basic UI
manage_editor action='play'
```

## Expected Success Indicators

✅ **Console shows no errors**
✅ **UIDocument component has assets assigned**
✅ **Simple test UI appears in Play Mode**
✅ **Coalition Desktop scene loads without errors**
✅ **All required GameObjects present in scene**

## Common Error Patterns to Watch For

❌ `VisualTreeAsset could not be loaded`
❌ `PanelSettings is null`
❌ `error CS0246: The type or namespace name could not be found`
❌ `NullReferenceException in CoalitionDesktop.Start()`

## Next Steps After Success

Once basic UI is working:
1. Test all 7 applications from the dock
2. Verify window management (drag, resize, snap)
3. Check FPS counter (F3 toggle)
4. Test notification system
5. Validate data loading from JSON files

This systematic approach using Unity MCP should resolve the UI issues and get the Coalition Desktop Shell fully functional!