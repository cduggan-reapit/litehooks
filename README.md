# litehooks

## The Vision

- `./src/api` is the API used to configure webhook consumers
- `./src/client` is a package used by products to publish webhooks
- `./src/common` is a library of common models and logic for client/processor/publish projects
- `./src/processor` is the lambda function responsible for processing webhook events
- `./src/publisher` is the lambda function responsible for sending processed events to consumers
