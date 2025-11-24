namespace MVC
{
    public interface IInteractable
    {
        bool BeginInteract(PlayerCtl player); // 返回 false 表示拒绝开始
        void EndInteract(PlayerCtl player);
    }
}
