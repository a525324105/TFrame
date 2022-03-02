public interface ILogicSys
{
    bool OnInit();

    void OnDestroy();

    void OnStart();

    void OnUpdate();

    void OnLateUpdate();

    void OnRoleLogout();

    void OnMapChanged();

    void OnPause();

    void OnResume();
}