# SimFusion Senior Unity Developer Assignment

## Overview

This project is a Unity prototype implementing the required anatomy viewer features for the SimFusion Senior Unity Developer assignment.

The focus of the implementation was to deliver a clean, maintainable architecture appropriate for a short prototype while keeping the code modular, readable, and easy to extend.

---

# How to Run

## Requirements
- Unity 6 (6000.3.14f1 LTS)
- Universal Render Pipeline (URP)

## Running the Project

1. Open the project using Unity Hub.
2. Open the main scene:
   Assets/Scenes/Main.unity
3. Press Play in the Unity Editor.

No additional setup is required.

---

# Features

## Task 1 – Anatomy Viewer

- Interactive anatomy model
- Modular project structure
- Data-driven body parts using ScriptableObjects

---

## Task 2 – Camera Controls

- Orbit camera
- Pan
- Zoom
- Reset View button
- Mouse and touch support

---

## Task 3 – Selection & Highlighting

- Raycast-based selection
- Single selected body part
- Highlight using MaterialPropertyBlock
- No runtime material instancing

---

## Task 4 – Information Panel

- Displays selected body part information
- Name
- Description
- Body System
- Optional icon
- Smooth fade animation

---

## Task 5 – Body System Visibility

- Toggle Skeleton, Muscles, and Organs independently
- Data-driven grouping using `BodySystemData`
- Automatically supports future body systems without code changes

---

## Task 6 – X-Ray Mode

- X-Ray toggle button
- All non-selected body parts become semi-transparent
- Selected body part remains fully visible and highlighted
- Original materials are restored when X-Ray is disabled
- Uses a shared transparent material to avoid runtime material instancing

---

## Task 7 – Section Tool

A simplified section tool was implemented using a moving section plane.

Instead of performing true runtime mesh slicing, the prototype moves a clipping plane through the anatomy model and hides body parts whose centers fall below the current plane position.

The section plane moves from the **bottom of the model toward the top**, producing the following progression:

```
Feet
↓
Legs
↓
Hips
↓
Abdomen
↓
Chest
↓
Neck
↓
Head
```

When the slider reaches its maximum value, all body parts are hidden.

If the currently selected body part becomes hidden by the section plane, the selection is cleared automatically so the highlight and information panel remain synchronized.

---

# Architecture

The project follows a simple event-driven architecture.

```
InputRaycaster
        │
        ▼
SelectionManager
        ├── HighlightService
        ├── InformationPanelUI
        └── XRayModeController

BodySystemManager

SectionPlaneController

OrbitCameraController
```

Each system has a single responsibility and communicates through existing events where appropriate.

---

## Script Organization

Scripts are organized by feature to keep responsibilities isolated:

- CameraControl – Camera movement and input providers
- Interaction – Selection and raycasting
- UI – Information panel and interface
- Systems – Body systems, X-Ray, Section Tool
- Data – ScriptableObjects and configuration assets
- Core – Shared interfaces and common utilities

Namespaces mirror the folder structure (SimFusion.Anatomy.<Feature>) to keep dependencies clear and maintainable.

---

# Design Decisions

## MaterialPropertyBlock

MaterialPropertyBlock is used for highlighting because it avoids creating runtime material instances.

---

## X-Ray

MaterialPropertyBlock cannot convert opaque URP materials into transparent ones.

For this reason, X-Ray mode swaps each renderer's `sharedMaterial` with a single transparent material asset while preserving the original material reference for restoration.

This approach:

- Avoids runtime material allocations
- Prevents material leaks
- Keeps the implementation simple
- Is appropriate for a prototype

## Section Tool

The assignment allows a simplified implementation of sectioning.

A moving section plane was chosen instead of true mesh slicing because:

- It satisfies the functional requirements.
- It is significantly simpler to implement.
- It is reliable and easy to maintain.
- It fits within the time constraints of the assignment.

A production implementation would likely use shader-based clipping or runtime mesh slicing depending on product requirements.

---

# Assumptions and Shortcuts

- The Section Tool uses a moving clipping plane rather than runtime mesh slicing to satisfy the assignment requirements with a simpler implementation.
- X-Ray mode swaps to a shared transparent material instead of using a custom transparent shader.
- The project focuses on clean architecture and maintainability over production-scale features.

---

# Verification

The project was verified to ensure:

- Camera controls work correctly
- Selection and highlighting function correctly
- Information panel updates correctly
- Body system toggles work independently
- X-Ray mode behaves correctly
- Section tool hides body parts in anatomical order
- Hidden selected parts automatically clear selection
- No runtime material leaks
- Zero compile errors
- Zero console warnings
- No runtime exceptions

---

# Future Improvements

For a production application, the following enhancements could be considered:

- Shader-based clipping for true cross-sectional views
- Runtime mesh slicing
- Animated transitions for section movement
- Lit transparent X-Ray shader
- Dynamic generation of body system UI
- Larger anatomy datasets with streaming support

---

## Implemented Optimizations

- Physics.RaycastNonAlloc with a preallocated RaycastHit buffer
- MaterialPropertyBlock for highlighting
- Shared transparent material for X-Ray mode
- FindObjectsByType(..., FindObjectsSortMode.None) during initialization

---

# Scaling for Larger Datasets

This prototype is designed for a small anatomy model and prioritizes simplicity over large-scale optimization. For a production application supporting hundreds or thousands of models, the following optimizations would be considered:

- **GPU Instancing and SRP Batching** to reduce draw calls for repeated meshes and compatible materials.
- **Level of Detail (LOD)** to render lower-detail meshes for distant objects.
- **Texture Atlases** to minimize material and texture switches.
- **Object Pooling** for frequently created or reused UI elements and runtime objects.
- **Spatial Culling** (frustum and occlusion culling) to avoid rendering objects outside the camera view.
- **Asynchronous Loading** using Addressables or Asset Bundles to stream models and textures on demand.
- **Data Caching** to avoid repeated lookups and unnecessary allocations.
- **Profiling and Optimization** using the Unity Profiler to identify bottlenecks before applying targeted optimizations.

These techniques were intentionally omitted from this prototype to keep the implementation focused on the assignment requirements while maintaining clean, extensible architecture.
