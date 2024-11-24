
# Kafka Features Setup and Testing Guide

This guide includes a comprehensive setup and example code for testing Kafka features such as producing, consuming, managing topics, and more using a Kafka-like setup (Kafkaesque).

---

## **1. Setup Kafka Features Testing**

### **Pre-requisites**
- A Kafka cluster (Docker or cloud-hosted Kafka setup is fine).
- A Kafka client library for your programming language:
  - [Confluent.Kafka](https://github.com/confluentinc/confluent-kafka-dotnet) for .NET.
  - [kafka-python](https://github.com/dpkp/kafka-python) for Python.
- Optional: [Kafdrop](https://github.com/obsidiandynamics/kafdrop) for monitoring topics and messages.

### **Topics to Cover**
1. Topic Management
2. Producing Messages
3. Consuming Messages
4. Error Handling
5. Offset Management
6. Message Filtering

---

## **2. Code Examples**

### **Topic Management**

Manage Kafka topics, including creating, listing, and deleting.

#### **Install Confluent.Kafka.Admin**
```bash
dotnet add package Confluent.Kafka.Admin
```

#### **Code: Listing and Creating Topics**
```csharp
using Confluent.Kafka.Admin;
using System;

var adminClientConfig = new AdminClientConfig
{
    BootstrapServers = "localhost:9092"
};

using var adminClient = new AdminClientBuilder(adminClientConfig).Build();

// Create a topic
var topicName = "test-topic";
try
{
    await adminClient.CreateTopicsAsync(new List<TopicSpecification>
    {
        new TopicSpecification { Name = topicName, NumPartitions = 3, ReplicationFactor = 1 }
    });
    Console.WriteLine($"Topic '{topicName}' created.");
}
catch (CreateTopicsException e)
{
    Console.WriteLine($"An error occurred creating the topic: {e.Results[0].Error.Reason}");
}

// List topics
var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
foreach (var topic in metadata.Topics)
{
    Console.WriteLine($"Topic: {topic.Topic}");
}
```

---

### **Producing Messages**

Send messages to a Kafka topic.

#### **Code: Producer**
```csharp
using Confluent.Kafka;

var producerConfig = new ProducerConfig
{
    BootstrapServers = "localhost:9092"
};

using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

try
{
    var deliveryResult = await producer.ProduceAsync("test-topic", new Message<Null, string>
    {
        Value = "Hello, Kafka!"
    });

    Console.WriteLine($"Message sent to {deliveryResult.TopicPartitionOffset}");
}
catch (ProduceException<Null, string> e)
{
    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
}
```

---

### **Consuming Messages**

Read messages from a Kafka topic.

#### **Code: Consumer**
```csharp
using Confluent.Kafka;

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "test-consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

consumer.Subscribe("test-topic");

try
{
    while (true)
    {
        var consumeResult = consumer.Consume(CancellationToken.None);
        Console.WriteLine($"Message: {consumeResult.Message.Value}, Partition: {consumeResult.Partition}");
    }
}
catch (OperationCanceledException)
{
    consumer.Close();
}
```

---

### **Error Handling**

Handle errors when producing or consuming.

#### **Producer Error Handling**
```csharp
catch (ProduceException<Null, string> e)
{
    if (e.Error.IsFatal)
    {
        Console.WriteLine("Fatal error occurred. Exiting...");
    }
    else
    {
        Console.WriteLine($"Non-fatal error: {e.Error.Reason}");
    }
}
```

#### **Consumer Error Handling**
```csharp
consumer.OnError += (_, e) =>
{
    Console.WriteLine($"Error: {e.Reason}");
};
```

---

### **Offset Management**

Control offsets for replaying or skipping messages.

#### **Code: Manual Offset Management**
```csharp
var consumerConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "test-consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
consumer.Subscribe("test-topic");

try
{
    while (true)
    {
        var consumeResult = consumer.Consume(CancellationToken.None);
        Console.WriteLine($"Message: {consumeResult.Message.Value}");

        // Manually commit the offset
        consumer.Commit(consumeResult);
    }
}
catch (OperationCanceledException)
{
    consumer.Close();
}
```

---

### **Message Filtering**

Filter messages based on content.

#### **Code: Filtering by Keyword**
```csharp
consumer.Subscribe("test-topic");

try
{
    while (true)
    {
        var consumeResult = consumer.Consume(CancellationToken.None);
        if (consumeResult.Message.Value.Contains("keyword"))
        {
            Console.WriteLine($"Filtered Message: {consumeResult.Message.Value}");
        }
    }
}
catch (OperationCanceledException)
{
    consumer.Close();
}
```

---

## **3. Testing Features on Kafkaesque**

### **Step 1: Verify Setup**
- Ensure Kafka is running locally or on Docker (`localhost:9092`).
- Use [Kafdrop](http://localhost:9000) to monitor topics and messages.

### **Step 2: Create Topics**
- Run the **Topic Management** code to create and list topics.

### **Step 3: Produce Messages**
- Run the **Producing Messages** code to send test messages to the topic.

### **Step 4: Consume Messages**
- Run the **Consuming Messages** code to verify the messages are consumed.

### **Step 5: Test Error Handling**
- Stop the Kafka service (`docker stop kafka`) and observe how the producer/consumer handles errors.

### **Step 6: Replay Messages**
- Modify the **Offset Management** code to replay old messages or skip messages.

### **Step 7: Filter Messages**
- Use the **Message Filtering** example to verify filtering logic works.

---

## **4. Explanation of Features**

1. **Topic Management**:
   - Dynamically create or delete topics.
   - Organizes message flow in a Kafka cluster.

2. **Producing Messages**:
   - Simulates writing data to Kafka.
   - Tests message serialization and partitioning.

3. **Consuming Messages**:
   - Reads messages in real-time.
   - Verifies message processing and application responsiveness.

4. **Error Handling**:
   - Ensures the application gracefully handles errors (e.g., Kafka broker downtime).

5. **Offset Management**:
   - Manages offsets for replaying or skipping messages.
   - Useful for debugging or recovering data.

6. **Message Filtering**:
   - Verifies logic for consuming specific messages based on conditions.

---

This setup ensures end-to-end testing of Kafka features in a Kafkaesque manner. Let me know if you need further assistance!
