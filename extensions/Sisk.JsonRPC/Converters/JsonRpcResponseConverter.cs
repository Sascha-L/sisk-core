﻿using LightJson;
using LightJson.Converters;

namespace Sisk.JsonRPC.Converters;

internal class JsonRpcResponseConverter : JsonConverter
{
    public override bool CanSerialize(Type type, JsonOptions options)
    {
        return type == typeof(JsonRpcResponse);
    }

    public override object Deserialize(JsonValue value, Type requestedType, JsonOptions options)
    {
        throw new NotSupportedException();
    }

    public override JsonValue Serialize(object value, JsonOptions options)
    {
        JsonRpcResponse response = (JsonRpcResponse)value;
        if (response.Error is JsonRpcError err)
        {
            return new JsonObject()
            {
                ["jsonrpc"] = "2.0",
                ["error"] = JsonValue.Serialize(err),
                ["id"] = response.Id
            };
        }
        else
        {
            return new JsonObject()
            {
                ["jsonrpc"] = "2.0",
                ["result"] = response.Result!.Value,
                ["id"] = response.Id
            };
        }
    }
}