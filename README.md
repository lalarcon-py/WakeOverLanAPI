This repository contains a Wake-On-LAN (WoL) API built with ASP.NET Core. The WoL protocol allows you to remotely wake up computers on your network, even when they are in a low-power or hibernating state. This API provides a simple and convenient way to trigger WoL packets from within your applications or scripts.
The API exposes a single endpoint that accepts a MAC address and an IP address. It then constructs a "magic packet" based on the provided MAC address and sends it to the specified IP address using UDP broadcasting. This can be incredibly useful for remotely powering on computers or servers for maintenance, updates, or any other purpose, without the need for physical access.
Key features of this WoL API include:

Flexible IP Address Support: You can specify the IP address to send the WoL packet to, making the API compatible with various network configurations.
Robust MAC Address Handling: The API validates and sanitizes the provided MAC address to ensure proper formatting and prevent potential injection attacks.
Comprehensive Error Handling: Appropriate HTTP status codes and error messages are returned in case of failures, providing clear feedback to the client.
Asynchronous Operations: Asynchronous programming is employed throughout the codebase, ensuring efficient handling of I/O operations and improved scalability.

To use this WoL API, simply make a POST request to the /WoL endpoint, providing the MAC address in the request body and the IP address as a query parameter. The API will respond with a success or failure message based on the outcome of the operation.
