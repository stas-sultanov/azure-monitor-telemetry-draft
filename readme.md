# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Class Diagram

```mermaid
---
title: Telemetry
config:
    class:
        hideEmptyMembersBox: true
---
classDiagram
    direction TB

    class Telemetry {
        <<Interface>>
    }

    class AvailabilityTelemetry {
    }

    class DependencyTelemetry {
    }

    class EventTelemetry {
    }

    class ExceptionTelemetry {
    }

    class PageViewTelemetry {
    }

    class RequestTelemetry {
    }

    class TraceTelemetry {
    }

    AvailabilityTelemetry ..|> Telemetry
    DependencyTelemetry ..|> Telemetry
    EventTelemetry ..|> Telemetry
    ExceptionTelemetry ..|> Telemetry
    PageViewTelemetry ..|> Telemetry
    RequestTelemetry ..|> Telemetry
    TraceTelemetry ..|> Telemetry

    click Telemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/dev/src/Code/Telemetry.cs" "Telemetry"
    click AvailabilityTelemetry href "../src/Code/AvailabilityTelemetry.cs" "AvailabilityTelemetry"
    click DependencyTelemetry href "../../blob/src/Code/AvailabilityTelemetry.cs" "DependencyTelemetry"

```
