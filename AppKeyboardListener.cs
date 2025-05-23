namespace WindowsKeyboardCapture;
public class AppKeyboardListener : IDisposable
{
    public event Action<EnumKey>? KeyDown;
    public event Action<EnumKey>? KeyUp;
    public static Window? MainWindow { get; set; }
    public AppKeyboardListener()
    {
        if (MainWindow == null)
        {
            throw new CustomBasicException("MainWindow must be set before using AppKeyboardListener.");
        }
        MainWindow.PreviewKeyDown += MainWindow_PreviewKeyDown;
        MainWindow.PreviewKeyUp += MainWindow_PreviewKeyUp;
    }

    private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        KeyDown?.Invoke((EnumKey)KeyInterop.VirtualKeyFromKey(e.Key));
    }

    private void MainWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        KeyUp?.Invoke((EnumKey)KeyInterop.VirtualKeyFromKey(e.Key));
    }

    public void Dispose()
    {
        if (MainWindow is null)
        {
            return;
        }
        MainWindow.PreviewKeyDown -= MainWindow_PreviewKeyDown;
        MainWindow.PreviewKeyUp -= MainWindow_PreviewKeyUp;
        GC.SuppressFinalize(this); // Added to address CA1816
    }
}