using HiLo.Client.Exceptions;
using HiLo.Client.Interfaces;
using HiLo.GlobalModels;
using System.Text;
using System.Text.Json;

namespace HiLo.Client.Deserializer
{
    public class MessageDeserializer : IMessageDeserializer
    {
        public IMessage? Deserialize(ArraySegment<byte> buffer)
        {
            // Remove null bytes
            buffer = buffer.Where(o => o != 0).ToArray();

            var json = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                if (document.RootElement.TryGetProperty("Type", out JsonElement propertyValue))
                {
                    return propertyValue.GetString() switch
                    {
                        "ResultOutputMessage" => JsonSerializer.Deserialize<ResultOutputMessage>(json),
                        "RequestInputMessage" => JsonSerializer.Deserialize<RequestInputMessage>(json),
                        "GameFinishedMessage" => JsonSerializer.Deserialize<GameFinishedMessage>(json),
                        "ErrorToDisplayMessage" => JsonSerializer.Deserialize<ErrorToDisplayMessage>(json),
                        "InformationToDisplayMessage" => JsonSerializer.Deserialize<InformationToDisplayMessage>(json),
                        _ => throw new UnknowTypeException(),
                    };
                }
                else
                {
                    throw new InvalidMessageException();
                }
            }
        }
    }
}
