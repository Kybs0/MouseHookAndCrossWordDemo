using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Gma.System.MouseKeyHook;

namespace MouseHookDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _hWndNextViewer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mouseHookManager = new MouseHookManager();
            mouseHookManager.Init();
            Initialize();
        }
        public void Initialize()
        {
            var wih = new WindowInteropHelper(this);
            var hWndSource = HwndSource.FromHwnd(wih.Handle);
            if (hWndSource != null)
            {
                hWndSource.AddHook(this.WinProc); // start processing window messages
                _hWndNextViewer = Win32.SetClipboardViewer(hWndSource.Handle); // set this window as a viewer
            }
        }

        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WmChangecbchain:
                    //Debug.WriteLine("WinProc,Win32.WmChangecbchain");
                    if (wParam == _hWndNextViewer)
                        this._hWndNextViewer = lParam; //clipboard viewer chain changed, need to fix it.
                    else if (_hWndNextViewer != IntPtr.Zero)
                        Win32.SendMessage(_hWndNextViewer, msg, wParam, lParam); //pass the message to the next viewer.
                    break;
                case Win32.WmDrawclipboard:
                    Win32.SendMessage(_hWndNextViewer, msg, wParam, lParam); //pass the message to the next viewer //clipboard content changed
                    if (Clipboard.ContainsText() && !string.IsNullOrEmpty(Clipboard.GetText().Trim()))
                    {
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            (Action)
                                delegate
                                {
                                    var currentText = Clipboard.GetText().Trim();
                                    if (!string.IsNullOrEmpty(currentText))
                                    {
                                        MyTestTextBlock.Text = currentText;
                                    }
                                });
                    }
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
