using UnityEngine;

namespace RoguelikeMap
{
    public enum WindowFormat
    {
        Small,
        Half,
        FullScreen
    }
    
    public class DialogWindowFormat
    {
        public Vector3 TextPosition { get; private set; }
        public WindowFormat WindowFormat { get; private set; }
        public Vector3 ImagePosition { get; private set; }
        public string Text { get; private set; }

        public DialogWindowFormat(WindowFormat windowFormat, Vector3 textPosition, Vector3 imagePosition, string text)
        {
            WindowFormat = windowFormat;
            TextPosition = textPosition;
            ImagePosition = imagePosition;
            Text = text;
        }
    }
}