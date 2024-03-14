using UnityEngine;

public class Title : MonoBehaviour
{
    // 서버 리스트 관련 변수
    [SerializeField]
    private GameObject disableServerListGroup;
    [SerializeField]
    private GameObject serverListGroup;
    [SerializeField]
    private Animator serverListAnimator;

    [SerializeField]
    private Login login;

    // 상태 변수
    private bool isSelectServer;

    public void OnServerSelectButtonClick()
    {
        if (isSelectServer)
        {
            serverListGroup.SetActive(false);
            disableServerListGroup.SetActive(true);

            isSelectServer = false;

            return;
        }

        isSelectServer = true;

        disableServerListGroup.SetActive(false);
        serverListGroup.SetActive(true);
        serverListAnimator.SetTrigger("Show");
    }

    public void OnServerButtonClick(int serverId)
    {
        login.ActiveLoginGroup(serverId);
    }
}
