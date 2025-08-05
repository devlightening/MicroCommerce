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
