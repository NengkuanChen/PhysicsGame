namespace Game.UI.Form.Control
{
    public class ControlForm : GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        static ControlForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(ControlForm), UiDepth.Control);
        }

    }
}