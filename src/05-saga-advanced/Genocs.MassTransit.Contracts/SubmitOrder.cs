﻿using System;
using MassTransit;

namespace Genocs.MassTransit.Contracts
{

    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }

        MessageData<string> Notes { get; }
    }
}
