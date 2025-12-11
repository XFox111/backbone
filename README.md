# Backbone

[![GitHub last commit](https://img.shields.io/github/last-commit/xfox111/backbone?label=Last+update)](https://github.com/XFox111/backbone/commits/main)
[![Docker Image Size](https://img.shields.io/docker/image-size/xfox111/backbone?logo=docker&logoColor=white)](https://hub.docker.com/r/xfox111/backbone/)

Small ASP.NET web server for one way communication between two clients.

This server is one of the key components of [EasyLogon project](https://github.com/xfox111/easylogon-web).

## Overview

```mermaid
sequenceDiagram
	participant S as Sender
	participant B as Backbone
	participant R as Receiver

	opt SignalR
		R->>+B: Connection request
		B->>-R: connection_id
	end
	opt Arbitrary data channel
		R-->>S: connection_id
	end
	opt HTTP POST
		S->>+B: connection_id + data
	end
	opt SignalR
		B->>-R: data
	end
```

### Endpoints
- **SignalR**: `/ws` - WebSocket endpoint for real-time communication.
- **POST**: `/send?id={connectionId}` - HTTP POST endpoint for sending data to the receiver.

Body of the `/send` endpoint must be of type `Content-Type: application/json`.

### Key points

- The arbitrary channel for `connectionId` tranmission should be as secure as possibe (preferably an offline channel), since posession of `connectionId` can pose a security threat.
- Connection between Backbone and receiver preferably should be re-established after every transmission to avoid replay attacks.

## Related papers

- [QR Code Authentication System as an Ultimate Tool for Personal Cybersecurity (2023, IEEE)](https://ieeexplore.ieee.org/abstract/document/10397212)

## Development

### Prerequisites

For development you can use [Dev Containers](https://devcontainers.io/) or [GitHub Codespaces](https://github.com/features/codespaces). Otherwise you will need to install following tools:
- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/)


### Building and debugging

Here're some commonly used commands:
```bash
dotnet restore			# Install dependencies
dotnet run				# Start the development server
dotnet build			# Build the project for production
```

To build a Docker image, run:

```bash
docker build -t backbone:latest .

# To run the Docker container:
docker run -d -p 8080:8080 --name backbone_server backbone:latest
```

> [!TIP]
> If you use VS Code, you can also use pre-defined tasks for building and debugging.

## Contributing
[![GitHub issues](https://img.shields.io/github/issues/xfox111/backbone)](https://github.com/xfox111/backbone/issues)
[![CI](https://github.com/XFox111/backbone/actions/workflows/ci.yml/badge.svg)](https://github.com/XFox111/backbone/actions/workflows/ci.yaml)
[![GitHub repo size](https://img.shields.io/github/repo-size/xfox111/backbone?label=repo%20size)](https://github.com/xfox111/backbone)

There are many ways in which you can participate in the project, for example:
- [Submit bugs and feature requests](https://github.com/xfox111/backbone/issues), and help us verify as they are checked in
- Review [source code changes](https://github.com/xfox111/backbone/pulls)
- Review documentation and make pull requests for anything from typos to new content

If you are interested in fixing issues and contributing directly to the code base, please refer to the [Contribution Guidelines](/CONTRIBUTING.md)

---

[![Bluesky](https://img.shields.io/badge/%40xfox111.net-BSky?logo=bluesky&logoColor=%230285FF&label=Bluesky&labelColor=white&color=%230285FF)](https://bsky.app/profile/xfox111.net)
[![GitHub](https://img.shields.io/badge/%40xfox111-GitHub?logo=github&logoColor=%23181717&label=GitHub&labelColor=white&color=%23181717)](https://github.com/xfox111)
[![Buy Me a Coffee](https://img.shields.io/badge/%40xfox111-BMC?logo=buymeacoffee&logoColor=black&label=Buy%20me%20a%20coffee&labelColor=white&color=%23FFDD00)](https://buymeacoffee.com/xfox111)

> ©2025 Eugene Fox. Licensed under [MIT license](/LICENSE)
