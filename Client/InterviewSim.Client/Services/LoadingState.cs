namespace InterviewSim.Client.Services;

public class LoadingState
{
    public event Action? OnChange;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnChange?.Invoke();
        }
    }
}
