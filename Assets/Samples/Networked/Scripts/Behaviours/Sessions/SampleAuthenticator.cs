using System;
using UnityEngine;
using NetRose.Behaviours.Auth;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            [RequireComponent(typeof(SampleDatabase))]
            public class SampleAuthenticator : StandardAuthenticator<SampleAuthMessage, int>
            {
                private SampleDatabase database;
                public string Username;
                public string Password;

                private void Awake()
                {
                    database = GetComponent<SampleDatabase>();
                }

                protected override int Authenticate(SampleAuthMessage request)
                {
                    try
                    {
                        return database.Login(request.Username, request.Password);
                    }
                    catch(SampleDatabase.LoginFailed)
                    {
                        AccountError("mismatch", null);
                    }
                    catch(Exception)
                    {
                        AccountError("unknown", null);
                    }
                    // Unreachable
                    return 0;
                }

                protected override SampleAuthMessage BuildAuthMessage()
                {
                    return new SampleAuthMessage(Username, Password);
                }
            }
        }
    }
}
