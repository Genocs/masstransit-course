# masstransit-course
A step by step example on using the awesome MassTransit Service Bus Library with .NET6  


[Serilog Datalust](https://blog.datalust.co/using-serilog-in-net-6/)


## Azure Service Bus integration Events



### Message created by MassTransit
{
    "messageId": "cc660000-c6ee-10e7-8fb5-08da834c772e",
    "requestId": null,
    "correlationId": null,
    "conversationId": "cc660000-c6ee-10e7-0a2e-08da834c7736",
    "initiatorId": null,
    "sourceAddress": "sb://masstransit-course.servicebus.windows.net/HPLNOCCO_GenocsMassTransitIntegrationsService_bus_3tuyyygg7aeqq698bdpegudsy3?autodelete=300",
    "destinationAddress": "sb://masstransit-course.servicebus.windows.net/settlement-manual-topic?type=topic",
    "responseAddress": null,
    "faultAddress": null,
    "messageType": [
        "urn:message:Genocs.MassTransit.Integrations.Contracts:SettlementSubmitted"
    ],
    "message": {
        "id": "c12def62-591e-4d7c-bc3b-59b8515d22d5",
        "code": "Tag_084309e6-ab28-47bb-9fb0-5889c47c7532",
        "accrualMonth": 0,
        "accrualYear": 0,
        "processedTimestamp": "2022-08-21T08:09:25.5737277Z"
    },
    "expirationTime": null,
    "sentTime": "2022-08-21T08:09:26.6639797Z",
    "headers": {
        "MT-Activity-Id": "00-18de3f1556085ddcba1c714df6714c6a-7d47860a08e82a31-01"
    },
    "host": {
        "machineName": "HPL-NOCCO",
        "processName": "Genocs.MassTransit.Integrations.Service",
        "processId": 157388,
        "assembly": "Genocs.MassTransit.Integrations.Service",
        "assemblyVersion": "1.0.0.0",
        "frameworkVersion": "6.0.8",
        "massTransitVersion": "8.0.6.0",
        "operatingSystemVersion": "Microsoft Windows NT 10.0.25182.0"
    }
}

### Message structure requested to be processed by the libray

urn:message:<messageNamespace>:<messageEventName>


{
    "messageType": [
        "urn:message:Genocs.MassTransit.Integrations.Contracts:SettlementSubmitted"
    ],
    "message": {
        "id": "c12def62-591e-4d7c-bc3b-59b8515d22d5",
        "code": "Tag_gio",
        "accrualMonth": 0,
        "accrualYear": 0,
        "processedTimestamp": "2022-08-21T08:09:25.5737277Z"
    }
}