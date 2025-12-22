## Source code

Place the source code you're sharing for the session in this folder.

## Agent Framework Selection and Working modes

This solution supports multiple working modes implemented in `src/ZavaWorkingModes/WorkingMode.cs`. The modes include direct HTTP calls, LLM direct call, Microsoft Agent Framework (MAF) against Microsoft Foundry, AI Foundry variants, and a local MAF mode for local agent creation.

### Switching working modes

The working mode selection is managed through the **Settings page** in the Store frontend application:

1. Navigate to the **Settings** page in the Store app (accessible from the navigation menu)
2. Select your preferred working mode (the UI uses short names saved to `localStorage`)
3. Your preference is automatically saved and takes effect immediately without a server restart.

### Controllers

Each demo project contains controller implementations that route requests depending on the selected working mode. The Store frontend automatically routes requests to the appropriate controller paths according to the selected mode.

## How to use this code for the session

For step-by-step instructions on how to start the services, run demos, and deliver the session content, see the session delivery guide:

- `session-delivery-resources\readme.md` — contains run instructions, presenter notes, demo scripts, and required configuration.
