我将为您创建一个完整的登录系统，包含必要的脚本和一个自动化设置工具，帮您自动生成场景和UI。

### 1. 创建脚本
*   **`Assets/Scripts/LoginManager.cs`**:
    *   处理用户输入（用户名/密码）。
    *   验证凭据（简单的演示验证）。
    *   登录成功后加载 "Battle scence" 场景。
    *   提供UI反馈（状态文本）。

### 2. 自动化设置工具 (编辑器脚本)
*   **`Assets/Editor/LoginSystemSetup.cs`**:
    *   在Unity菜单栏添加 `Tools > Setup Login System` 选项。
    *   **自动创建 "LoginScene"**:
        *   设置包含 Canvas 的现代化 UI 布局（背景、输入框、登录按钮、状态文本）。
        *   将所有 UI 组件自动连接到 `LoginManager`。
    *   **自动创建 "Battle scence"**:
        *   创建一个基础的新场景用于跳转。
    *   **更新 Build Settings**:
        *   自动将这两个场景添加到构建列表中，确保场景跳转功能直接可用。

### 3. 执行步骤
1.  创建 `Assets/Scripts` 和 `Assets/Editor` 文件夹。
2.  编写 `LoginManager.cs`。
3.  编写 `LoginSystemSetup.cs`。
4.  运行设置工具为您生成所有内容。
