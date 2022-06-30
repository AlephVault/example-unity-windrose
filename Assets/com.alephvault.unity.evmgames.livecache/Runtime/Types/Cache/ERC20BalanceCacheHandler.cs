using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        namespace Cache
        {
            /// <summary>
            ///   An abstraction over an EVM ERC20 balance
            ///   resource. Used to query balances and to
            ///   reset the cache.
            /// </summary>
            public class ERC20BalanceCacheHandler
            {
                private class BalanceOfResult
                {
                    [JsonProperty("amount")]
                    public string Amount;
                }

                private class BalancesResultEntry
                {
                    [JsonProperty("owner")]
                    public string Owner;
                    
                    [JsonProperty("amount")]
                    public string Amount;
                }

                /// <summary>
                ///   The related resource.
                /// </summary>
                public readonly SimpleResource<ERC20Balance> ERC20BalanceResource;
                
                /// <summary>
                ///   Creates the instance from a specific resource
                ///   (a root one) and a resource.
                /// </summary>
                /// <param name="root">The root resource</param>
                /// <param name="resource">The resource key</param>
                public ERC20BalanceCacheHandler(Root root, string resource = "evm-erc20-balance")
                {
                    ERC20BalanceResource = (SimpleResource<ERC20Balance>)root.GetSimple<ERC20Balance>(resource);
                }
                
                /// <summary>
                ///   Resets the cache for a given contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                public async Task<Result<JObject, string>> Reset(string contractKey)
                {
                    return await ERC20BalanceResource.OperationToJson(
                        "reset-cache", new Dictionary<string, string>()
                        {
                            { "contract-key", contractKey }
                        }
                    );
                }

                // Parses an amount.
                private BigInteger Parse(string value)
                {
                    BigInteger result;
                    BigInteger.TryParse(value, out result);
                    return result;
                }

                /// <summary>
                ///   Queries all the balances inside a contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                /// <param name="offset">The offset for the query</param>
                /// <param name="limit">The limit for the query</param>
                /// <returns>the balances</returns>
                public async Task<Result<Tuple<string, BigInteger>[], string>> Balances(
                    string contractKey, uint offset, uint limit
                ) {
                    Result<BalancesResultEntry[], string> result = await ERC20BalanceResource.ViewTo<BalancesResultEntry[]>(
                        "balances", new Dictionary<string, string>
                        {
                            { "contract-key", contractKey },
                            { "offset", offset.ToString() },
                            { "limit", limit.ToString() }
                        }
                    );

                    if (result.Code == ResultCode.Ok)
                    {
                        int length = result.Element.Length;
                        Tuple<string, BigInteger>[] values = new Tuple<string, BigInteger>[length];
                        for (int i = 0; i < length; i++)
                        {
                            BalancesResultEntry entry = result.Element[i];
                            BigInteger.TryParse(entry.Amount, out var number);
                            values[i] = new Tuple<string, BigInteger>(entry.Owner, number);
                        }
                        return new Result<Tuple<string, BigInteger>[], string>
                        {
                            Element = (from entry in result.Element
                                       select new Tuple<string, BigInteger>(
                                           entry.Owner, Parse(entry.Amount)
                                       )).ToArray(),
                            Code = result.Code
                        };
                    }
                    
                    return new Result<Tuple<string, BigInteger>[], string>
                    {
                        Element = null,
                        Code = result.Code
                    };
                }

                /// <summary>
                ///   Queries the balance of an address inside a contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                /// <param name="owner">The address of the owner</param>
                /// <returns>the balance for the address</returns>
                public async Task<Result<BigInteger, string>> BalanceOf(string contractKey, string owner)
                {
                    Result<BalanceOfResult, string> result = await ERC20BalanceResource.ViewTo<BalanceOfResult>(
                        "balance-of", new Dictionary<string, string>
                        {
                            { "contract-key", contractKey },
                            { "owner", owner }
                        }
                    );

                    if (result.Code == ResultCode.Ok)
                    {
                        return new Result<BigInteger, string>
                        {
                            Element = Parse(result.Element.Amount),
                            Code = result.Code
                        };
                    }

                    return new Result<BigInteger, string>
                    {
                        Element = default,
                        Code = result.Code
                    };
                }
            }
        }
    }
}
