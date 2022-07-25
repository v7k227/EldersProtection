# Elders Protection

Elders Protection is a windows application for "Fraud Prevention". It has three characteristics of immediacy, effectiveness and interactivity.
This service has three functions of "warning", "monitoring" and "reporting" according to the stage of fraudulent behavior.

- Warning
-> Remind the user that the call is a fraudulent call, so that the elderly can be more vigilant and reduce the possibility of being deceived.
- Monitoring
-> Detect the content of the conversation to determine whether it is a scam call.
- Reporting
-> Notify family members or other trust units and take the initiative to care for the elderly.

## How it works ##
- Fraud Detection
-> Use Azure Streaming Analysis to determine whether it is a fraudulent call from a pre-trained fraudulent call database.
- Identify fraudulent
-> Use AZURE LUIS to perform semantic analysis and combine the contextual sentences of fraudulent keywords to accurately determine whether it is a fraudulent call.
- Assistent
-> Notifying seniors of their registered trust lists by means of telephone messages, etc.
The system is connected with the BOT Service to interact with the elderly in real time and provide advice.

## Project Environment ##
- Visual Studio 2019 with dotnet framework 4.5