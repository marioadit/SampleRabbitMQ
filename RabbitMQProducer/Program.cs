using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue:"task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

var message = getMassage(args);
string getMassage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}
var body = System.Text.Encoding.UTF8.GetBytes(message);

var properties = new BasicProperties
{
    Persistent = true // Set the message to be persistent
};

await channel.BasicPublishAsync(
    exchange: string.Empty,
    routingKey: "task_queue",
    mandatory: true,
    basicProperties: properties,
    body: body
);
Console.WriteLine($" [x] Sent {message}");


// var count = 0;
// while (true)
// {
//     count++;
//     var message = $"hello World! Urutan pesan ke-: {count}";
//     var body = System.Text.Encoding.UTF8.GetBytes(message);
//     await channel.BasicPublishAsync(
//         exchange: string.Empty,
//         routingKey: $"hello",
//         body: body
//     );
//     Console.WriteLine($" [x] Sent {message}");
//     await Task.Delay(1000/2); // Delay for 1 second before sending the next message
// }