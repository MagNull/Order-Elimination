using UnityEngine;

namespace RoguelikeMap
{
    public enum WindowFormat
    {
        None,
        Small,
        Half,
        FullScreen
    }
    
    public class DialogWindowData
    {
        public Vector3 TargetOnCanvas { get; } = new Vector3(1600, 100);
        public Vector3 TargetBehindCanvas { get; } = new Vector3(2500, 100);
        public Vector3 TextPosition { get; private set; }
        public WindowFormat WindowFormat { get; private set; }
        public Vector3 ImagePosition { get; private set; }
        public string Text { get; private set; }

        public DialogWindowData(WindowFormat windowFormat, Vector3 textPosition, Vector3 imagePosition, string text)
        {
            WindowFormat = windowFormat;
            TextPosition = textPosition;
            ImagePosition = imagePosition;
            Text = text;
        }
    }
}