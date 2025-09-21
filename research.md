# Unity UI Toolkit Desktop Window Manager Research
*Research compiled: September 21, 2025*

## Executive Summary

Unity UI Toolkit provides a modern, performance-oriented UI framework for creating desktop applications with window management capabilities. While UI Toolkit excels at creating scalable interfaces, implementing desktop-style window management (drag/resize/snap/focus) requires custom manipulator implementations.

## Key Findings

### 1. Unity UI Toolkit Capabilities (Unity 6, 2024-2025)

**Strengths:**
- Modern web-inspired technology stack (UXML, USS, C#)
- High performance compared to legacy uGUI
- Excellent cross-platform support including macOS
- Built-in flexbox layout system
- Comprehensive event handling system
- Focus management with configurable focus order

**Runtime Window Management Status:**
- ✅ **Basic windowing**: Possible through custom implementation
- ✅ **Drag functionality**: Achievable via custom manipulators
- ✅ **Resize capability**: Implementable with pointer event handling
- ⚠️ **Window snapping**: Requires custom logic implementation
- ⚠️ **Z-order management**: Limited when mixing with other UI systems
- ❌ **Native minimize/maximize**: Not directly supported
- ❌ **Independent OS windows**: Not possible at runtime

### 2. Draggable/Resizable Implementation Patterns

**Core Implementation Approach:**
```csharp
// Based on community examples from 2024
public class DragManipulator : PointerManipulator
{
    // Register PointerDownEvent, PointerMoveEvent, PointerUpEvent
    // Handle state machine: idle → dragging → dropped/reset
    // Manage CSS classes for visual feedback
    // Support custom drop validation
}
```

**Event Handling Pattern:**
1. **PointerDownEvent**: Capture pointer, store initial position, enter drag state
2. **PointerMoveEvent**: Calculate delta, update element transform, validate drop targets
3. **PointerUpEvent**: Release pointer, finalize drop or reset, clean up state

**Best Practices Found:**
- Extend `PointerManipulator` base class for mouse/touch compatibility
- Use `activators` to control interaction conditions
- Implement proper pointer capture/release cycles
- Add CSS classes for visual feedback during interactions
- Stop event propagation to prevent interference

### 3. Community Examples and Resources

**Key Community Resources (2024-2025):**
- **DragManipulator.cs** by Shane Celis: Comprehensive draggable element implementation
  - GitHub Gist: `gist.github.com/shanecelis/b6fb3fe8ed5356be1a3aeeb9e7d2c145`
  - Features: CSS class management, drop target validation, transform position updates

- **Unity Technologies UIElementsExamples**: Official drag-and-drop examples
  - Repository: `github.com/Unity-Technologies/UIElementsExamples`
  - File: `Assets/Examples/Editor/E20_DragAndDrop.cs`

- **Unity Discussions Community**: Active discussion on runtime window solutions
  - Topics: Resizable visual elements, draggable windows, desktop-style interfaces

### 4. macOS Build Configuration (Unity 6)

**Build Process (2024 Standards):**
- **Build Profiles Window**: File > Build Profiles → Add Build Profile → macOS
- **Architecture Support**: Intel, Apple Silicon, or Universal builds
- **Chipset Considerations**: Automatic detection of target hardware
- **Code Signing**: Required for distribution outside development

**UI Toolkit macOS Compatibility:**
- ✅ Full UI Toolkit support on macOS
- ✅ Retina display scaling handled automatically
- ✅ Native pointer events (mouse, trackpad, touch bar)
- ✅ Focus ring and keyboard navigation
- ✅ Accessibility features integration

### 5. Constraints and Limitations

**Technical Constraints:**
1. **No Native OS Windows**: UI Toolkit cannot create independent OS-level windows at runtime
2. **Z-Order Mixing**: Issues when combining UI Toolkit with uGUI systems
3. **Animation Limitations**: No built-in animation system for window transitions
4. **Touch Input Variations**: Some reported issues with touch events on specific mobile configurations

**Development Constraints:**
1. **Custom Implementation Required**: No built-in window manager components
2. **Performance Considerations**: Manual optimization needed for complex window hierarchies
3. **Event Propagation Management**: Careful handling required to prevent conflicts
4. **CSS Integration**: Window styling requires understanding of USS system

### 6. Minimum Window Manager Feature Requirements

**Essential Features for Desktop Simulation:**

**Drag System:**
- Custom `DragManipulator` extending `PointerManipulator`
- Pointer capture/release management
- Transform position updates
- Visual feedback through CSS classes

**Resize System:**
- Edge/corner detection for resize handles
- Min/max size constraints
- Proportional resize support
- Style property updates (width/height)

**Focus/Z-Order System:**
- Focus ring management for keyboard navigation
- Custom z-index ordering for visual elements
- Event propagation control
- Active window highlighting

**Window Lifecycle:**
- Window creation/destruction management
- State persistence (position, size, minimized state)
- Parent-child window relationships
- Modal dialog support

**Desktop Integration:**
- Window snapping to screen edges
- Multi-monitor awareness (Unity canvas level)
- Keyboard shortcuts (minimize, close, etc.)
- Title bar with standard controls

## Implementation Recommendations

### Phase 1: Core Window System
1. Implement base `WindowElement` class extending `VisualElement`
2. Create `DragManipulator` for title bar dragging
3. Develop `ResizeManipulator` for edge/corner resizing
4. Build focus management system

### Phase 2: Advanced Features
1. Add window snapping logic
2. Implement minimize/restore animations
3. Create window manager singleton for coordination
4. Add keyboard shortcuts support

### Phase 3: Polish and Integration
1. macOS-specific optimizations
2. Accessibility compliance
3. Performance profiling and optimization
4. Integration testing across Unity 6 features

## Citations and Version Information

**Primary Sources:**
- Unity UI Toolkit Documentation (Unity 6.0, 2024)
- Shane Celis DragManipulator (GitHub, 2024)
- Unity Discussions Forum (2024-2025 threads)
- Unity Official Blog Posts (Unity 6 UI Toolkit Updates, 2024)

**Version Context:**
- Unity Version: Unity 6 (2024 LTS)
- UI Toolkit Package: com.unity.ui (latest stable)
- Target Platform: macOS (Intel + Apple Silicon)
- Research Date: September 21, 2025

**Community Resources:**
- Unity Discussions: Runtime UI threads (ongoing 2024-2025)
- GitHub Examples: UIElementsExamples repository
- Unite 2024: UI Toolkit presentations and demos

## Conclusion

Unity UI Toolkit provides a solid foundation for desktop window management systems, but requires significant custom implementation. The framework's event system and manipulator architecture are well-suited for creating draggable, resizable windows, though developers must build window management features from scratch. For desktop simulation applications, the combination of custom manipulators, proper focus management, and careful event handling can achieve professional-quality windowing experiences on macOS and other platforms.