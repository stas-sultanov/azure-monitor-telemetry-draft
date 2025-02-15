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

    click Telemetry href "https://github.com/stas-sultanov/azure-monitor-telemetry/blob/dev/src/Code/Telemetry.cs" "Telemetry"
    click AvailabilityTelemetry href "../src/Code/AvailabilityTelemetry.cs" "AvailabilityTelemetry"
    click DependencyTelemetry href "../../blob/src/Code/AvailabilityTelemetry.cs" "DependencyTelemetry"

```
