using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace PartyScreenEnhancements
{
    public static class Utilities
    {
        public static void DisplayMessage(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, Color.White));
        }
        
        public static void DisplayMessage(string message, Color color)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, color));
        }

        public static bool IsControlDown()
        {
            return InputKey.LeftControl.IsDown() || InputKey.RightControl.IsDown();
        }

        public static bool IsShiftDown()
        {
            return InputKey.LeftShift.IsDown() || InputKey.RightShift.IsDown();
        }

    }
}
