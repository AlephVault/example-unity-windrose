using AlephVault.Unity.MMO.Samples.Behaviours.Chat;
using System.Linq;
using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace UI
        {
            public class Chat : MonoBehaviour
            {
                private Button changeNickName;
                private Button joinChannel;
                private Button leaveChannel;
                private Button showChannel;
                private Button sendMessage;
                private InputField channelName;
                private InputField nickName;
                private InputField message;
                private Text messages;
                private Text users;

                private string currentChannelName = "";

                // Start is called before the first frame update
                void Start()
                {
                    changeNickName = transform.Find("NickNameChange").gameObject.GetComponent<Button>();
                    joinChannel = transform.Find("ChannelJoin").gameObject.GetComponent<Button>();
                    leaveChannel = transform.Find("ChannelLeave").gameObject.GetComponent<Button>();
                    showChannel = transform.Find("ChannelShow").gameObject.GetComponent<Button>();
                    sendMessage = transform.Find("SendMessage").gameObject.GetComponent<Button>();
                    nickName = transform.Find("NickName").gameObject.GetComponent<InputField>();
                    channelName = transform.Find("ChannelName").gameObject.GetComponent<InputField>();
                    message = transform.Find("Message").gameObject.GetComponent<InputField>();
                    messages = transform.Find("Messages").Find("Content").GetComponent<Text>();
                    users = transform.Find("Users").Find("Content").GetComponent<Text>();

                    changeNickName.onClick.AddListener(ChangeNickName_Click);
                    joinChannel.onClick.AddListener(JoinChannel_Click);
                    leaveChannel.onClick.AddListener(LeaveChannel_Click);
                    showChannel.onClick.AddListener(ShowChannel_Click);
                    sendMessage.onClick.AddListener(SendMessage_Click);
                }

                private User GetCurrentUser()
                {
                    if (NetworkManager.Singleton.IsClient)
                    {
                        return NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<User>();
                    }
                    return null;
                }

                private void ChangeNickName_Click()
                {
                    User user = GetCurrentUser();
                    if (user != null) user.Nickname = nickName.text;
                }

                private void JoinChannel_Click()
                {
                    GetCurrentUser()?.Join(channelName.text);
                    ShowChannel_Click();
                }

                private void LeaveChannel_Click()
                {
                    GetCurrentUser()?.Leave(channelName.text);
                    if (channelName.text == currentChannelName) currentChannelName = "";
                }

                private void ShowChannel_Click()
                {
                    currentChannelName = channelName.text;
                }

                private void SendMessage_Click()
                {
                    GetCurrentUser()?.Say(message.text, currentChannelName);
                }

                private void OnDestroy()
                {
                    if (changeNickName) changeNickName.onClick.AddListener(ChangeNickName_Click);
                    if (joinChannel) joinChannel.onClick.AddListener(JoinChannel_Click);
                    if (leaveChannel) leaveChannel.onClick.AddListener(LeaveChannel_Click);
                    if (showChannel) showChannel.onClick.AddListener(ShowChannel_Click);
                    if (sendMessage) sendMessage.onClick.AddListener(SendMessage_Click);
                }

                // Update is called once per frame
                void Update()
                {
                    bool enabled = NetworkManager.Singleton.IsClient;
                    changeNickName.interactable = joinChannel.interactable = leaveChannel.interactable =
                        showChannel.interactable = sendMessage.interactable =
                        nickName.interactable = channelName.interactable = message.interactable = enabled;
                    Channel currentChannel = Channel.Find(currentChannelName);
                    if (currentChannel)
                    {
                        messages.text = string.Join("\n", (from message in currentChannel.Messages() select message).AsEnumerable().ToArray());
                        users.text = string.Join("\n", currentChannel.UserNames());
                    }
                    else
                    {
                        messages.text = "";
                        users.text = "";
                    }
                }
            }
        }
    }
}