using System;
using System.Collections;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace CommandExchange
            {
                /// <summary>
                ///   <para>
                ///     Sends a command to an adjacent object. If the object contains a
                ///       <see cref="CommandReceiver"/> component, it could process the
                ///       command being sent.
                ///   </para>
                ///   <para>
                ///     The object will throw command through <see cref="Cast(string, bool, object[])"/>,
                ///       which may be instantaneous (i.e. just one frame) or stay until
                ///       the command is released with <see cref="Release"/>. Only one command
                ///       may exist at a time: succesive calls will release former still-alive
                ///       commands.
                ///   </para>
                /// </summary>
                /// <remarks>
                ///   If the object being hit by a non-instantaneous command moves away, then
                ///     it will count as if the command was released. Also, when it moves again
                ///     into the place where the command was hit, it will count as the command
                ///     being cast again. A good practice is to drop instantaneous commands
                ///     to avoid this case.
                /// </remarks>
                [RequireComponent(typeof(Oriented))]
                class CloseCommandSender : MonoBehaviour, Common.Pausable.IPausable
                {
                    // The command currently being sent.
                    private Misc.Command command;
                    private Oriented oriented;
                    private Object mapObject;
                    private bool paused = false;

                    private void Start()
                    {
                        oriented = GetComponent<Oriented>();
                        mapObject = GetComponent<Object>();
                    }

                    private void FixCommandPosition()
                    {
                        float x, y;
                        switch (oriented.orientation)
                        {
                            case Types.Direction.DOWN:
                                x = mapObject.transform.position.x + (mapObject.Width / 2f) * mapObject.GetCellWidth();
                                y = mapObject.transform.position.y - 0.5f * mapObject.GetCellHeight();
                                break;
                            case Types.Direction.UP:
                                x = mapObject.transform.position.x + (mapObject.Width / 2f) * mapObject.GetCellWidth();
                                y = mapObject.transform.position.y + (mapObject.Height + 0.5f) * mapObject.GetCellHeight();
                                break;
                            case Types.Direction.LEFT:
                                y = mapObject.transform.position.y + (mapObject.Height / 2f) * mapObject.GetCellHeight();
                                x = mapObject.transform.position.x - 0.5f * mapObject.GetCellWidth();
                                break;
                            case Types.Direction.RIGHT:
                                y = mapObject.transform.position.y + (mapObject.Height / 2f) * mapObject.GetCellHeight();
                                x = mapObject.transform.position.x + (mapObject.Width + 0.5f) * mapObject.GetCellWidth();
                                break;
                            default:
                                x = command.transform.position.x;
                                y = command.transform.position.y;
                                break;
                        }
                        command.transform.position = new Vector3(x, y, command.transform.position.z);
                    }

                    private void SetCommandData(string commandName, params object[] arguments)
                    {
                        command.sender = gameObject;
                        command.name = commandName;
                        command.arguments = arguments;
                    }

                    /// <summary>
                    ///   Casts a command in the direction it is looking to.
                    /// </summary>
                    /// <param name="commandName">The command name</param>
                    /// <param name="instantaneous">
                    ///   Whether the command will flash, or will be present untilthe next call
                    ///   to <see cref="Release"/> or another call to <see cref="Cast(string, bool, object[])"/>
                    /// </param>
                    /// <param name="arguments">The command arguments</param>
                    public void Cast(string commandName, bool instantaneous = true, params object[] arguments)
                    {
                        if (paused) return;
                        Release();
                        GameObject commandObject = new GameObject("Command");
                        CircleCollider2D collider = Support.Utils.Layout.AddComponent<CircleCollider2D>(commandObject);
                        collider.enabled = false;
                        collider.isTrigger = true;
                        command = Support.Utils.Layout.AddComponent<Misc.Command>(commandObject);
                        SetCommandData(commandName, arguments);
                        FixCommandPosition();
                        collider.enabled = true;
                        if (instantaneous)
                        {
                            StartCoroutine(InstantRelease());
                        }
                    }

                    private IEnumerator InstantRelease()
                    {
                        yield return new WaitForSeconds(0f);
                        Release();
                    }

                    /// <summary>
                    ///   Releases the last command sent by <see cref="Cast(string, bool, object[])"/>, if still present.
                    /// </summary>
                    public void Release()
                    {
                        if (command)
                        {
                            Destroy(command.gameObject);
                            command = null;
                        }
                    }

                    /// <summary>
                    ///   Pauses this behaviour - it will not cast anything.
                    /// </summary>
                    public void Pause(bool fullFreeze)
                    {
                        paused = true;
                    }

                    /// <summary>
                    ///   Resumes the behaviour - it will cast again if told to.
                    /// </summary>
                    public void Resume()
                    {
                        paused = false;
                    }
                }
            }
        }
    }
}
