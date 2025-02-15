# Azure Monitor Telemetry 

A tiny and very effective library to work via components aka Application Insights.

# Why Consider This Library
Standard Application Insights library has the following disadvantages. 


1.  Is considered as deprecated.
2.  Is not designed to work in certain scenarios like Power Platform plugins.
3.  Has problems with authentication.
4.  Slow.

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

    click Telemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Telemetry.cs" "Telemetry"
    click AvailabilityTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/AvailabilityTelemetry.cs" "AvailabilityTelemetry"
    click DependencyTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/DependencyTelemetry.cs" "DependencyTelemetry"
    click EventTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/EventTelemetry.cs" "EventTelemetry"
    click ExceptionTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/ExceptionTelemetry.cs" "ExceptionTelemetry"
    click MetricTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/MetricTelemetry.cs" "MetricTelemetry"
    click PageViewTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/PageViewTelemetry.cs" "PageViewTelemetry"
    click RequestTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/RequestTelemetry.cs" "RequestTelemetry"
    click TraceTelemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/main/src/Code/Types/TraceTelemetry.cs" "TraceTelemetry"

```
