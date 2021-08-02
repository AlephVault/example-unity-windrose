using AlephVault.Unity.MMO.Samples.Behaviours.Chat;
using System.Linq;
using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AlephVault.Unity.MMO.Samples.Behaviours.Realms;

namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace UI
        {
            namespace Realms
            {
                public class ChatWithRealm : MonoBehaviour
                {
                    private Button joinChannel;
                    private Button leaveChannel;
                    private Button showChannel;
                    private Button sendMessage;
                    private InputField channelName;
                    private InputField message;
                    private Text messages;
                    private Text users;

                    private string currentChannelName = "";

                    // Start is called before the first frame update
                    void Start()
                    {
                        joinChannel = transform.Find("ChannelJoin").gameObject.GetComponent<Button>();
                        leaveChannel = transform.Find("ChannelLeave").gameObject.GetComponent<Button>();
                        showChannel = transform.Find("ChannelShow").gameObject.GetComponent<Button>();
                        sendMessage = transform.Find("SendMessage").gameObject.GetComponent<Button>();
                        channelName = transform.Find("ChannelName").gameObject.GetComponent<InputField>();
                        message = transform.Find("Message").gameObject.GetComponent<InputField>();
                        messages = transform.Find("Messages").Find("Content").GetComponent<Text>();
                        users = transform.Find("Users").Find("Content").GetComponent<Text>();

                        joinChannel.onClick.AddListener(JoinChannel_Click);
                        leaveChannel.onClick.AddListener(LeaveChannel_Click);
                        showChannel.onClick.AddListener(ShowChannel_Click);
                        sendMessage.onClick.AddListener(SendMessage_Click);
                    }

                    private void JoinChannel_Click()
                    {
                        ChatUser.Owned()?.Join(channelName.text);
                        ShowChannel_Click();
                    }

                    private void LeaveChannel_Click()
                    {
                    }

                    private void ShowChannel_Click()
                    {
                        currentChannelName = channelName.text;
                        Debug.Log("Current channel is: " + currentChannelName);
                    }

                    private void SendMessage_Click()
                    {
                        ChatUser.Owned()?.Say(message.text);
                    }

                    private void OnDestroy()
                    {
                        if (joinChannel) joinChannel.onClick.AddListener(JoinChannel_Click);
                        if (leaveChannel) leaveChannel.onClick.AddListener(LeaveChannel_Click);
                        if (showChannel) showChannel.onClick.AddListener(ShowChannel_Click);
                        if (sendMessage) sendMessage.onClick.AddListener(SendMessage_Click);
                    }

                    // Update is called once per frame
                    void Update()
                    {
                        bool enabled = NetworkManager.Singleton.IsClient;
                        joinChannel.interactable = leaveChannel.interactable =
                            showChannel.interactable = sendMessage.interactable =
                            channelName.interactable = message.interactable = enabled;
                        ChatRoom currentChannel = ChatRoom.Find(currentChannelName);
                        if (currentChannel)
                        {
                            messages.text = string.Join("\n", (from message in currentChannel.Messages select string.Format("{0} - {1}: {2}", message.Date, message.UserName, message.Body)).AsEnumerable().ToArray());
                            users.text = string.Join("\n", (from user in currentChannel.Users select user.UserName.Value).AsEnumerable().ToArray());
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
}