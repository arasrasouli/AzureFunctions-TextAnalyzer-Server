# Azure Functions Text Analyzer

## Overview

This project implements a **serverless text processing pipeline** using **Azure Functions**. It processes large text files by **dividing them into indexed chunks** (without extracting text), analyzing word counts, and storing the results in **Azure Table Storage**. The workflow is event-driven, leveraging **Azure Blob Storage, Azure Queue Storage, and Azure Service Bus** for scalability and efficient processing.

## Workflow

1. **Blob Storage Trigger**:  
   - A function is triggered when a new text file is uploaded to **Azure Blob Storage**.  
   - The file is divided into **smaller chunks** based on **file length and a predefined chunk size**.  
   - Each chunk contains only **start and end byte positions**, not the actual text.  
   - These chunk metadata entries are then placed into **Azure Queue Storage** for processing.  

2. **Queue Storage Trigger (Chunk Processing)**:  
   - Each chunk metadata entry is read from the queue.  
   - The corresponding text portion is **retrieved dynamically** from Blob Storage based on the **start and end byte positions**.  
   - Word counts are calculated for each chunk.  
   - The results are stored in **Azure Table Storage**.  
   - Once all chunks of a file are processed, a **completion message** is published to another queue.  

3. **Queue Storage Trigger (Final Aggregation)**:  
   - A function listens for **file processing completion** messages.  
   - It **aggregates word counts** from all chunks of the file.  
   - The final word count summary is published to **Azure Service Bus** for further downstream processing or reporting.  

## Technologies Used

- **Azure Functions** – Serverless event-driven execution  
- **Azure Blob Storage** – Input/output storage for text files  
- **Azure Queue Storage** – Messaging system for chunk processing  
- **Azure Table Storage** – Storing word count results  
- **Azure Service Bus** – Event-based communication  

## Future Enhancements

- Add **real-time monitoring** with **Azure Application Insights**  
- Implement **text sentiment analysis** for additional insights  
- Optimize chunk processing with **parallel execution**  
- Add support for **different file formats (PDF, DOCX, etc.)**  

---

This architecture ensures efficient processing of large text files in a **scalable and cost-effective** manner using **serverless computing**.
