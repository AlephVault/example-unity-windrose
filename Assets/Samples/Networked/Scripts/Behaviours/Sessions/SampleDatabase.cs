using GMM.Types;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            public class SampleDatabase : MonoBehaviour
            {
                public class DBException : GMM.Types.Exception
                {
                    public DBException() { }
                    public DBException(string message) : base(message) { }
                    public DBException(string message, System.Exception inner) : base(message, inner) { }
                }

                public class LoginFailed : DBException
                {
                    public LoginFailed() { }
                    public LoginFailed(string message) : base(message) { }
                    public LoginFailed(string message, System.Exception inner) : base(message, inner) { }
                }

                public class NegativeID : DBException
                {
                    public NegativeID() { }
                    public NegativeID(string message) : base(message) { }
                    public NegativeID(string message, System.Exception inner) : base(message, inner) { }
                }

                [Serializable]
                public class Account
                {
                    public string Username;
                    public string Password;
                    public int[] CharacterIDs;
                }

                [Serializable]
                public class Character
                {
                    public string CharName;
                    public GameObject Prefab;
                }

                [Serializable]
                public class Characters : SerializableDictionary<int, Character> { }

                [Serializable]
                public class Accounts : SerializableDictionary<int, Account> { }

                [SerializeField]
                private Accounts AccountsTable;

                [SerializeField]
                private Characters CharactersTable;

                public int Login(string username, string password)
                {
                    foreach(KeyValuePair<int, Account> pair in AccountsTable)
                    {
                        if (pair.Value.Password == password && username.ToLower() == pair.Value.Username.ToLower())
                        {
                            return pair.Key;
                        }
                    }
                    throw new LoginFailed();
                }

                public Account GetAccount(int id)
                {
                    if (id < 0)
                    {
                        throw new NegativeID();
                    }
                    Account account;
                    AccountsTable.TryGetValue(id, out account);
                    return account;
                }

                public Character GetAccountCharacter(int id, int characterId)
                {
                    if (id < 0)
                    {
                        throw new NegativeID();
                    }

                    Account account;
                    if (!AccountsTable.TryGetValue(id, out account))
                    {
                        return null;
                    }

                    if (characterId < 0)
                    {
                        throw new NegativeID();
                    }

                    Character character = null;
                    if (account.CharacterIDs.Contains(characterId))
                    {
                        CharactersTable.TryGetValue(characterId, out character);
                    }
                    return character;
                }

                public Characters ListAccountCharacters(int id)
                {
                    if (id < 0)
                    {
                        throw new NegativeID();
                    }

                    Account account;
                    if (!AccountsTable.TryGetValue(id, out account))
                    {
                        return null;
                    }

                    Characters characters = new Characters();
                    foreach(int characterId in account.CharacterIDs)
                    {
                        characters[characterId] = CharactersTable[characterId];
                    }
                    return characters;
                }
            }
        }
    }
}
