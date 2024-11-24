using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class KafkaController : ControllerBase
{
    private readonly IProducer<Null, string> _producer;

    public KafkaController(IProducer<Null, string> producer)
    {
        _producer = producer;
    }

    [HttpPost("produce")]
    public async Task<IActionResult> Produce([FromBody] string message)
    {
        var result = await _producer.ProduceAsync("test-topic", new Message<Null, string> { Value = message });
        return Ok($"Message sent to {result.TopicPartitionOffset}");
    }
}