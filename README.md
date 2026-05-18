# ATOM - Discord & System Management Suite

ATOM is a comprehensive management and automation tool built in C# for the Windows platform. It provides a robust set of features for Discord account administration, webhook management, and system-level utilities through a high-performance console interface.

## Core Features

### Discord Account Administration
*   **Profile Management**: Synchronous update of biography, username (password-validated), and pronouns.
*   **Presence Control**: Granular control over custom status text and presence states (Online, Idle, DND, Invisible).
*   **Status Rotation**: Automated sequential status updates with customizable intervals.
*   **Security Analysis**: Detailed token verification including Nitro status, badge identification, and payment method checking.
*   **Social & Guild Cleanup**: 
    *   Bulk server exit or deletion (owner-specific).
    *   Relationship management (Bulk friend removal/blocking).
    *   Automated DM channel closure.
*   **Mass Communication**: Direct messaging capabilities across all open conversations.
*   **Account Reset (Nuke)**: Full-scale account cleaning protocol involving server, friend, and settings reset.

### Webhook Management
*   **Diagnostic Tools**: Real-time validity checking and metadata extraction.
*   **Automated Deployment**: Multi-threaded webhook creation and high-frequency messaging (Spam) modules.
*   **Cleanup**: Secure deletion of existing webhooks via Discord API.

### System Utilities
*   **Memory Injection**: Low-level DLL injector with Unicode and x64 support.
*   **Network Identification**: MAC address spoofing and HWID retrieval.
*   **Security Maintenance**: Detection and removal of common Discord client injections.
*   **System Optimization**: Automated temporary file and crash dump cleanup.

### Network & Data Tools
*   **Geolocation**: Detailed IP address lookup and analysis.
*   **Identity Simulation**: Generator for identity, card, and token data.
*   **Data Serialization**: QR code generation and token formatting/sorting utilities.

## Technical Specifications
*   **Framework**: .NET 10.0
*   **Language**: C# 13.0
*   **Architecture**: Service-oriented console application
*   **Security**: Cloudflare-compliant headers and rate-limit aware request handling.

## Installation and Setup

### Prerequisites
*   Windows 10/11
*   [.NET 10.0 Runtime or SDK](https://dotnet.microsoft.com/download)

### Build Instructions
```powershell
# Clone the repository
git clone https://github.com/Redwxll-atm/Tools.git

# Navigate to the project directory
cd Tools

# Build the project
dotnet build --configuration Release
```

## Legal Disclaimer
This software is provided for educational and security research purposes only. The author is not responsible for any misuse or violations of the Discord Terms of Service. Users are responsible for their own actions and should ensure compliance with local laws and platform regulations.

---
**Project maintained by Redwxll**
