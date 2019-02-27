﻿namespace TUI
{
    public class EnabledArgs
    {
        public RootVisualObject Root { get; set; }
        public bool Value { get; set; }

        public EnabledArgs(RootVisualObject root, bool value)
        {
            Root = root;
            Value = value;
        }
    }
}