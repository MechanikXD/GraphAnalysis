
# Complex Networks Analysis Tool

A desktop application for modeling, analyzing, and visualizing complex networks with a focus on retail network structures.

## Table of Contents

1. [Introduction](#introduction)
2. [Features](#features)
3. [Installation](#installation)
4. [Usage](#usage)
5. [Advanced Usage](#advanced-usage)
6. [Changelog](#changelog)
7. [Acknowledgments](#Acknowledgments)

## Introduction

This application provides an intuitive interface for creating, editing, and analyzing complex networks. Built with Unity game engine, it offers interactive graph visualization and automatic computation of key network characteristics. The tool is designed for researchers, students, and professionals working with network analysis, particularly in the context of retail networks and distribution systems.

Key capabilities include:
- Interactive graph creation and editing
- Automatic calculation of network metrics
- Visual representation with customizable coloring based on characteristics
- Import/export functionality for data persistence
- Support for both directed and weighted graphs

## Features

### Network Modeling
- **Interactive Node Creation**: Create nodes by right-clicking on the canvas
- **Flexible Edge Management**: Add unidirectional or bidirectional edges between nodes
- **Drag-and-Drop Interface**: Move nodes freely to organize your network layout
- **Batch Node Addition**: Import multiple nodes at once using relative coordinates

### Network Analysis
The application automatically computes the following network characteristics:

**Global Metrics:**
- Network diameter
- Density
- Assortativity
- Entropy
- Laplacian Spectrum

**Node-Level Metrics:**
- Degree centrality
- Betweenness centrality
- Eigenvector centrality
- Clustering coefficient
- Local efficiency

### Visualization
- **Dynamic Coloring**: Nodes and edges change color based on selected characteristics
- **Real-time Updates**: Network metrics update automatically when structure changes
- **Customizable Background**: Change the workspace background for better visibility
- **Information Panels**: View global and local network characteristics in real-time

### Data Management
- **JSON Export/Import**: Save and load complete network structures
- **CSV Export**: Export adjacency matrices and node characteristics
- **Project Management**: Organize multiple networks in separate projects
- **Auto-save**: Projects are saved automatically to prevent data loss

### Additional Features
- **Automatic Link Generation**: Generate connections based on maximum distance and connectivity requirements
- **Multilingual Support**: Available in multiple languages
- **Performance Optimization**: Adaptive computation strategies based on network size

## Installation

1. Go to the [Releases](https://github.com/MechanikXD/GraphAnalysis/releases/tag/Release) page
2. Download the latest release for your operating system:
   - `GraphTool-Windows-v{current_version}.zip` for Windows
   - `GraphTool-Linux-v{current_version}.zip` for Linux
3. Extract the archive to your preferred location
4. Run the executable:
   - Windows: `GraphTool-Windows-v{current_version}.exe`
   - Linux: `./GraphTool-Linux-v{current_version}.x86_64`

## Usage

### Getting Started

**Main Menu:**
1. Launch the application
2. Create a new project or open an existing one
3. Configure application settings (optional)

**Working with Networks:**

1. **Creating Nodes**
   - Right-click on the canvas → Select "Create Node"
   - The node will appear at the cursor position

2. **Creating Edges**
   - Right-click on a node
   - Select "Connect (One-way)" or "Connect (Two-way)"
   - Click on the target node

3. **Editing Elements**
   - Left-click on a node to view/edit its name and characteristics
   - Left-click on an edge to view/edit its weight
   - Right-click on any element for context menu options

4. **Moving Nodes**
   - Right-click on a node → Select "Move"
   - Drag to the desired position
   - Click to place

5. **Deleting Elements**
   - Right-click on a node → Select "Delete"
   - Right-click on an edge → Select "Delete"

### Menu Options

Access the main menu via the button in the top-right corner:

- **Auto-generate Links**: Automatically create connections based on distance
- **Add Node Group**: Import multiple nodes from coordinates
- **Change Background**: Customize workspace appearance
- **Export Graph (JSON)**: Save complete network structure
- **Export Adjacency Matrix (CSV)**: Save matrix representation
- **Export Characteristics (CSV)**: Save computed metrics
- **Settings**: Adjust application preferences
- **Save & Exit**: Return to main menu

### Interface Elements

- **Bottom-left**: Current cursor coordinates in world space
- **Bottom-right**: Information feed with status messages
- **Top-left**: Global network characteristics panel

### Customization

**Coloring by Characteristic:**
- Open Settings
- Select which characteristic determines node/edge colors
- Changes apply immediately to the visualization

## Advanced Usage

**Prerequisites:**
- Unity 6000.0.60f1 LTS or later
- Git

**Clone the Repository:**
```bash
git clone https://github.com/MechanikXD/GraphAnalysis.git
cd GraphAnalysis
```

**Open in Unity:**
1. Launch Unity Hub
2. Click "Add" → "Add project from disk"
3. Navigate to the cloned repository folder
4. Select the folder and click "Open"
5. Unity will import the project (this may take several minutes)

**Required Packages:**
The project uses the following packages (automatically installed):
- UniTask
- Newtonsoft.JSON
- Runtime File Browser
- Localization Package
- Flexible Color Picker

**Building the Application:**
1. Open the project in Unity
2. Go to `File → Build Settings`
3. Select your target platform (Windows/Linux)
4. Click "Build" and choose output location
5. The executable will be created in the selected folder

A Jupyter Notebook for verifying network calculations is available in the repository:

**Location:** `/Git Files/Test Scripts/graph_tests.ipynb`

**Running in Google Colab:**
1. Open the notebook in GitHub
2. Click "Open in Colab" badge
3. Run all cells to verify calculations

**Running Locally:**
```bash
pip install networkx jupyter matplotlib
jupyter notebook /Git Files/Test Scripts/graph_tests.ipynb
```

The script compares results from the application with NetworkX library calculations.

## Changelog

### Version 0.1 (Initial Release)
**Release Date:** February 2026

## Acknowledgments

This project was developed as part of a Bachelor's thesis on complex network analysis with application to retail network structures.

**Technologies Used:**
- Unity 6000.0.60f1 LTS
- UniTask for asynchronous operations
- NetworkX for verification
- Google Colab for testing environment
