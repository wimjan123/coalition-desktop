# UI Troubleshooting Guide - Coalition Desktop Shell

## Problem: No UI Appearing in Unity

This guide will help you get the UI Toolkit interface working in Unity Editor.

## Quick Diagnosis Checklist

### 1. Check Scene Setup
In Unity Editor:
- [ ] Is `Assets/Scenes/Desktop.unity` loaded?
- [ ] Is there a GameObject named "Coalition Desktop" in the scene?
- [ ] Does it have a `UIDocument` component?
- [ ] Does it have a `CoalitionDesktop` script component?

### 2. Check UIDocument Configuration
Select the "Coalition Desktop" GameObject and verify:
- [ ] **Visual Tree Asset**: Should point to `Assets/UI/Desktop/Desktop.uxml`
- [ ] **Panel Settings**: Should point to `Assets/UI/Settings/PanelSettings.asset`
- [ ] **Sort Order**: Should be 0 or higher

### 3. Check Console for Errors
Look for these common errors:
- `NullReferenceException` in UI scripts
- `VisualTreeAsset not found` errors
- `PanelSettings not assigned` warnings
- CSS parsing errors

## Step-by-Step Fix

### Step 1: Create a Minimal Test Scene

1. **Create New Scene**:
   - File → New Scene → Basic (Built-in)
   - Save as `Assets/Scenes/UITest.unity`

2. **Add UI Document**:
   - Create Empty GameObject → Rename to "UI Root"
   - Add Component → UI Toolkit → UI Document

3. **Create Simple UXML**:
   - Create `Assets/UI/Test/SimpleTest.uxml`

### Step 2: Use This Simple Test UXML

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement style="flex-grow: 1; background-color: rgba(0, 100, 200, 0.5);">
        <ui:Label text="UI Toolkit Test" style="font-size: 32px; color: white; -unity-text-align: middle-center; flex-grow: 1;" />
        <ui:Button text="Test Button" style="font-size: 20px; height: 60px; margin: 20px;" />
    </ui:VisualElement>
</ui:UXML>
```

### Step 3: Create Panel Settings

1. **Create Panel Settings**:
   - Right-click in Project → Create → UI Toolkit → Panel Settings Asset
   - Name it `TestPanelSettings`
   - Set Reference Resolution to 1920x1080
   - Set Scale Mode to "Scale With Screen Size"

### Step 4: Connect Everything

1. **Assign Assets**:
   - Select UI Root GameObject
   - Drag `SimpleTest.uxml` to Visual Tree Asset field
   - Drag `TestPanelSettings` to Panel Settings field

2. **Test**:
   - Press Play
   - You should see a blue background with white text

## If Simple Test Works

The basic UI Toolkit is functioning. The issue is with the Coalition Desktop setup.

### Fix Coalition Desktop

1. **Check Assembly References**:
   - Verify `Coalition.asmdef` includes "UnityEngine.UIElementsModule"

2. **Verify Script Compilation**:
   - Check Console for compilation errors
   - All scripts must compile before UI can work

3. **Check Resource Loading**:
   - Verify `Desktop.uxml` is in correct location
   - Check that `PanelSettings.asset` exists

4. **Debug the CoalitionDesktop Script**:
   Add debug logs to see what's happening:

```csharp
private void Start()
{
    Debug.Log("CoalitionDesktop Start() called");
    if (uiDocument == null)
    {
        Debug.LogError("UIDocument is null!");
        return;
    }

    if (uiDocument.visualTreeAsset == null)
    {
        Debug.LogError("Visual Tree Asset is null!");
        return;
    }

    Debug.Log("UI Document configured correctly");
    // ... rest of initialization
}
```

## Common Issues and Solutions

### Issue 1: Black Screen
**Cause**: Panel Settings not configured correctly
**Solution**:
- Create new Panel Settings asset
- Set Reference Resolution: 1920x1080
- Set Scale Mode: Scale With Screen Size

### Issue 2: UI Elements Not Responding
**Cause**: Missing event system or incorrect sorting order
**Solution**:
- Ensure Panel Settings Sort Order is 0 or higher
- Check for EventSystem in scene (usually automatic)

### Issue 3: Styles Not Applied
**Cause**: USS files not loading or incorrect paths
**Solution**:
- Verify USS files are in correct locations
- Check Console for CSS parsing errors
- Ensure classes match between UXML and USS

### Issue 4: Scripts Not Running
**Cause**: Compilation errors or missing assembly references
**Solution**:
- Check Console for compilation errors
- Verify Assembly Definition files are correct
- Ensure all required packages are installed

## Emergency Reset Procedure

If nothing works, try this complete reset:

1. **Delete** `Library` folder (Unity will regenerate)
2. **Reimport All**: Assets → Reimport All
3. **Check Package Manager**: Window → Package Manager
   - Ensure "UI Toolkit" package is installed
4. **Create Fresh Scene** using the simple test above
5. **Gradually add complexity** once basic UI works

## Unity MCP Debugging

If you have Unity MCP available:

```bash
# Check Unity state
manage_editor action='get_state'

# Check console for errors
read_console action='get' types=['error'] format='detailed'

# Verify scene objects
manage_gameobject action='find' search_term='Coalition Desktop' search_method='by_name'

# Check components
manage_gameobject action='get_components' target='Coalition Desktop' search_method='by_name'
```

## Contact Points

If you're still having issues:
1. Share the Console output (especially any red error messages)
2. Confirm which Unity version you're using
3. Check if the simple test UXML works
4. Verify the GameObject setup in the scene

The Coalition Desktop Shell should work perfectly once UI Toolkit is properly configured!