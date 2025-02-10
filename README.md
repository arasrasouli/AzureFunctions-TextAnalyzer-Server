# Azure Functions Text Analyzer

This project implements a serverless architecture for processing and analyzing text using Azure Functions. It uses Azure Blob Storage for input/output, Service Bus for messaging, and a scalable set of function apps for efficient and event-driven processing.

---

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Prerequisites](#prerequisites)


---

## Overview

Azure Functions Text Analyzer processes text files uploaded to Azure Blob Storage, breaking them into chunks and processing them asynchronously via Azure Service Bus. This project demonstrates the power of Azure serverless services, combining scalability, high availability, and a distributed architecture.

---

## Features

- **Blob Storage Integration**: Watches for new blob uploads and triggers processing automatically.
- **Service Bus Messaging**: Ensures asynchronous and reliable processing of text chunks.
- **Text Chunking**: Splits large text files into manageable chunks.
- **Extensibility**: Supports additional pipelines for advanced text analysis.

---

## Prerequisites

Before running the project, ensure you have the following:

- Azure subscription
- .NET 8 SDK installed
- Azure CLI installed
- Access to:
  - Azure Blob Storage
  - Azure Service Bus
  - App Insights (optional for telemetry)
- A Service Bus namespace with a queue/topic configured.

## Configuration
Update the local.settings.json file for local testing. It should include the following keys:
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<BlobStorageConnectionString>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ServiceBusConnectionString": "<ServiceBusConnectionString>",
    "TextProcessingQueueName": "text-chunk-processing-queue",

  }
}
