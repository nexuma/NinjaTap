using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using AutoHotkey.Interop;

namespace NinjaTap
{
    // MainWindow class inherits from Window
    public partial class MainWindow : Window
    {
        // Create an instance of AutoHotkeyEngine
        private readonly AutoHotkeyEngine ahk = AutoHotkeyEngine.Instance;
        // Boolean to track the enabled state of the script
        private bool _isEnabledState = false;

        // Method to initialize hotkeys and load the AHK script
        private void InitializeHotKeys()
        {
            // Set AHK variables
            ahk.SetVar("A_MaxHotkeysPerInterval", "99000000");
            ahk.SetVar("A_HotkeyInterval", "0");

            // Load the AHK script file, (changed to in code to compile into single .exe file)
            //ahk.LoadFile("libs/snappytap.ahk");
            ahk.ExecRaw(@"; Define global variables to track key states
global aKeyHeld := 0
global dKeyHeld := 0
global wKeyHeld := 0
global sKeyHeld := 0

global aKeyScript := 0
global dKeyScript := 0
global wKeyScript := 0
global sKeyScript := 0

; Function to display a message box with a given value
MsgBox(value)
{
    MsgBox, %value%
}

; Hotkey definitions for key press and release events using scan codes for multi-layout keyboard support
*$SC01E::a_pressed()       ; Call a_pressed() when 'a' is pressed
*$SC01E up::a_released()   ; Call a_released() when 'a' is released

*$SC020::d_pressed()       ; Call d_pressed() when 'd' is pressed
*$SC020 up::d_released()   ; Call d_released() when 'd' is released

*$SC011::w_pressed()       ; Call w_pressed() when 'w' is pressed
*$SC011 up::w_released()   ; Call w_released() when 'w' is released

*$SC01F::s_pressed()       ; Call s_pressed() when 's' is pressed
*$SC01F up::s_released()   ; Call s_released() when 's' is released

; Function to handle 'a' key press
a_pressed()
{
    global

    aKeyHeld := 1

    if dKeyScript
    {
        dKeyScript := 0
        SendInput, {Blind}{SC020 up}
    }

    aKeyScript := 1
    SendInput, {Blind}{SC01E down}
}

; Function to handle 'a' key release
a_released()
{
    global

    aKeyHeld := 0

    if aKeyScript
    {
        aKeyScript := 0
        SendInput, {Blind}{SC01E up}
    }

    if dKeyHeld && !dKeyScript
    {
        dKeyScript := 1
        SendInput, {Blind}{SC020 down}
    }
}

; Function to handle 'd' key press
d_pressed()
{
    global

    dKeyHeld := 1

    if aKeyScript
    {
        aKeyScript := 0
        SendInput, {Blind}{SC01E up}
    }

    dKeyScript := 1
    SendInput, {Blind}{SC020 down}
}

; Function to handle 'd' key release
d_released()
{
    global

    dKeyHeld := 0

    if dKeyScript
    {
        dKeyScript := 0
        SendInput, {Blind}{SC020 up}
    }

    if aKeyHeld && !aKeyScript
    {
        aKeyScript := 1
        SendInput, {Blind}{SC01E down}
    }
}

; Function to handle 'w' key press
w_pressed()
{
    global

    wKeyHeld := 1

    if sKeyScript
    {
        sKeyScript := 0
        SendInput, {Blind}{SC01F up}
    }

    wKeyScript := 1
    SendInput, {Blind}{SC011 down}
}

; Function to handle 'w' key release
w_released()
{
    global

    wKeyHeld := 0

    if wKeyScript
    {
        wKeyScript := 0
        SendInput, {Blind}{SC011 up}
    }

    if sKeyHeld && !sKeyScript
    {
        sKeyScript := 1
        SendInput, {Blind}{SC01F down}
    }
}

; Function to handle 's' key press
s_pressed()
{
    global

    sKeyHeld := 1

    if wKeyScript
    {
        wKeyScript := 0
        SendInput, {Blind}{SC011 up}
    }

    sKeyScript := 1
    SendInput, {Blind}{SC01F down}
}

; Function to handle 's' key release
s_released()
{
    global

    sKeyHeld := 0

    if sKeyScript
    {
        sKeyScript := 0
        SendInput, {Blind}{SC01F up}
    }

    if wKeyHeld && !wKeyScript
    {
        wKeyScript := 1
        SendInput, {Blind}{SC011 down}
    }
}");

            // Suspend the AHK script initially
            ahk.Suspend();
        }

        // Constructor for MainWindow
        public MainWindow()
        {
            InitializeComponent(); // Initialize WPF components
            InitializeHotKeys(); // Initialize hotkeys and load AHK script
        }

        // Event handler for the toggle button click event
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButtonImage = (Image)ToggleButton.Template.FindName("ToggleButtonImage", ToggleButton);

            if (_isEnabledState)
            {
                try
                {
                    // Change background image and button content to indicate disabled state
                    BackgroundImageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/disabled_NINJATAP.png"));
                    toggleButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/off.png"));
                    // Suspend the AHK script
                    ahk.Suspend();
                    _isEnabledState = false; // Update the state to disabled
                }
                catch (Exception ex)
                {
                    // Show error message if disabling the script fails
                    MessageBox.Show($"Failed to disable AHK script (close the app): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    // Change background image and button content to indicate enabled state
                    BackgroundImageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/enabled_NINJATAP.png"));
                    toggleButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/on.png"));
                    // Unsuspend the AHK script
                    ahk.UnSuspend();
                    _isEnabledState = true; // Update the state to enabled
                }
                catch (Exception ex)
                {
                    // Show error message if enabling the script fails
                    MessageBox.Show($"Failed to run AHK script (restart the app): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Event handler for mouse left button down event
        private void ToggleButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var toggleButtonImage = (Image)ToggleButton.Template.FindName("ToggleButtonImage", ToggleButton);
            if (_isEnabledState)
            {
                toggleButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/clicked_on.png"));
            }
            else
            {
                toggleButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/clicked_off.png"));
            }
        }
            // Event handler for mouse left button up event
        private void ToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var toggleButtonImage = (Image)ToggleButton.Template.FindName("ToggleButtonImage", ToggleButton);
            toggleButtonImage.Source = new BitmapImage(new Uri(_isEnabledState ? "pack://application:,,,/Images/on.png" : "pack://application:,,,/Images/off.png"));
        }
    }
}