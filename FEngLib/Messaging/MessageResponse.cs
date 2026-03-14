using System;
using System.Collections.Generic;

namespace FEngLib.Messaging;

public class MessageResponse : ICloneable
{
    public MessageResponse()
    {
        Responses = new List<ResponseCommand>();
    }

    public MessageResponse(uint id, List<ResponseCommand> responses)
    {
        Id = id;
        Responses = responses;
    }

    public uint Id { get; set; }
    public List<ResponseCommand> Responses { get; set; }


    public object Clone()
    {
        var result = new MessageResponse();

        result.Id = this.Id;

        foreach (var response in this.Responses)
        {
            result.Responses.Add(response?.Clone() as ResponseCommand);
        }

        return result;
    }
}