# Whisper UI

A Windows Forms (WinForms) application written in C# (.NET Framework 4.8) that provides a graphical user interface for running [OpenAI Whisper](https://github.com/openai/whisper) speech-to-text models.

## Features

- Simple UI for running Whisper commands without using the command line
- Supports custom arguments and output directory selection
- Displays process output and error logs
- Automatically organizes output files by date and input file name

## Prerequisites

- Windows 10 or later
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- [Whisper](https://github.com/openai/whisper) installed and accessible via command line (e.g., in a Python environment)
- Python and required dependencies for Whisper

## Installation
  
1. Clone this repository:    
       git clone https://github.com/yourusername/whisper-ui.git
2. Open the solution in Visual Studio 2022.
3. Build the solution.

## Usage

1. Launch the application (`CommandUI.exe`).
2. Select or enter the path to your audio file.
3. Configure Whisper arguments as needed (e.g., model, language, output directory).
4. Click the "Run" button to start transcription.
5. Output and error logs are saved to `log.txt` in the application directory.
6. The output directory will open automatically after processing.

## Configuration

- The application expects Whisper to be installed and accessible at the path specified in the configuration.
- Output files are organized by date and input file name for easy management.

## Troubleshooting

- Ensure that the Whisper executable path is correct and that all dependencies are installed.
- Check `log.txt` for error messages if transcription fails.
