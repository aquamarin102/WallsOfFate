public static class InputModeTracker
{
    public static bool UsingKeyboard { get; private set; } = false;

    public static void NotifyKeyboardInput()
    {
        UsingKeyboard = true;
    }

    public static void NotifyMouseInput()
    {
        UsingKeyboard = false;
    }
}