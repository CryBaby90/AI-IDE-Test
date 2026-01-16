# AGENTS.md for Unity Card Game Project

This file contains guidelines for build/lint/test commands and code style conventions for this Unity-based card game project.

## Build Commands

### Unity Editor Build
- Open Unity Editor
- Go to File > Build Settings
- Select build target (Windows, macOS, Linux, WebGL, etc.)
- Click "Build" or "Build and Run"

### Command Line Build
For Windows standalone:
```
Unity.exe -batchmode -nographics -projectPath "D:\Project\AI-IDE-Test" -buildWindowsPlayer "build/CardGame.exe" -quit
```

For WebGL:
```
Unity.exe -batchmode -nographics -projectPath "D:\Project\AI-IDE-Test" -buildTarget WebGL -buildWebPlayer "build/WebGL" -quit
```

For Android (requires Android SDK):
```
Unity.exe -batchmode -nographics -projectPath "D:\Project\AI-IDE-Test" -buildTarget Android -buildAppBundle "build/CardGame.aab" -quit
```

### Automated Build Script
Create a `build.bat` file:
```batch
@echo off
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2021.3.16f1\Editor\Unity.exe"
%UNITY_PATH% -batchmode -nographics -projectPath "%~dp0" -buildWindowsPlayer "build/CardGame.exe" -quit
if %errorlevel% neq 0 exit /b %errorlevel%
echo Build completed successfully
```

## Test Commands

### Unity Test Runner (Editor)
- Open Unity Editor
- Go to Window > General > Test Runner
- Select "PlayMode" or "EditMode" tab
- Click "Run All" to run all tests
- Click "Run Selected" to run a single test class or method

### Running a Single Test
- In Test Runner, expand test hierarchy
- Right-click on individual test method
- Select "Run"

### Command Line Testing
Run all tests:
```
Unity.exe -batchmode -nographics -projectPath "D:\Project\AI-IDE-Test" -runTests -testResults "test-results.xml" -testPlatform EditMode -quit
```

Run tests with specific filter:
```
Unity.exe -batchmode -nographics -projectPath "D:\Project\AI-IDE-Test" -runTests -testResults "test-results.xml" -testPlatform PlayMode -testFilter "CardGameManagerTests" -quit
```

### Test Coverage
Using Unity Test Tools Code Coverage:
- Install package: com.unity.testtools.codecoverage
- In Test Runner, check "Enable Code Coverage"
- Run tests to generate coverage report

## Lint Commands

Unity doesn't have built-in linting, but you can use external tools:

### StyleCop
Install:
```
dotnet tool install -g stylecop.cli
```

Run on Scripts directory:
```
stylecop "Assets/Scripts/**/*.cs" --settings "Assets/Scripts/stylecop.json"
```

### Roslyn Analyzers
Add to project via NuGet:
- Microsoft.CodeAnalysis.FxCopAnalyzers
- Run analysis in Visual Studio or Rider

### EditorConfig
Ensure .editorconfig exists with formatting rules:
```
root = true

[*.cs]
charset = utf-8
indent_style = space
indent_size = 4
end_of_line = crlf
trim_trailing_whitespace = true
insert_final_newline = true
```

## Code Style Guidelines

### Language and Framework
- Primary language: C# 9.0+
- Framework: Unity 2021.3+
- Target .NET Standard 2.1 (Unity default)

### File Organization
- Scripts in `Assets/Scripts/`
- Subdirectories by feature: `CardGame/`, `UI/`, etc.
- One class per file (except nested classes)
- File name matches main class name

### Imports (Using Statements)
```csharp
// System imports first
using System.Collections;
using System.Collections.Generic;

// Unity imports second
using UnityEngine;
using UnityEngine.UI;

// Third-party imports (if any)

// Custom imports last, grouped by namespace
using CardGame.Events;
using CardGame.Effects;
```

- No unused imports
- Sort alphabetically within groups
- Use file-scoped namespaces when possible (C# 10+)

### Naming Conventions

#### Classes and Structs
- PascalCase: `CardGameManager`, `PlayerStats`
- Interfaces: `ICardEffect` (prefix with I)
- Enums: PascalCase for type and values: `CardType { Minion, Spell }`

#### Methods and Properties
- PascalCase: `PlaceCard()`, `CurrentHealth`
- Boolean properties: `IsBoardFull`, `HasEndedTurn`
- Private methods: PascalCase (same as public)

#### Fields and Variables
- Private fields: camelCase with underscore: `_currentHealth`
- Public fields: PascalCase (rare, prefer properties)
- Local variables: camelCase: `cardInstance`
- Parameters: camelCase: `cardData`

#### Constants
- PascalCase with underscore: `MAX_BOARD_SIZE`
- Or all caps: `MAX_BOARD_SIZE`

### Formatting
- Indentation: 4 spaces (no tabs)
- Line length: 120 characters max
- Blank lines:
  - Between using groups
  - Before/after class members
  - Before/after method bodies
- Braces: Same line for methods, new line for classes
```csharp
public class Player
{
    public void TakeDamage(int damage)
    {
        // code
    }
}
```

### Types and Type Safety
- Use explicit types over `var`
- Prefer `IReadOnlyList<T>` for read-only collections
- Use nullable reference types where appropriate
- Avoid `object` type; use generics instead

### Error Handling
- Use exceptions for exceptional cases
- Validate parameters with Debug.Assert or ArgumentException
- Use Debug.LogWarning/Error for development warnings
- Return bool/null for expected failures
```csharp
public bool PlaceCard(CardInstance card)
{
    if (card == null)
    {
        Debug.LogWarning("Cannot place null card");
        return false;
    }
    // success
    return true;
}
```

### Comments and Documentation
- XML documentation for public APIs:
```csharp
/// <summary>
/// Places a card on the board at the specified position.
/// </summary>
/// <param name="card">The card to place</param>
/// <param name="position">Position on board (-1 for auto)</param>
/// <returns>True if placement succeeded</returns>
public bool PlaceCard(CardInstance card, int position = -1)
```
- Internal comments: `//` for single lines, `/* */` for blocks
- TODO comments: `// TODO: Implement mana cost calculation`
- Avoid obvious comments

### Unity-Specific Patterns
- Use `[Header("Section Name")]` to group Inspector fields
- Use `[Tooltip("Description")]` for field help
- Use `[SerializeField]` for private serialized fields
- Prefer ScriptableObjects for data that needs editing
- Use coroutines for async operations: `IEnumerator`
- Event system: Use UnityEvents or custom delegates
- Singleton pattern:
```csharp
private static GameManager _instance;
public static GameManager Instance => _instance ??= FindObjectOfType<GameManager>();
```

### Performance Considerations
- Use object pooling for frequently created/destroyed objects
- Cache GetComponent calls
- Use FixedUpdate for physics, Update for frame-dependent logic
- Avoid FindObjectOfType in Update; cache references
- Use LayerMasks for raycasts
- Minimize allocations in hot paths

### Code Review Checklist
- [ ] No unused imports or variables
- [ ] Consistent naming and formatting
- [ ] Null checks for reference types
- [ ] Proper error handling
- [ ] Comments for complex logic
- [ ] No hardcoded strings (use constants or localization)
- [ ] Unit tests for new functionality
- [ ] Performance considerations addressed

### Version Control
- Commit frequently with descriptive messages
- Use feature branches for new work
- Rebase instead of merge when possible
- Include relevant assets in commits

### IDE Setup
- Use Visual Studio or Rider with Unity plugin
- Enable code analysis warnings
- Configure formatting on save
- Use Unity's built-in debugger

This document should be updated as the project evolves and new conventions emerge.