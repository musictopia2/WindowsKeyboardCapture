namespace WindowsKeyboardCapture;
public class KeyboardHook
{
    [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, KBDLLHookProc HookProc, IntPtr hInstance, int wParam);
    [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int UnhookWindowsHookEx(int idHook);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
    private delegate int KBDLLHookProc(int nCode, IntPtr wParam, IntPtr lParam);
    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public KBDLLHOOKSTRUCTFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
    [Flags]
    private enum KBDLLHOOKSTRUCTFlags : uint
    {
        LLKHF_EXTENDED = 0x1,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN = 0x20,
        LLKHF_UP = 0x80
    }
    public event KeyDownEventHandler? KeyDown;
    public delegate void KeyDownEventHandler(EnumKey key);
    public event KeyUpEventHandler? KeyUp;
    public delegate void KeyUpEventHandler(EnumKey key);
    private readonly KBDLLHookProc _kBDLLHookProcDelegate;
    private readonly IntPtr _hHookID = IntPtr.Zero;
    private const int _wH_KEYBOARD_LL = 13;
    private const int _hC_ACTION = 0;
    private const int _wM_KEYDOWN = 0x100;
    private const int _wM_KEYUP = 0x101;
    private const int _wM_SYSKEYDOWN = 0x104;
    private const int _wM_SYSKEYUP = 0x105;
#nullable disable
    private int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode == _hC_ACTION)
        {
            KBDLLHOOKSTRUCT @struct = default;
            if ((int)wParam == _wM_KEYDOWN || (int)wParam == _wM_SYSKEYDOWN)
            {
                KeyDown?.Invoke((EnumKey)((KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam,
                                                                                      structureType: @struct.GetType())).vkCode);
            }
            else if ((int)wParam == _wM_KEYUP || (int)wParam == _wM_SYSKEYUP)
            {
                KeyUp?.Invoke((EnumKey)((KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, @struct.GetType())).vkCode);
            }
        }
        return CallNextHookEx((int)IntPtr.Zero, nCode, wParam, lParam);
    }
    public KeyboardHook()
    {
        Process curProcess = Process.GetCurrentProcess();
        ProcessModule curModule = curProcess.MainModule!;
        _kBDLLHookProcDelegate = new KBDLLHookProc(KeyboardProc);
        _hHookID = (IntPtr)SetWindowsHookEx(_wH_KEYBOARD_LL, _kBDLLHookProcDelegate, GetModuleHandle(curModule.ModuleName!), 0);
        if (_hHookID == IntPtr.Zero)
        {
            throw new Exception("Could not set keyboard hook");
        }
    }
    #region IDisposable Support
    private bool _disposedValue = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {

            }
            if (!(_hHookID == IntPtr.Zero))
            {
                _ = UnhookWindowsHookEx((int)_hHookID);
            }
            _disposedValue = true;
        }
    }
    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}