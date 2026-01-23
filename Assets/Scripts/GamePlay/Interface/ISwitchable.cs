namespace GamePlay.Player.Interface
{
    /**
     * 开关接口，定义可切换对象的行为
     */
    public interface ISwitchable
    {
        bool IsActive { get; }
        void OnSwitchOn();
        void OnSwitchOff();
    }
}