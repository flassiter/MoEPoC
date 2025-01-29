using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;

namespace MoEPoC
{
    class MoEPoC : Form
    {
        private HttpClient client = new HttpClient();
        private TextArea promptInput;
        private TextArea responseOutput;
        private TextBox serverIpInput;
        private Button sendButton;
        private Button clearButton;
        private Button exitButton;

        private static readonly Dictionary<string, string> modelMapping = new Dictionary<string, string>
    {
        { "chat", "mistral-7b" },
        { "code", "wizardcoder-7b" },
        { "math", "deepseek-math-7b" },
        { "story", "mythomax-7b" },
        { "summary", "phi-2" }
    };

        public MoEPoC()
        {
            Title = "MoE Proof of Concept using LM Studio";
            ClientSize = new Size(600, 500);

            // Server IP Input
            var serverIpLabel = new Label { Text = "LM Studio Server IP:", VerticalAlignment = VerticalAlignment.Center };
            serverIpInput = new TextBox { Text = "http://127.0.0.1:1234", Width = 200 };

            var serverLayout = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Items = { serverIpLabel, serverIpInput }
            };

            // Prompt Input
            promptInput = new TextArea { ToolTip = "Enter your prompt here...", Size = new Size(600, 100) };

            // Response Output
            responseOutput = new TextArea { ReadOnly = true, Size = new Size(600, 200) };

            // Buttons
            sendButton = new Button { Text = "Send" };
            clearButton = new Button { Text = "Clear" };
            exitButton = new Button { Text = "Exit" };

            sendButton.Click += async (sender, e) => await ProcessPrompt();
            clearButton.Click += (sender, e) => { promptInput.Text = ""; responseOutput.Text = ""; };
            exitButton.Click += (sender, e) => Application.Instance.Quit();

            var buttonLayout = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                Items = { sendButton, clearButton, exitButton }
            };

            // Main Layout
            Content = new StackLayout
            {
                Padding = 10,
                Spacing = 10,
                Items = { serverLayout, new Label { Text = "Prompt:" }, promptInput, new Label { Text = "Response:" }, responseOutput, buttonLayout }
            };
        }

        private async Task ProcessPrompt()
        {
            string prompt = promptInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(prompt))
            {
                MessageBox.Show("Please enter a prompt.", MessageBoxButtons.OK);
                return;
            }

            string taskType = ClassifyTask(prompt);
            string selectedModel = modelMapping.ContainsKey(taskType) ? modelMapping[taskType] : "mistral-7b";
            string serverUrl = serverIpInput.Text.Trim() + "/v1/completions";

            responseOutput.Text = $"[Routing to {selectedModel}]...\n";
            string response = await QueryModel(serverUrl, selectedModel, prompt);
            responseOutput.Text += response;
        }

        private string ClassifyTask(string prompt)
        {
            if (prompt.Contains("def ", StringComparison.OrdinalIgnoreCase) ||
                prompt.Contains("function ", StringComparison.OrdinalIgnoreCase) ||
                prompt.Contains("class ", StringComparison.OrdinalIgnoreCase))
            {
                return "code";
            }
            else if (prompt.Contains("solve", StringComparison.OrdinalIgnoreCase) ||
                     prompt.Contains("equation", StringComparison.OrdinalIgnoreCase) ||
                     prompt.Contains("math", StringComparison.OrdinalIgnoreCase))
            {
                return "math";
            }
            else if (prompt.Contains("once upon a time", StringComparison.OrdinalIgnoreCase) ||
                     prompt.Contains("story", StringComparison.OrdinalIgnoreCase))
            {
                return "story";
            }
            else if (prompt.Contains("summarize", StringComparison.OrdinalIgnoreCase) ||
                     prompt.Contains("summary", StringComparison.OrdinalIgnoreCase))
            {
                return "summary";
            }
            return "chat";
        }

        private async Task<string> QueryModel(string serverUrl, string model, string prompt)
        {
            var requestData = new
            {
                model = model,
                prompt = prompt,
                max_tokens = 256,
                temperature = 0.7
            };

            string json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(serverUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                string textResponse = doc.RootElement.GetProperty("choices")[0].GetProperty("text").GetString().Trim();

                return $"[Model: {model}]\n{textResponse}";
            }
            catch (Exception ex)
            {
                return $"[Error querying model: {ex.Message}]";
            }
        }

        
    }
}