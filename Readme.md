# MoEPoC - Proof of Concept

## Overview
This project is a **Mixture of Experts Proof of Concept (MoEPoC)** that dynamically routes user prompts to the most appropriate **Local Large Language Model (LLM)** via LM Studio.

### Purpose
The goal of this project is to:
- **Route prompts intelligently** to specialized LLMs.
- **Utilize multiple models** instead of relying on a single AI model.
- **Provide a simple GUI interface** to interact with multiple models easily.

---
## Features
- **Automatic Task Classification** – Determines the type of prompt (e.g., chat, coding, math, storytelling).
- **Model Routing** – Uses predefined LLMs suited for different tasks.
- **Graphical User Interface (GUI) using Eto.Forms** – Simple and user-friendly design.
- **Configurable LM Studio Server IP** – Users can input their LM Studio API address.
- **Displays Model and Response Information** – Shows which model handled the request.

---
## Installation & Setup
1. **Install Dependencies**
   ```sh
   dotnet add package Eto.Forms
   ```
2. **Run LM Studio** and ensure the API server is enabled.
3. **Compile & Run the C# Project**
   ```sh
   dotnet run
   ```

---
## Pseudo Code
```plaintext
1. User inputs a prompt.
2. The system classifies the task type:
    - If coding → Use WizardCoder 7B
    - If math → Use DeepSeek Math 7B
    - If storytelling → Use MythoMax 7B
    - Otherwise → Use Mistral 7B
3. The system sends the prompt to the appropriate model.
4. The model processes the request and returns a response.
5. The response is displayed in the UI.
6. The user can clear the response or exit the application.
```

---
## Future Phases
- **Advanced Model Routing** – Implement NLP-based classifiers for better task recognition.
- **Parallel Model Execution** – Query multiple models simultaneously for efficiency.
- **Performance Optimizations** – Introduce caching and response-time improvements.
- **Model Selection Options** – Allow users to manually select which model to use for a task.

---
## Contribution
Contributions and modifications are welcome to improve the model selection and routing logic. Open for enhancements and optimizations.

Thank you for exploring MoEPoC!
