# Microservices E-commerce Order Flow

This project demonstrates a microservices architecture for an e-commerce order system, leveraging **RabbitMQ** as a message broker for asynchronous communication. The solution consists of three distinct APIs: `OrderAPI`, `StockAPI`, and `PaymentAPI`, which communicate indirectly through a series of events. This design ensures that services are **loosely coupled**, allowing for greater scalability and resilience.

---

### Architecture and Message Flow

The diagram below illustrates the event-driven communication between the services during an order creation process.

```mermaid
graph TD
    %% Class Definitions
    classDef mainBox fill:#f9f9f9,stroke:#000,stroke-width:3px;
    classDef boldText font-weight:bold;

    %% Main Grouping
    subgraph "E-Commerce Microservices Flow"
        direction LR

        %% Services
        subgraph Services
            OrderService["Order Service (OrderAPI)"]:::boldText
            StockService["Stock Service (StockAPI)"]:::boldText
            PaymentService["Payment Service (PaymentAPI)"]:::boldText
        end

        %% Databases
        subgraph Databases
            OrderDB[(Order Database)]
            StockDB[(Stock Database)]
            PaymentDB[(Payment Database)]
        end

        %% RabbitMQ Queues
        subgraph "RabbitMQ Queues"
            Queue1["Stock_OrderCreatedQueue"]
            Queue2["Payment_StockReservedEventQueue"]
            Queue3["Order_PaymentCompletedQueue"]
            Queue4["Order_PaymentFailedQueue"]
        end

        %% Order Creation Flow
        Client["Client Request"] -- "Create Order" --> OrderService
        OrderService -- "Write to DB" --> OrderDB
        OrderService -- "Publish OrderCreatedEvent" --> Queue1

        %% Stock Reservation Flow
        Queue1 -- "Consume Event" --> StockService
        StockService -- "Check Stock" --> StockDB
        StockService -- "Publish StockReservedEvent" --> Queue2
        StockService -- "Publish StockNotAvailableEvent" --> Queue4

        %% Payment Processing Flow
        Queue2 -- "Consume Event" --> PaymentService
        PaymentService -- "Process Payment" --> PaymentDB
        PaymentService -- "Publish PaymentCompletedEvent" --> Queue3
        PaymentService -- "Publish PaymentFailedEvent" --> Queue4

        %% Order Status Update Flow
        Queue3 -- "Consume Event" --> OrderService
        OrderService -- "Mark Order as Completed" --> OrderDB

        Queue4 -- "Consume Event" --> OrderService
        OrderService -- "Mark Order as Failed" --> OrderDB

        %% Link Styles
        linkStyle default stroke-width:2px,fill:none,stroke:#555;

        %% Class Assignments
        class OrderService,StockService,PaymentService boldText;

        %% Box Styles
        style Services fill:#ffffff,stroke:#000,stroke-width:2px;
        style Databases fill:#ffffff,stroke:#000,stroke-width:2px;
        style RabbitMQ_Queues fill:#ffffff,stroke:#000,stroke-width:2px;

        %% Node Styles
        style Client fill:#B0E0E6,stroke:#333,stroke-width:2px;
        style OrderService fill:#87CEEB,stroke:#333,stroke-width:2px;
        style StockService fill:#98FB98,stroke:#333,stroke-width:2px;
        style PaymentService fill:#FFD700,stroke:#333,stroke-width:2px;
        style OrderDB fill:#D3D3D3,stroke:#333,stroke-width:2px;
        style StockDB fill:#D3D3D3,stroke:#333,stroke-width:2px;
        style PaymentDB fill:#D3D3D3,stroke:#333,stroke-width:2px;
        style Queue1 fill:#F08080,stroke:#333,stroke-width:2px;
        style Queue2 fill:#F08080,stroke:#333,stroke-width:2px;
        style Queue3 fill:#F08080,stroke:#333,stroke-width:2px;
        style Queue4 fill:#F08080,stroke:#333,stroke-width:2px;
    end
```




-----------------------------------------------------


E-Commerce Microservices: A Journey Through Asynchronous Events 🚀
Welcome to our modern microservices architecture, designed for a scalable and resilient e-commerce platform! This project showcases a powerful, event-driven communication model using RabbitMQ as our message broker. Say goodbye to tight coupling and hello to independent, efficient services working in perfect harmony. ✨

Here's a glimpse into our seamless order-to-completion workflow:

🛍️ 1. OrderAPI: The Customer's Gateway
The journey begins here! When a customer places an order, the OrderAPI springs into action:

It instantly records the order in the database, marking it as Suspend. ⏳

A single, powerful OrderCreatedEvent is published to RabbitMQ, signaling to the entire ecosystem that a new order is ready for processing. 📢

📦 2. StockAPI: The Warehouse Master
Our StockAPI is a vigilant listener, constantly monitoring for new order events:

It consumes the OrderCreatedEvent from its queue. 👂

Stock Check: It quickly verifies if all items are in stock.

Success Path: If everything is available, it reserves the items, updates the stock count, and publishes a StockReservedEvent. This message is the green light for the payment process! ✅

Failure Path (Optional): If a product is out of stock, it can publish a StockNotReservedEvent to gracefully handle the failure. ❌

💳 3. PaymentAPI: The Financial Wizard
With a StockReservedEvent in hand, the PaymentAPI takes the stage:

It simulates or executes the payment transaction. 💰

Success Path: A successful payment results in a PaymentCompletedEvent being published, broadcasting that the money has been securely processed. 🎉

Failure Path: If the payment fails for any reason, a PaymentFailedEvent is sent out, ensuring the order can be rolled back. 🚫

🎯 4. OrderAPI (Again!): The Finalizer
The OrderAPI makes a triumphant return to finalize the process:

It listens for both PaymentCompletedEvent and PaymentFailedEvent. 👂

Final Update: Upon receiving a completion event, it updates the order's status to Completed. 🥳

Rollback: If a failure event arrives, the status is changed to Failed, and any necessary compensation logic (like returning stock) is triggered. ↩️

This event-driven flow ensures each service has a single responsibility and communicates without direct dependencies. It's a modern, elegant, and robust solution for handling complex business logic with ease. 💡
