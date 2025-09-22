#!/bin/bash

# Unity MCP Test Script for Coalition Desktop Shell
# Run this script when Unity Editor is running with MCP Bridge connected

echo "🏛️ Coalition Desktop Shell - Unity MCP Setup Test"
echo "=================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to run MCP command and check result
run_mcp_command() {
    local command="$1"
    local description="$2"

    echo -e "${BLUE}Testing: ${description}${NC}"
    echo "Command: $command"

    # Execute the command (you'll need to replace this with actual MCP execution)
    # For now, we'll just echo the command
    echo "  → Would execute: $command"
    echo ""
}

# Step 1: Verify Unity Connection
echo -e "${YELLOW}Step 1: Verifying Unity Connection${NC}"
run_mcp_command "manage_editor action='get_state'" "Unity Editor connection"

# Step 2: Check for Errors
echo -e "${YELLOW}Step 2: Checking Console for Errors${NC}"
run_mcp_command "read_console action='get' types=['error'] format='detailed'" "Console error check"

# Step 3: Load Scene
echo -e "${YELLOW}Step 3: Loading Main Scene${NC}"
run_mcp_command "manage_scene action='load' name='Desktop' path='Assets/Scenes/'" "Load Desktop scene"

# Step 4: Verify GameObjects
echo -e "${YELLOW}Step 4: Verifying GameObject Setup${NC}"
run_mcp_command "manage_gameobject action='find' search_term='Coalition Desktop' search_method='by_name'" "Find Coalition Desktop GameObject"
run_mcp_command "manage_gameobject action='get_components' target='Coalition Desktop' search_method='by_name'" "Check components"

# Step 5: Test Simple UI
echo -e "${YELLOW}Step 5: Testing Simple UI${NC}"
run_mcp_command "manage_scene action='load' name='UITest' path='Assets/Scenes/'" "Load UI test scene"
run_mcp_command "manage_editor action='play'" "Enter Play Mode"

echo ""
echo -e "${GREEN}✅ Setup test sequence complete!${NC}"
echo ""
echo "To run these commands manually:"
echo "1. Ensure Unity Editor is running with Coalition Desktop project"
echo "2. Ensure Unity MCP Bridge is connected"
echo "3. Execute each command from the UNITY_MCP_SETUP_COMMANDS.md file"
echo ""
echo "Expected results:"
echo "  ✅ Unity responds to MCP commands"
echo "  ✅ No compilation errors in console"
echo "  ✅ Coalition Desktop GameObject found with UIDocument component"
echo "  ✅ Simple UI test shows blue screen with text"
echo ""
echo "If any step fails, refer to docs/UI_TROUBLESHOOTING.md for detailed fixes"

# Additional diagnostic information
echo ""
echo -e "${BLUE}Project Information:${NC}"
echo "  Project: Coalition Desktop Shell"
echo "  Unity Version: 6000.2.5f1+"
echo "  Platform: macOS Apple Silicon"
echo "  UI System: UI Toolkit (UXML/USS)"
echo ""
echo "  Key Files to Verify:"
echo "    - Assets/Scenes/Desktop.unity (main scene)"
echo "    - Assets/Scenes/UITest.unity (test scene)"
echo "    - Assets/UI/Desktop/Desktop.uxml (main UI)"
echo "    - Assets/UI/Test/SimpleTest.uxml (test UI)"
echo "    - Assets/Scripts/Core/CoalitionDesktop.cs (main script)"
echo ""
echo "  GitHub Repository: https://github.com/wimjan123/coalition-desktop"