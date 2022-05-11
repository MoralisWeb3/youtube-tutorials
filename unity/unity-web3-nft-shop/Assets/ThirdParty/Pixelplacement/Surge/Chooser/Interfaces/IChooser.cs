namespace Pixelplacement
{
    interface IChooser
    {
        void Selected();
        void Deselected();
        void Pressed();
        void Released();
    }
}