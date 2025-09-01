# Advanced Movement & Interactive Liquid Prototype (2025)

A Unity-based technical prototype in active development for **Atlas Games**. Currently featuring a live 2D minimap system. A dynamic liquid interaction system is an upcoming core feature.

## Current Feature: Live 2D Minimap

**Status: Implemented**

### Simple Live 2D Minimap
A real-time top-down view of the player's surroundings, providing essential spatial awareness.
- Built efficiently using Unity's **RenderTexture**.
- Renders a separate camera view to a UI element without compromising main gameplay visuals.
- Fully functional and ready for integration with other systems.

### Advanced Third-Person Controller
*Movement suite:*
- **Locomotion:** Walk, Sprint, and Air Control 
- **Verticality:** Jump and Wall Climbing  
- **Ground Slides:** Crouch and Momentum-based Sliding 

### Dynamic Camera System
A real-time camera that tracks and follows the player's position

## In Development

### Dynamic Liquid Interaction System
*Planned visual reactivity:*
- Liquid surfaces will displace and ripple based on player movement.
- Interactions triggered by walking, jumping, and landing.

## Technical Implementation

- **Engine:** Unity 6.1
- **Scripting:** C#
- **Current Tech:**
  - Camera RenderTextures for minimap functionality.
- **Future Tech:**
  - Custom character controller logic.
  - Liquid shader (likely Shader Graph) with vertex displacement.
