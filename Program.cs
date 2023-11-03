using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

try
{
    
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;

    IntPtr consoleWindow = Process.GetCurrentProcess().MainWindowHandle;
    
    AddToStartup();
    ShowWindow(consoleWindow, SW_HIDE);
    
    string username;
    
    if (File.Exists("C:\\Users\\Public\\Documents\\username.txt")) 
    {
        username = File.ReadAllText("C:\\Users\\Public\\Documents\\username.txt");
    }
    else
    {
        ShowWindow(consoleWindow, SW_SHOW);
        Console.WriteLine("Enter a username:");
        username = Console.ReadLine();
        File.WriteAllText("C:\\Users\\Public\\Documents\\username.txt", username);
        ShowWindow(consoleWindow, SW_HIDE);
    }
    
    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = "runas",
        Arguments = $"/user:{username} /savecred \"cmd /C start gpupdate /force\"",
        Verb = "runas"
    };
    
    Process.Start(psi);
}  
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

static void AddToStartup()
{
    string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
    string keyName = "AutoGPUpdate";

    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
    {
        if (key == null)
        {
            using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
            {
                subKey.SetValue(keyName, path);
            }
        }
        else
        {
            key.SetValue(keyName, path);
        }
    }
}