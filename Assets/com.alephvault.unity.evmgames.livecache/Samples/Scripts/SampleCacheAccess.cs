using System;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.LiveCache.Types.Cache;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using UnityEngine;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Samples
    {
        [RequireComponent(typeof(SampleStateGrabber))]
        public class SampleCacheAccess : MonoBehaviour
        {
            private Root root;
            private SampleStateGrabber grabber;
            private ERC20BalanceCacheHandler erc20handler;
            private ERC721OwnershipCacheHandler erc721handler;
            private ERC1155OwnershipCacheHandler erc1155handler;
            
            private void Awake()
            {
                grabber = GetComponent<SampleStateGrabber>();
            }

            private async Task ERC20BalanceOf()
            {
                Debug.Log("Getting an ERC20 balance");
                Result<BigInteger, string> result = await new ERC20BalanceCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).BalanceOf("erc20-sample", "0xAF950274754d7408B7bAA9358F5CB92D162C5c09");
                Debug.Log("ERC20BalanceOf(0xAF950274754d7408B7bAA9358F5CB92D162C5c09) returned " +
                          $"{result.Code} -> {result.Element}");
            }

            private async Task ERC20Balances()
            {
                Debug.Log("Getting a bunch of ERC20 balances");
                Result<Tuple<string, BigInteger>[], string> result = await new ERC20BalanceCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).Balances("erc20-sample", 0, 40);
                Debug.Log($"ERC20Balances() returned {result.Code} -> {result.Element.Length} elements");
                foreach (Tuple<string, BigInteger> element in result.Element)
                {
                    Debug.Log($"Balance: {element.Item1} -> {element.Item2}");
                }
            }

            private async Task ERC20Reset()
            {
                Debug.Log("Cache is about to be reset for ERC20");
                await new ERC20BalanceCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).Reset("erc20-sample");
                Debug.Log("Cache was reset");
            }

            private async Task ERC721CollectionOf()
            {
                Debug.Log("Getting an ERC721 collection");
                Result<HexBigInteger[], string> result = await new ERC721OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).CollectionOf(
                    "erc721-sample", "0xAF950274754d7408B7bAA9358F5CB92D162C5c09", 
                    0, 40
                );
                Debug.Log($"ERC721CollectionOf() returned {result.Code} -> {result.Element.Length} elements");
                Debug.Log("ERCBalanceOf(0xAF950274754d7408B7bAA9358F5CB92D162C5c09) returned " +
                          $"{result.Code} -> {result.Element}");
                foreach (HexBigInteger element in result.Element)
                {
                    Debug.Log($"Owned: {element}");
                }
            }

            private async Task ERC721Collections()
            {
                Debug.Log("Getting all the ERC721 collections");
                Result<Tuple<string, HexBigInteger>[], string> result = await new ERC721OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).Collections("erc721-sample", 0, 40);
                Debug.Log($"ERC721Collections() returned {result.Code} -> {result.Element?.Length} elements");
                Debug.Log("ERCBalanceOf(0xAF950274754d7408B7bAA9358F5CB92D162C5c09) returned " +
                          $"{result.Code} -> {result.Element}");
                foreach (Tuple<string, HexBigInteger> element in result.Element)
                {
                    Debug.Log($"Ownership: {element.Item2} -> {element.Item1}");
                }
            }

            private async Task ERC721Reset()
            {
                Debug.Log("Cache is about to be reset");
                await new ERC721OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).Reset("erc721-sample");
                Debug.Log("Cache was reset");
            }

            private async Task ERC1155BalanceOf()
            {
                Debug.Log("Getting an ERC1155 balance");
                Result<BigInteger, string> result = await new ERC1155OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).BalanceOf(
                    "erc1155-sample", "0xAF950274754d7408B7bAA9358F5CB92D162C5c09", 
                    new HexBigInteger("0x111")
                );
                Debug.Log("ERC1155BalanceOf(0xAF950274754d7408B7bAA9358F5CB92D162C5c09) returned " +
                          $"{result.Code} -> {result.Element}");
            }

            private async Task ERC1155BalancesOf()
            {
                Debug.Log("Getting the ERC1155 balances of an owner");
                Result<Tuple<HexBigInteger, BigInteger>[], string> result = await new ERC1155OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).BalancesOf(
                    "erc1155-sample", "0xAF950274754d7408B7bAA9358F5CB92D162C5c09", 
                    0, 40
                );
                Debug.Log("ERC1155BalanceOf(0xAF950274754d7408B7bAA9358F5CB92D162C5c09) returned " +
                          $"{result.Code} -> {result.Element}");
                foreach (Tuple<HexBigInteger, BigInteger> element in result.Element)
                {
                    Debug.Log($"Balance: {element.Item1} -> {element.Item2}");
                }
            }

            private async Task ERC1155Balances()
            {
                Debug.Log("Getting the ERC1155 balances of an owner");
                Result<Tuple<string, HexBigInteger, BigInteger>[], string> result = await new ERC1155OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).Balances("erc1155-sample", 0, 40);
                Debug.Log("ERC1155BalanceOf(0xAF950274754d7408B7bAA9358F5CB92D162C5c09) returned " +
                          $"{result.Code} -> {result.Element}");
                foreach (Tuple<string, HexBigInteger, BigInteger> element in result.Element)
                {
                    Debug.Log($"Balance: {element.Item1}:{element.Item2} -> {element.Item3}");
                }
            }

            private async Task ERC1155Reset()
            {
                Debug.Log("Cache is about to be reset");
                await new ERC1155OwnershipCacheHandler(new Root(
                    grabber.CacheURL, new Authorization("bearer", grabber.ApiKey)
                )).Reset("erc1155-sample");
                Debug.Log("Cache was reset");
            }

            private async void PickCommand()
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    await ERC20BalanceOf();
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    await ERC20Balances();
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    await ERC20Reset();
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    await ERC721CollectionOf();
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    await ERC721Collections();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    await ERC721Reset();
                }
                else if (Input.GetKeyDown(KeyCode.U))
                {
                    await ERC1155BalanceOf();
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    await ERC1155BalancesOf();
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    await ERC1155Balances();
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    await ERC1155Reset();
                }
            }

            private void Update()
            {
                PickCommand();
            }
        }
    }
}
