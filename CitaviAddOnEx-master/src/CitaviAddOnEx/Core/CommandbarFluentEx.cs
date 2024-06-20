using System;

namespace SwissAcademic.Controls
{
    public static class CommandbarFluentEx
    {
        public static Commandbar CreateCommandbarMenu(this Commandbar commandbar, string key, string text, Action<CommandbarMenu> createdAction)
        {
            var menu = commandbar.AddCommandbarMenu(key, text);
            if (menu != null)
            {
                createdAction?.Invoke(menu);
            }
            return commandbar;
        }

        public static Commandbar CreateCommandbarButton(this Commandbar commandbar, string key, string text, Action<CommandbarButton> createdAction)
        {
            var button = commandbar.AddCommandbarButton(key, text);
            if (button != null)
            {
                createdAction?.Invoke(button);
            }
            return commandbar;
        }

        public static Commandbar CreateCommandbarMenu(this Commandbar commandbar, int index, string key, string text, Action<CommandbarMenu> createdAction)
        {
            var menu = commandbar.InsertCommandbarMenu(index, key, text);
            if (menu != null)
            {
                createdAction?.Invoke(menu);
            }
            return commandbar;
        }

        public static Commandbar CreateCommandbarButton(this Commandbar commandbar, int index, string key, string text, Action<CommandbarButton> createdAction)
        {
            var button = commandbar.InsertCommandbarButton(index, key, text);
            if (button != null)
            {
                createdAction?.Invoke(button);
            }
            return commandbar;
        }

        public static Commandbar UpdateCommandbarMenu(this Commandbar commandbar, string key, Action<CommandbarMenu> updateAction)
        {
            var menu = commandbar.GetCommandbarMenu(key);
            if (menu != null)
            {
                updateAction?.Invoke(menu);
            }
            return commandbar;
        }

        public static Commandbar UpdateCommandbarButton(this Commandbar commandbar, string key, Action<CommandbarButton> updateAction)
        {
            var button = commandbar.GetCommandbarButton(key);
            if (button != null)
            {
                updateAction?.Invoke(button);
            }
            return commandbar;
        }
    }
}
