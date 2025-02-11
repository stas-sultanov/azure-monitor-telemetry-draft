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
---

    classDiagram
    direction TB
    class Telemetry {
        <<Interface>>
        +OperationContext Operation #123; get; #125;
        +PropertyList Properties #123; get; #125;
        +TagList Tags #123; get; #125;
        +DateTime Time #123; get; #125;
    }

    class MeteredTelemetry {
        <<Interface>>
        +MeasurementList Measurements #123; get; #125;
    }

    class NamedTelemetry {
        <<Interface>>
        +String Nmae #123; get; #125;
    }

    class OperationTelemetry {
        <<Interface>>
        +TimeSpan Duration #123; get; #125;
        +String Id #123; get #125;
        +Boolean Success #123; get; #125;
    }

    class AvailabilityTelemetry {
        +TimeSpan Duration #123; get; init; #125;
        +String Id #123; get; #125;
        +MeasurementList Measurements #123; get; init; #125;
        +String Name #123; get; #125;
        +OperationContext Operation #123; get; init; #125;
        +PropertyList Properties #123; get; init; #125;
        +String RunLocation #123; get; init; #125;
        +String Message #123; get; #125;
        +Boolean Success #123; get; init; #125;
        +TagList Tags #123; get; init; #125;
        +DateTime Time #123; get; #125;
        +AvailabilityTelemetry(DateTime time, String id, String name, String message)
    }

    class DependencyTelemetry {
        +String Data #123; get; init; #125;
        +TimeSpan Duration #123; get; init; #125;
        +String Id #123; get; #125;
        +MeasurementList Measurements #123; get; init; #125;
        +String Name #123; get; init; #125;
        +OperationContext Operation #123; get; init; #125;
        +String ResultCode #123; get; init; #125;
        +PropertyList Properties #123; get; init; #125;
        +Boolean Success  #123; get; init; #125;
        +TagList Tags #123; get; init; #125;
        +String Target #123; get; init; #125;
        +DateTime Time #123; get; #125;
        +String Type #123; get; init; #125;
        +DependencyTelemetry(DateTime time, String id)
    }

    class EventTelemetry {
        +MeasurementList Measurements #123; get; init; #125;
        +String Name #123; get; #125;
        +OperationContext Operation #123; get; init; #125;
        +PropertyList Properties #123; get; init; #125;
        +TagList Tags #123; get; init; #125;
        +DateTime Time #123; get; #125;
        +EventTelemetry(DateTime time, String name)
    }

    class ExceptionTelemetry {
        +Exception Exception  #123; get; #125;
        +MeasurementList Measurements #123; get; init; #125;
        +OperationContext Operation #123; get; init; #125;
        +PropertyList Properties #123; get; init; #125;
        +Nullable~SeverityLevel~ SeverityLevel #123; get; init; #125;
        +TagList Tags #123; get; init; #125;
        +DateTime Time #123; get; #125;
        +ExceptionTelemetry(DateTime time, Exception exception)
    }

    class RequestTelemetry {
        +TimeSpan Duration #123; get; init; #125;
        +String Id #123; get; #125;
        +MeasurementList Measurements #123; get; init; #125;
        +String Name #123; get; init; #125;
        +OperationContext Operation #123; get; init; #125;
        +PropertyList Properties #123; get; init; #125;
        +String ResponseCode #123; get; init; #125;
        +Boolean Success #123; get; set; #125;
        +TagList Tags #123; get; init; #125;
        +DateTime Time #123; get; #125;
        +Uri Url #123; get; #125;
        +RequestTelemetry(DateTime time, String id, Uri url)
    }

    class TraceTelemetry {
        +String Message #123; get; #125;
        +OperationContext Operation #123; get; init; #125;
        +PropertyList Properties #123; get; init; #125;
        +SeverityLevel SeverityLevel #123; get; #125;
        +TagList Tags #123; get; init; #125;
        +DateTime Time #123; get; #125;
    }

    Telemetry <|-- MeteredTelemetry
    MeteredTelemetry <|-- NamedTelemetry
    NamedTelemetry <|-- OperationTelemetry

    AvailabilityTelemetry ..|> OperationTelemetry
    DependencyTelemetry ..|> OperationTelemetry
    EventTelemetry ..|> NamedTelemetry
    ExceptionTelemetry ..|> MeteredTelemetry
    RequestTelemetry ..|> OperationTelemetry
    TraceTelemetry ..|> Telemetry

```
