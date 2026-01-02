# Low-Latency LLM Consumption Systems

<p align="left">
  <img src="https://github.com/ringorjo/VirtualRealityProjects/blob/main/gif/AI.gif" width="500px">
</p>


 [**Watch Demo Video**](https://youtu.be/HNy9f1PpM2I)

> Realtime AI integration for Unity using WebSockets, streaming audio, and scalable multi-agent architectures.

---

## Overview

This project documents the architecture and implementation of **low-latency LLM consumption systems** for real-time applications (primarily **Unity**). It focuses on conversational AI with **streaming audio**, **minimal end-to-end latency**, and **provider-agnostic design**.

Supported providers include:

* **OpenAI Realtime API**
* **ElevenLabs Conversational AI**

---

## Key Features

* 🔌 Persistent **WebSocket** communication
* 🎙️ **Bidirectional audio streaming** (PCM / float samples)
* ⚡ **Ultra-low latency** conversational responses
* 🤖 **Multi-agent support** with independent context and state
* 🧠 **State-driven conversation flow** (Idle / Listening / Processing / Talking)
* 🎮 Seamless **Unity integration** (single-player & multiplayer)
* 🔄 Provider-agnostic via Strategy Pattern

---

## Tech Stack

* **Engine**: Unity
* **Languages**: C#
* **AI Providers**:

  * OpenAI Realtime API
  * ElevenLabs Conversational AI
* **Networking**: WebSockets
* **Multiplayer Audio**: Photon Voice (custom Recorder)
* **Backend / Orchestration**: REST APIs (Azure)

---

## Architecture

### High-Level Flow

```
[Unity Client]
     |
     | WebSocket (bidirectional)
     v
[Realtime AI Connector]
     |
     | Commands / Events
     v
[AI Orchestrator / Agent Manager]
     |
     | Audio / Text Streams
     v
[Audio Playback + Gameplay Systems]
```

### Design Principles

* Decoupled systems
* Event-driven communication
* Explicit state management
* Streaming-first (audio is never treated as a full clip)

---

## Conversational State Machine

The conversation lifecycle is controlled using the **State Pattern**:

* **Idle** – Awaiting user interaction
* **Listening** – Capturing user input
* **Processing** – LLM request in progress
* **Talking** – Streaming AI-generated audio

Benefits:

* Clean interruption handling
* Animation and lip-sync synchronization
* Predictable conversational flow

---

## Audio Streaming Pipeline (Unity)

1. Receive PCM / float audio chunks
2. Convert to Unity-compatible format
3. Write to circular buffer
4. Immediate playback

### Multiplayer (Photon Voice)

* Custom `Recorder` implementation
* Injects AI-generated samples directly
* Audio replicated to all connected clients
* Minimal perceived latency

---

## Design Patterns Used

* **Command Pattern** – Encapsulates LLM requests
* **Observer / Event Pattern** – Async response handling
* **Repository Pattern** – Per-agent context persistence (threads, runs)
* **Strategy Pattern** – Dynamic AI provider swapping
* **State Pattern** – Conversation lifecycle management

---

## Agent Orchestration

* Multiple agents supported simultaneously
* Each agent owns:

  * Identity
  * Conversational context
  * Independent state machine
* External orchestration via REST (Azure-based services)

---

## Challenges Solved

* Reducing perceptible latency in VR
* Audio–animation synchronization
* Cancelling in-progress AI responses
* Debugging async WebSocket traffic
* Scaling to multiple concurrent agents

---

## Best Practices

* Never couple gameplay logic to AI SDKs
* Treat audio as a continuous stream
* Log all WebSocket traffic
* Design for interruptions from day one
* Keep AI providers behind abstractions

---

## Future Improvements

* Multimodal input (vision)
* Intent prediction
* Response caching
* Agent load balancing

## Plugins Required
* Sirenix Odin Inspector

## AIConfig Required
For both ElevenLabs and OpenAI integrations, it is necessary to provide URLs , Agent ID and, when required, an authentication token to properly initialize and authorize the connection.
  
---

## Author

**Jonathan Rincón**

Real-time AI / Unity / XR Developer

---

> This repository serves as a technical reference for designing and implementing low-latency, real-time LLM systems in Unity.
