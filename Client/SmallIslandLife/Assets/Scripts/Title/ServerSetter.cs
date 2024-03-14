using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerSetter : MonoBehaviour
{
    [SerializeField]
    private Title title;
    [SerializeField]
    private GameObject serverConnectButtonPrefab;

    private void Awake()
    {
        for (int i = 0; i < ServerSetting.Instance.serverConfigs.Length; i++)
        {
            GameObject instance = Instantiate(serverConnectButtonPrefab, transform);

            instance.name = $"ServerButtun({ServerSetting.Instance.serverConfigs[i].name})";

            int index = i;

            instance.GetComponent<Button>().onClick.AddListener(() => title.OnServerButtonClick(ServerSetting.Instance.serverConfigs[index].id));
            instance.GetComponentInChildren<TextMeshProUGUI>().text = ServerSetting.Instance.serverConfigs[i].name;
        }
    }
}
