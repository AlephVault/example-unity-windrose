﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Visuals
            {
                /// <summary>
                ///   <para>
                ///     Holds several states under a dictionary. States are intended to be visual, and
                ///       subclasses will implement them (usually as animations, animation roses, or
                ///       static images). Since they are only intended to be visual, only one instance
                ///       of MultiState should be used and their appropriate context should be taken
                ///       into account (e.g. cannot add a static multi while Animated is present, or
                ///       cannot add rose-animated while animated multi is present,...).
                ///   </para>
                ///   <para>
                ///     Elements can be even temporarily replaced. Aside from this replacement feature,
                ///       no other logic will be in particular supported here (it must belong to other
                ///       behaviour(s)), like state changes.
                ///   </para>
                ///   <para>
                ///     Fallbacks are another feature of our interest. Let's assume we have a character
                ///       main visual holding 2 states: standing, movement. We'd then create an "aura"
                ///       visual that would have only one single animation: standing. Our goal is to use
                ///       "standing" regardless what other visuals in the same object, or even the
                ///       object key, is telling to: for that purpose, this visual would have just one
                ///       animation at "standing" key, and a fallback "movement" -> "standing".
                ///   </para>
                /// </summary>
                public abstract class MultiState<StateType> : VisualBehaviour
                {
                    /***************** Core data elements *****************/

                    /// <summary>
                    ///   The default state key.
                    /// </summary>
                    public const string IDLE = "";

                    /// <summary>
                    ///   Tells when an error is raised inside multi-state component methods.
                    /// </summary>
                    public class Exception : Types.Exception
                    {
                        public Exception() { }
                        public Exception(string message) : base(message) { }
                        public Exception(string message, System.Exception inner) : base(message, inner) { }
                    }

                    // All the registered states
                    private Dictionary<string, StateType> states = new Dictionary<string, StateType>();

                    // All the temporary replacements for the registered states
                    private Dictionary<string, StateType> replacements = new Dictionary<string, StateType>();

                    // All the fallbacks
                    private Dictionary<string, string> fallbacks = new Dictionary<string, string>();
                    
                    /***************** Data elements *****************/

                    /// <summary>
                    ///   The default state (used for idle state, and usually meaning
                    ///     a resting or stand-up position).
                    /// </summary>
                    [SerializeField]
                    private StateType idleState;

                    /// <summary>
                    ///   The key of the state being rendered. It will be grabbed from
                    ///     a <see cref="Objects.StatePicker"/> component.
                    /// </summary>
                    private string selectedKey = IDLE;

                    // Uses the appropriate state being selected. If something goes wrong,
                    //   the exception will be absorbed, a warning will be issued, and
                    //   the idle state will be set.
                    private void RefreshState(bool allowFallback = true)
                    {
                        try
                        {
                            StateType state;
                            if (replacements.TryGetValue(selectedKey, out state))
                            {
                                UseState(state);
                            }
                            else if (states.TryGetValue(selectedKey, out state))
                            {
                                UseState(state);
                            }
                            else if (allowFallback)
                            {
                                selectedKey = fallbacks[selectedKey];
                                RefreshState(false);
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            // Key IDLE will always be available
                            selectedKey = IDLE;
                        }
                    }

                    protected abstract void UseState(StateType state);

                    /// <summary>
                    ///   Registers an state under a key. This is usually done when initializing
                    ///     other components (e.g. moving components).
                    /// </summary>
                    /// <param name="key">The key to use</param>
                    /// <param name="state">The state to register</param>
                    public void AddState(string key, StateType state)
                    {
                        if (states.ContainsKey(key))
                        {
                            throw new Types.Exception("State key already in use: " + key);
                        }
                        else
                        {
                            states.Add(key, state);
                        }
                    }

                    /// <summary>
                    ///   Replaces an existing state with a new one. This is run at run-time
                    ///     and will require the state being replaced to exist, or fail otherwise.
                    ///     Set state to <c>null</c> to clear the replacement on a given key.
                    /// </summary>
                    /// <param name="key">The key of the state being replaced</param>
                    /// <param name="state">The new state to use, or null to undo the replacement</param>
                    public void ReplaceState(string key, StateType state)
                    {
                        if (states.ContainsKey(key))
                        {
                            throw new Types.Exception("state key does not exist: " + key);
                        }
                        else
                        {
                            if (state != null)
                            {
                                replacements[key] = state;
                            }
                            else
                            {
                                replacements.Remove(key);
                            }
                            RefreshState();
                        }
                    }

                    /// <summary>
                    ///   Adds a fallback value, so when a state is not found given a source key, a
                    ///     destination key will be used instead.
                    /// </summary>
                    public void AddFallback(string key, string fallback)
                    {
                        if (fallbacks.ContainsKey(key))
                        {
                            throw new Types.Exception("state key for fallback already in use: " + key);
                        }
                        fallbacks[key] = fallback;
                    }
                    
                    // The current state picker (belongs to the related object)
                    private Objects.StatePicker picker;

                    // Updates the currently selected key
                    private void OnSelectedKeyChanged(string newKey)
                    {
                        if (newKey != selectedKey)
                        {
                            selectedKey = newKey;
                            RefreshState();
                        }
                    }

                    // On enabled, takes the related object's state picker and binds the event
                    private void OnEnable()
                    {
                        picker = visual.RelatedObject ? visual.RelatedObject.GetComponent<Objects.StatePicker>() : null;
                        if (picker)
                        {
                            picker.onStateKeyChanged.AddListener(OnSelectedKeyChanged);
                        }
                    }

                    // On disabled, releases the event and the picker
                    private void OnDisable()
                    {
                        if (picker)
                        {
                            picker.onStateKeyChanged.RemoveListener(OnSelectedKeyChanged);
                        }
                        picker = null;
                    }

                    protected override void Awake()
                    {
                        base.Awake();
                        // Ensures at least the idle state exists
                        AddState(IDLE, idleState);
                    }
                }
            }
        }
    }
}
 
 