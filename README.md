# Microservices E-commerce Order Flow

This project demonstrates a microservices architecture for an e-commerce order system, leveraging **RabbitMQ** as a message broker for asynchronous communication. The solution consists of three distinct APIs: `OrderAPI`, `StockAPI`, and `PaymentAPI`, which communicate indirectly through a series of events. This design ensures that services are **loosely coupled**, allowing for greater scalability and resilience.

---

### Architecture and Message Flow

The diagram below illustrates the event-driven communication between the services during an order creation process.

```mermaid
%%{init: {"themeVariables": {"edgeLabelBackground":"#fff", "fontSize":"18px", "fontFamily":"Segoe UI, Arial, sans-serif", "curve":"cardinal"}}}%%
graph LR

%% Service nodes
OrderService["Order Service (OrderAPI)"]:::service
StockService["Stock Service (StockAPI)"]:::stock
PaymentService["Payment Service (PaymentAPI)"]:::payment

%% Database nodes
OrderDB(["Order Database"]):::db
StockDB(["Stock Database"]):::db
PaymentDB(["Payment Database"]):::db

%% Queue nodes
Queue1(["Stock Order Created Queue"]):::queue
Queue2(["Payment Stock Reserved Event Queue"]):::queue
Queue3(["Order Payment Completed Queue"]):::queue
Queue4(["Order Payment Failed Queue"]):::queue

%% Client node
Client["Client Request"]:::client

%% Flows (use thick lines for commands, dashed for events)
Client ==> |"Create Order"| OrderService

OrderService ==> |"Write to DB"| OrderDB
OrderService -. "Publish Order Created Event" .-> Queue1

Queue1 ==> |"Consume Event"| StockService
StockService ==> |"Check Stock"| StockDB
StockService -. "Publish Stock Reserved Event" .-> Queue2
StockService -. "Publish Stock Not Available Event" .-> Queue4

Queue2 ==> |"Consume Event"| PaymentService
PaymentService ==> |"Process Payment"| PaymentDB
PaymentService -. "Publish Payment Completed Event" .-> Queue3
PaymentService -. "Publish Payment Failed Event" .-> Queue4

Queue3 ==> |"Consume Event"| OrderService
OrderService ==> |"Mark Order as Completed"| OrderDB

Queue4 ==> |"Consume Event"| OrderService
OrderService ==> |"Mark Order as Failed"| OrderDB

%% Styling
classDef client fill:#2632A0,stroke:#1B2156,stroke-width:3px,color:#fff,font-weight:bold;
classDef service fill:#D1E9FF,stroke:#176CBE,stroke-width:3px,color:#042940,font-weight:bold;
classDef stock fill:#B5E7A0,stroke:#52B788,stroke-width:3px,color:#15573F,font-weight:bold;
classDef payment fill:#FFE28A,stroke:#B39200,stroke-width:3px,color:#665500,font-weight:bold;
classDef db fill:#F6F7F9,stroke:#444D5C,stroke-width:2.5px,color:#111;
classDef queue fill:#FF7A76,stroke:#B91616,stroke-width:2.5px,color:#fff,stroke-dasharray: 8 5;
classDef eventLink stroke-dasharray: 6 4,stroke-width:3px;
classDef thickLink stroke-width:4px,stroke:#0d47a1;
classDef label font-weight:bold,font-size:18px;

class OrderService service;
class StockService stock;
class PaymentService payment;
class OrderDB,StockDB,PaymentDB db;
class Queue1,Queue2,Queue3,Queue4 queue;
class Client client;

%% Edge styling for events (dashed)
linkStyle 2,5,6,8,9 stroke:#757575,stroke-dasharray: 7 5,stroke-width:3px;
%% link numbers are: 
%% 0: Client==>OrderService
%% 1: OrderService==>OrderDB
%% 2: OrderService-.->Queue1    [event]
%% 3: Queue1==>StockService
%% 4: StockService==>StockDB
%% 5: StockService-.->Queue2    [event]
%% 6: StockService-.->Queue4    [event]
%% 7: Queue2==>PaymentService
%% 8: PaymentService-.->Queue3  [event]
%% 9: PaymentService-.->Queue4  [event]
%% 10: PaymentService==>PaymentDB
%% 11: Queue3==>OrderService
%% 12: OrderService==>OrderDB
%% 13: Queue4==>OrderService
%% 14: OrderService==>OrderDB

%% Make main command/concrete action links even thicker
linkStyle 0,1,3,4,7,10,11,12,13,14 stroke:#1976d2,stroke-width:4px;

%% Subgraphs for group separation (visual purposes only, no inner direction override since edges cross subgraphs)
subgraph "E-Commerce Microservices Flow"
    direction LR
    subgraph "Services"
        OrderService
        StockService
        PaymentService
    end
    subgraph "Databases"
        OrderDB
        StockDB
        PaymentDB
    end
    subgraph "Queues"
        Queue1
        Queue2
        Queue3
        Queue4
    end
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
