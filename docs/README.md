# COALITION Desktop Shell

A satirical desktop environment simulation built with Unity 6 and UI Toolkit, designed for macOS Apple Silicon.

## Quick Start

### Prerequisites
- Unity 6.0 LTS or newer
- macOS with Apple Silicon (M1/M2/M3)
- Xcode for code signing (for distribution)

### Running the Project
1. Open Unity Hub and add this project
2. Open the project in Unity
3. Load the `Assets/Scenes/Desktop.unity` scene
4. Press Play to run the desktop simulation

### Building for Distribution
1. Go to File > Build Settings
2. Select macOS platform
3. Click "Build" and choose output location
4. The resulting .app can be distributed to other macOS machines

## Project Structure

```
Assets/
├── Scripts/          # C# game logic
│   ├── Core/         # Desktop system managers
│   ├── UI/           # Window management components
│   ├── Applications/ # Individual app controllers
│   ├── Data/         # Data management system
│   └── Utils/        # Utility classes
├── UI/               # UXML/USS interface files
│   ├── Desktop/      # Main desktop interface
│   ├── Windows/      # Window chrome and controls
│   ├── Applications/ # Individual app interfaces
│   └── Shared/       # Common UI components
├── Data/             # JSON data fixtures
├── Textures/         # UI graphics and icons
└── Scenes/           # Unity scene files
```

## Key Features

- **Window Management**: Drag, resize, minimize, and snap windows
- **Application Suite**: 7 satirical political applications
- **Performance Optimized**: Maintains 60 FPS on Apple Silicon
- **Offline Operation**: No network dependencies
- **Satirical Content**: Political humor with ethical guardrails

## Documentation

- [Vision & Design Philosophy](VISION.md)
- [Feature Specifications](FEATURES.md)
- [Technology Stack Rationale](STACK_CHOICE.md)
- [Political Context](DUTCH_POLITICS.md)
- [Social & Media Dynamics](SOCIAL_AND_MEDIA.md)
- [Ethics & Content Guidelines](ETHICS.md)

## Development

### Testing
- Run PlayMode tests: Window > General > Test Runner
- Performance monitoring: F3 key toggles FPS counter
- Manual testing checklist in `Assets/Tests/`

### Contributing
1. Follow the existing code style and architecture
2. Maintain 60 FPS performance target
3. Ensure satirical content follows ethics guidelines
4. Test on Apple Silicon hardware before submitting

## License

This project is a demonstration/educational tool. See ethics guidelines for content usage restrictions.

---

*Built with Unity 6 + UI Toolkit for maximum performance and modern UI capabilities.*