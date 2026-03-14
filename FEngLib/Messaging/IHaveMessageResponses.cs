using System;
using System.Collections.Generic;

namespace FEngLib.Messaging;

public interface IHaveMessageResponses : ICloneable
{
    List<MessageResponse> MessageResponses { get; }
}