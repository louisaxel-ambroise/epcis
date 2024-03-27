namespace FasTnT.Application.Services.Notifications;

public interface ICaptureListener
{
    event Action<int> OnCapture;
}
