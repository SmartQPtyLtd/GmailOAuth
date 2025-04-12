# GmailOAuth# GmailOAuth

GmailOAuth is a C# library designed to simplify the process of sending emails using Gmail's SMTP server with OAuth2 authentication. It supports both service account and user account authentication methods.

## Features

- Authenticate using a Google Service Account.
- Authenticate using user credentials via OAuth2.
- Send emails securely using Gmail's SMTP server.
- Built on `.NET Standard 2.0`, compatible with a wide range of .NET platforms.

## Requirements

- .NET Standard 2.0 compatible runtime.
- A Google Cloud project with Gmail API enabled.
- For service account authentication:
  - A service account JSON key file.
- For user account authentication:
  - OAuth2 client credentials (Client ID and Client Secret).

## Installation

1. Clone the repository:
2. Add the project to your solution or include the compiled DLL in your project.

3. Install the required NuGet packages:
## Usage

### Service Account Authentication
### User Account Authentication
## Configuration

- **Service Account JSON Key**: Obtain this from the Google Cloud Console under "Service Accounts".
- **OAuth2 Client Credentials**: Obtain this from the Google Cloud Console under "APIs & Services > Credentials".

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

# Gmail Daemon

## Overview
The Gmail Daemon is a .NET application designed to automate the processing and sending of `.eml` email files using Gmail's SMTP service. It leverages service account authentication for secure and efficient email handling.

## Features
- Processes `.eml` files from a specified directory.
- Sends emails using Gmail's SMTP service with domain-wide delegation.
- Handles large files by moving them to a designated folder.
- Configurable settings via `AppSettings.json`.

## Requirements
- .NET 9 (for the `GmailDaemon` project).
- .NET Standard 2.0 (for the `GmailOAuth` library).
- Gmail service account credentials with domain-wide delegation enabled.

## Setup
1. Clone the repository to your local machine.
2. Ensure you have the required .NET SDK installed.
3. Configure the `AppSettings.json` file:
   

2. Run the application:
   
3. Monitor the console output for email processing status.

## Dependencies
- [MailKit](https://github.com/jstedfast/MailKit)
- [MimeKit](https://github.com/jstedfast/MimeKit)

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

# TestApp - Gmail OAuth Email Sender

TestApp is a C# application that sends emails using Gmail's OAuth authentication. It leverages the `MimeKit` library for email creation and `GmailOAuth` for handling Gmail's OAuth-based SMTP authentication.

## Features
- Reads configuration from JSON files.
- Supports both plain text and HTML email formats.
- Uses Gmail's OAuth for secure authentication.
- Automatically opens the Google login page for authorization.

## Prerequisites
- .NET 9 SDK installed.
- A Gmail account with API access enabled.
- A Google Cloud project with OAuth credentials configured.
- The following NuGet packages installed:
  - `MimeKit`
  - `GmailOAuth`

## Configuration

### 1. AppSettings.json
Create an `AppSettings.json` file in the root directory with the following structure:


### 2. Google Project Auth JSON
Download the service account credentials JSON file from your Google Cloud project and specify its path in the `ProjectAuthJsonFile` field of `AppSettings.json`.

## How to Run

1. Clone the repository.

2. Install dependencies:
   Ensure the required NuGet packages are installed in your project.

3. Build and run the application.

4. Follow the on-screen instructions to authorize the app via the Google login page.

## Code Overview

The application performs the following steps:
1. Reads configuration from `AppSettings.json`.
2. Validates the configuration and loads the Google service account credentials.
3. Creates an email message using `MimeKit`.
4. Authenticates with Gmail using OAuth and sends the email.

## Error Handling
- If the configuration files are missing or invalid, the application will exit with an error message.
- If the email fails to send, the application will log the error to the console.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments
- [MimeKit](https://github.com/jstedfast/MimeKit) for email creation.
- [GmailOAuth](https://github.com/your-gmail-oauth-library) for Gmail OAuth integration.
