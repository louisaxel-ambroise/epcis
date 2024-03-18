using FasTnT.Domain.Exceptions;
using FasTnT.Host.Endpoints.Responses.Soap;
using FasTnT.Host.Features.v1_2.Extensions;
using System.Runtime.CompilerServices;

namespace FasTnT.Host.Extensions;

public class SoapActionBuilder
{
    private readonly Dictionary<string, Delegate> _mappedActions = [];

    public void Handle(Delegate requestDelegate, [CallerArgumentExpression(nameof(requestDelegate))] string action = null) => _mappedActions[action] = requestDelegate;
    public void On(string action, Delegate requestDelegate) => _mappedActions[action] = requestDelegate;

    internal Task<IResult> SoapAction(SoapEnvelope envelope, HttpContext context, CancellationToken cancellationToken)
    {
        return _mappedActions.TryGetValue(envelope.Action, out var handler)
            ? HandleSoapAction(handler, envelope, context, cancellationToken)
            : throw new Exception($"Unknown soap action: '{envelope.Action}'");
    }

    private static async Task<IResult> HandleSoapAction(Delegate handler, SoapEnvelope envelope, HttpContext context, CancellationToken cancellationToken)
    {
        try
        {
            var parameters = handler.Method.GetParameters();
            var paramList = new object[parameters.Length];

            for (int i = 0; i < paramList.Length; i++)
            {
                if (parameters[i].ParameterType == envelope.Query?.GetType())
                {
                    paramList[i] = envelope.Query;
                }
                else if (parameters[i].ParameterType == typeof(CancellationToken))
                {
                    paramList[i] = cancellationToken;
                }
                else if (envelope.CustomFields.TryGetValue(parameters[i].Name, out var parameter))
                {
                    paramList[i] = parameter;
                }
                else
                {
                    paramList[i] = context.RequestServices.GetService(parameters[i].ParameterType);
                }
            }

            var result = await handler.DynamicInvoke(paramList).CastTask();

            return SoapResults.FromResult(result);
        }
        catch (Exception ex)
        {
            return SoapResults.Fault(ex is EpcisException epcisException ? epcisException : EpcisException.Default);
        }
    }
}
