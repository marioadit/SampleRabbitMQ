using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue:"task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

await channel.BasicQosAsync(prefetchCount: 1, prefetchSize: 0, global: false);

Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    int dots = message.Split('.').Length - 1;
    await Task.Delay(dots * 1000);


    Console.WriteLine(" [x] Done");

    // Acknowledge the message
    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queue: "task_queue", autoAck: false, consumer: consumer);
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();