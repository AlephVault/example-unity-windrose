using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
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
            ///   An abstraction over an EVM ERC1155 ownership
            ///   resource. Used to query ownerships and to
            ///   reset the cache.
            /// </summary>
            public class ERC1155OwnershipCacheHandler
            {
                private class BalancesResultEntry
                {
                    [JsonProperty("owner")]
                    public string Owner;
                    
                    [JsonProperty("token")]
                    public string Token;
                    
                    [JsonProperty("amount")]
                    public string Amount;
                }
                
                private class BalancesOfResultEntry
                {
                    [JsonProperty("token")]
                    public string Token;
                    
                    [JsonProperty("amount")]
                    public string Amount;
                }

                private class BalanceOfResultEntry
                {
                    [JsonProperty("amount")]
                    public string Amount;
                }
                
                /// <summary>
                ///   The related resource.
                /// </summary>
                public readonly SimpleResource<ERC1155Ownership> ERC1155OwnershipResource;
                
                /// <summary>
                ///   Creates the instance from a specific resource
                ///   (a root one) and a resource.
                /// </summary>
                /// <param name="root">The root resource</param>
                /// <param name="resource">The resource key</param>
                public ERC1155OwnershipCacheHandler(Root root, string resource = "evm-erc1155-ownership")
                {
                    ERC1155OwnershipResource = (SimpleResource<ERC1155Ownership>)root.GetSimple<ERC1155Ownership>(
                        resource
                    );
                }
                
                /// <summary>
                ///   Resets the cache for a given contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                public async Task<Result<JObject, string>> Reset(string contractKey)
                {
                    return await ERC1155OwnershipResource.OperationToJson(
                        "reset-cache", new Dictionary<string, string>()
                        {
                            { "contract-key", contractKey }
                        }
                    );
                }
                
                // Parses a token.
                private HexBigInteger ParseToken(string value)
                {
                    try
                    {
                        return new HexBigInteger(value);
                    }
                    catch
                    {
                        return null;
                    }
                }

                // Parses an amount.
                private BigInteger ParseAmount(string value)
                {
                    BigInteger result;
                    BigInteger.TryParse(value, out result);
                    return result;
                }
                
                /// <summary>
                ///   Queries the ownerships inside a contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                /// <param name="offset">The offset for the query</param>
                /// <param name="limit">The limit for the query</param>
                /// <returns>the balances list</returns>
                public async Task<Result<Tuple<string, HexBigInteger, BigInteger>[], string>> Balances(
                    string contractKey, int offset, int limit
                ) {
                    Result<BalancesResultEntry[], string> result = await ERC1155OwnershipResource.ViewTo<BalancesResultEntry[]>(
                        "balances", new Dictionary<string, string>
                        {
                            { "contract-key", contractKey },
                            { "offset", offset.ToString() },
                            { "limit", limit.ToString() }
                        }
                    );

                    if (result.Code == ResultCode.Ok)
                    {
                        return new Result<Tuple<string, HexBigInteger, BigInteger>[], string>
                        {
                            Element = (from element in result.Element
                                       select new Tuple<string, HexBigInteger, BigInteger>(
                                           element.Owner, ParseToken(element.Token), ParseAmount(element.Amount)
                                       )).ToArray(),
                            Code = result.Code
                        };
                    }
                    
                    return new Result<Tuple<string, HexBigInteger, BigInteger>[], string>
                    {
                        Element = null,
                        Code = result.Code
                    };
                }

                /// <summary>
                ///   Queries the ownerships inside a contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                /// <param name="owner">The owner to query</param>
                /// <param name="offset">The offset for the query</param>
                /// <param name="limit">The limit for the query</param>
                /// <returns>the balances list for an address</returns>
                public async Task<Result<Tuple<HexBigInteger, BigInteger>[], string>> BalancesOf(
                    string contractKey, string owner, int offset, int limit
                ) {
                    Result<BalancesOfResultEntry[], string> result = await ERC1155OwnershipResource.ViewTo<BalancesOfResultEntry[]>(
                        "balances-of", new Dictionary<string, string>
                        {
                            { "contract-key", contractKey },
                            { "owner", owner },
                            { "offset", offset.ToString() },
                            { "limit", limit.ToString() }
                        }
                    );

                    if (result.Code == ResultCode.Ok)
                    {
                        return new Result<Tuple<HexBigInteger, BigInteger>[], string>
                        {
                            Element = (from element in result.Element
                                select new Tuple<HexBigInteger, BigInteger>(
                                    ParseToken(element.Token), ParseAmount(element.Amount)
                                )).ToArray(),
                            Code = result.Code
                        };
                    }
                    
                    return new Result<Tuple<HexBigInteger, BigInteger>[], string>
                    {
                        Element = null,
                        Code = result.Code
                    };
                }

                /// <summary>
                ///   Queries the ownerships of an address and token inside
                ///   a contract.
                /// </summary>
                /// <param name="contractKey">The contract key</param>
                /// <param name="owner">The address of the owner</param>
                /// <param name="token">The token to query about</param>
                /// <param name="offset">The offset for the query</param>
                /// <param name="limit">The limit for the query</param>
                /// <returns>the list of ownerships</returns>
                public async Task<Result<BigInteger, string>> BalanceOf(
                    string contractKey, string owner, HexBigInteger token, uint offset, uint limit
                ) {
                    Result<BalanceOfResultEntry, string> result = 
                        await ERC1155OwnershipResource.ViewTo<BalanceOfResultEntry>(
                        "balance-of", new Dictionary<string, string>
                        {
                            { "contract-key", contractKey },
                            { "owner", owner },
                            { "token", token.ToString() },
                            { "offset", offset.ToString() },
                            { "owner", limit.ToString() },
                        }
                    );

                    if (result.Code == ResultCode.Ok)
                    {
                        return new Result<BigInteger, string>
                        {
                            Element = ParseAmount(result.Element.Amount),
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
