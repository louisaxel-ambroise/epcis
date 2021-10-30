using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FluentValidation;

namespace FasTnT.Domain.Commands.Capture;

public class CaptureEpcisRequestCommandValidator : AbstractValidator<CaptureEpcisRequestCommand>
{
    const string AggregationEventMissingParentId = "Aggregation Event is missing ParentID EPC value";
    const string RequestMustContainEventOrMasterdata = "Request must contain Event or Masterdata";

    public CaptureEpcisRequestCommandValidator()
    {
        RuleFor(x => x.Request)
            .Must(HaveEventOrMasterdataOrBeACallback)
            .WithMessage(RequestMustContainEventOrMasterdata);

        RuleForEach(x => x.Request.Events)
            .Where(IsAddOrDeleteAggregation)
            .Must(HaveAParentIdEpc)
            .WithMessage(AggregationEventMissingParentId);
    }

    private bool HaveEventOrMasterdataOrBeACallback(Request request) 
        => request.Events.Count + request.Masterdata.Count > 0 
        || request.SubscriptionCallback != null;
    private bool IsAddOrDeleteAggregation(Event evt) => evt.Type == EventType.AggregationEvent && (evt.Action == EventAction.Add || evt.Action == EventAction.Delete);
    private bool HaveAParentIdEpc(Event evt) => evt.Epcs.Any(epc => epc.Type == EpcType.ParentId);
}
