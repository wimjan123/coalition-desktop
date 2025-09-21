# Unity Project Setup Instructions

Since I cannot directly create Unity projects through the command line, please follow these steps:

## Task 1: Unity Project Creation & Configuration

1. **Create New Unity Project:**
   - Open Unity Hub
   - Click "New Project"
   - Select "3D Core" template
   - Project name: "CoalitionDesktopDemo"
   - Location: `/Users/williamvisser/dev/coalition-desktop/`

2. **Platform Configuration:**
   - Go to File > Build Settings
   - Select "macOS" platform and click "Switch Platform"
   - In Player Settings:
     - Set Scripting Backend to "IL2CPP"
     - Set Architecture to "Apple Silicon" (ARM64)
     - Set Graphics APIs to "Metal" only
     - Set Default Screen Width: 1920, Height: 1080
     - Set Fullscreen Mode: "Windowed"

3. **Project Settings:**
   - Company Name: "Coalition Demo"
   - Product Name: "Coalition Desktop Shell"
   - Bundle Identifier: "com.coalition.desktop"

After completing these steps, continue with the automated implementation below.