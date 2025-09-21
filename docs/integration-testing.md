# Coalition Desktop Shell - Integration Testing Guide

## Testing Overview

This document provides comprehensive testing procedures for the Coalition Desktop Shell to verify all systems work together correctly.

## Pre-Testing Setup

### Unity Project Configuration
1. **Open Unity 6**: Load the coalition-desktop project
2. **Verify Scene**: Ensure `Assets/Scenes/Desktop.unity` is loaded
3. **Check Components**: Verify the Coalition Desktop GameObject has all components attached
4. **Build Settings**: Configure for macOS Apple Silicon (IL2CPP backend, Metal graphics)
5. **Package Dependencies**: Ensure UI Toolkit package is installed and updated

### System Requirements
- **Unity 6.0.0f1** or later
- **macOS 12.0+** (Apple Silicon recommended for optimal performance)
- **Metal graphics API** support
- **8GB RAM minimum** (16GB recommended)

## Core System Testing

### 1. Desktop Environment Initialization
**Test Steps:**
1. Enter Play Mode in Unity Editor
2. Verify desktop background appears
3. Check dock appears at bottom with all 7 app icons
4. Confirm FPS counter can be toggled with F3 key
5. Verify welcome notifications appear sequentially

**Expected Results:**
- ✅ Desktop background renders correctly
- ✅ Dock shows: Mail, Chat, Polling, Calendar, Cabinet, Policy, Media icons
- ✅ FPS counter displays performance metrics
- ✅ Welcome notifications appear with proper timing (0.5s, 2s, 3.5s delays)

### 2. Window Management System
**Test Steps:**
1. Open any application from dock
2. Test window dragging across desktop
3. Test window resizing from corners and edges
4. Open multiple windows and test focus switching
5. Test snap-to-grid functionality by dragging to screen edges
6. Test minimize/restore functionality

**Expected Results:**
- ✅ Windows can be dragged smoothly within desktop bounds
- ✅ Windows resize properly from all 8 resize handles
- ✅ Focus switching works correctly (z-order management)
- ✅ Windows snap to half/quarter screen positions
- ✅ Minimized windows show indicators on dock

### 3. Application Functionality
**Test Each Application:**

#### Mail/Briefings App
1. Open from dock
2. Verify sample briefings load from JSON
3. Test message selection and reading
4. Check priority indicators display correctly

#### Chat/DMs App
1. Open from dock
2. Verify conversation list loads
3. Check unread message indicators
4. Test status indicators (online/offline/busy)

#### Polling Dashboard
1. Open from dock
2. Verify approval ratings display
3. Check party standings with trend indicators
4. Test issue support meters

#### Calendar/Rhythms
1. Open from dock
2. Verify daily schedule loads
3. Check event categorization (meeting/media/parliamentary)
4. Test priority indicators

#### Cabinet Ministers
1. Open from dock
2. Verify minister list loads
3. Check approval ratings and status indicators
4. Test portfolio information display

#### Policy Builder
1. Open from dock
2. Verify policy proposals load
3. Check status progression indicators
4. Test support level displays

#### Media Monitor
1. Open from dock
2. Verify news headlines load
3. Check sentiment analysis indicators
4. Test timestamp displays

### 4. Data Management System
**Test Steps:**
1. Verify all JSON files load correctly at startup
2. Test default data creation for missing files
3. Check error handling for malformed JSON
4. Test data persistence across sessions

**Expected Results:**
- ✅ All 7 data files load without errors
- ✅ Default data created if files missing
- ✅ Graceful error handling for data issues
- ✅ Data consistency maintained

### 5. Notification System
**Test Steps:**
1. Trigger notifications through various actions
2. Test notification types (info, success, warning, error)
3. Check notification positioning and stacking
4. Test auto-hide functionality
5. Test manual dismissal with close button

**Expected Results:**
- ✅ Notifications appear in top-right corner
- ✅ Multiple notifications stack properly
- ✅ Auto-hide after specified duration
- ✅ Manual close button works correctly

### 6. Performance Monitoring
**Test Steps:**
1. Toggle FPS counter with F3
2. Monitor performance during window operations
3. Test performance warnings for low FPS
4. Check resource usage during multi-window scenarios

**Expected Results:**
- ✅ Maintains 60 FPS during normal operation
- ✅ Performance warnings appear below 30 FPS
- ✅ Smooth animations and transitions
- ✅ Responsive user interactions

## User Experience Testing

### 1. Satirical Content Quality
**Review Criteria:**
- Political content remains respectful and educational
- Fictional characters and parties are clearly satirical
- No real political figures or parties referenced
- Content promotes civic understanding

### 2. Accessibility Testing
**Test Points:**
- High contrast mode functionality
- Reduced motion preferences
- Keyboard navigation support
- Screen reader compatibility preparation

### 3. Usability Testing
**Test Scenarios:**
1. New user opens application for first time
2. User manages multiple windows simultaneously
3. User reviews daily briefings and schedules
4. User monitors polling and media sentiment

## Build Testing

### 1. Editor Testing
**Complete all above tests in Unity Editor Play Mode**

### 2. Standalone Build Testing
**Test Steps:**
1. Build for macOS (Apple Silicon)
2. Test application launch
3. Verify all systems work in build
4. Check performance on target hardware
5. Test .app bundle integrity

**Expected Results:**
- ✅ Clean build with no errors
- ✅ All functionality works in standalone build
- ✅ Performance meets 60 FPS target
- ✅ Proper .app bundle structure

## Issue Resolution Guide

### Common Issues and Fixes

#### Missing UI Elements
- **Cause**: Visual Tree Asset not assigned
- **Fix**: Assign Desktop.uxml to UIDocument component

#### Performance Issues
- **Cause**: Inefficient rendering or excessive UI updates
- **Fix**: Check FPS counter, optimize UI complexity

#### Data Loading Failures
- **Cause**: Missing or malformed JSON files
- **Fix**: Verify JSON files in Resources/Data/ folder

#### Window Management Problems
- **Cause**: Missing manipulator components
- **Fix**: Ensure all manipulators are properly attached

#### Styling Issues
- **Cause**: USS files not loading or incorrect class names
- **Fix**: Verify USS file references and class assignments

## Quality Assurance Checklist

### ✅ Functional Requirements
- [ ] All 7 applications launch and display content
- [ ] Window management works correctly
- [ ] Data loads from JSON files
- [ ] Notifications system functions
- [ ] FPS counter operates properly

### ✅ Performance Requirements
- [ ] Maintains 60 FPS on Apple Silicon
- [ ] Smooth window operations
- [ ] Responsive user interface
- [ ] Efficient memory usage

### ✅ Content Requirements
- [ ] Satirical content is appropriate
- [ ] Educational value present
- [ ] No offensive material
- [ ] Clear fictional context

### ✅ Technical Requirements
- [ ] Unity 6 compatibility
- [ ] macOS build success
- [ ] IL2CPP backend works
- [ ] Metal graphics API utilization

## Sign-off Criteria

The Coalition Desktop Shell is ready for delivery when:

1. **All functional tests pass** ✅
2. **Performance targets met** ✅
3. **Build deploys successfully** ✅
4. **Content guidelines followed** ✅
5. **Documentation complete** ✅

---

**Testing completed by**: Development Team
**Testing date**: [To be filled during actual testing]
**Build version**: 1.0.0
**Unity version**: 6.0.0f1
**Target platform**: macOS Apple Silicon