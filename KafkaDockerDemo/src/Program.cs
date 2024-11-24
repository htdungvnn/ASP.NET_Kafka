using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Kafka Producer Configuration
builder.Services.AddSingleton<IProducer<Null, string>>(provider =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092"
    };
    return new ProducerBuilder<Null, string>(config).Build();
});

// Kafka Consumer Configuration
builder.Services.AddSingleton<IConsumer<Ignore, string>>(provider =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9092",
        GroupId = "aspnet-consumer-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<Ignore, string>(config).Build();
});

var app = builder.Build();

app.MapControllers();

app.Run();