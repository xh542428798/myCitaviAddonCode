using System;

namespace SwissAcademic.Controls
{
    public static class CommandbarMenuFluentEx
    {
        public static CommandbarMenu CreateCommandbarMenu(this CommandbarMenu commandbarMenu, string key, string text, Action<CommandbarMenu> createdAction)
        {
            var menu = commandbarMenu.AddCommandbarMenu(key, text);
            if (menu != null)
            {
                createdAction?.Invoke(menu);
            }
            return commandbarMenu;
        }

        public static CommandbarMenu CreateCommandbarButton(this CommandbarMenu commandbarMenu, string key, string text, Action<CommandbarButton> createdAction)
        {
            var button = commandbarMenu.AddCommandbarButton(key, text);
            if (button != null)
            {
                createdAction?.Invoke(button);
            }
            return commandbarMenu;
        }

        public static CommandbarMenu CreateCommandbarMenu(this CommandbarMenu commandbarMenu, int index, string key, string text, Action<CommandbarMenu> createdAction)
        {
            var menu = commandbarMenu.InsertCommandbarMenu(index, key, text);
            if (menu != null)
            {
                createdAction?.Invoke(menu);
            }
            return commandbarMenu;
        }

        public static CommandbarMenu CreateCommandbarButton(this CommandbarMenu commandbarMenu, int index, string key, string text, Action<CommandbarButton> createdAction)
        {
            var button = commandbarMenu.InsertCommandbarButton(index, key, text);
            if (button != null)
            {
                createdAction?.Invoke(button);
            }
            return commandbarMenu;
        }

        public static CommandbarMenu UpdateCommandbarMenu(this CommandbarMenu commandbarMenu, string key, Action<CommandbarMenu> updateAction)
        {
            var menu = commandbarMenu.GetCommandbarMenu(key);
            if (menu != null)
            {
                updateAction?.Invoke(menu);
            }
            return commandbarMenu;
        }

        public static CommandbarMenu UpdateCommandbarButton(this CommandbarMenu commandbarMenu, string key, Action<CommandbarButton> updateAction)
        {
            var button = commandbarMenu.GetCommandbarButton(key);
            if (button != null)
            {
                updateAction?.Invoke(button);
            }
            return commandbarMenu;
        }
    }

}
