namespace StarkSharp.Platforms.Godot.WebGL{
	public class BrowserScript
	{
		public static string ConnectWallet(string walletType, int id) {
			return $@"(async function(walletType, id) {{
				function CheckConnection(walletType, id) {{
					function GetWallet(type) {{
						const valueMap = {{
							'ArgentX': window.starknet_argentX,
							'Braavos': window.starknet_braavos,
						}};
						return valueMap[type] || null;
					}}
					
					if(!window.starknet) {{
						SendErrorMessage(id, ""There is no wallet at the browser"");
						return false;
					}}

					const wallet = GetWallet(walletType);

					if(!wallet) {{
						SendErrorMessage(id, ""There is no wallet you specified at the browser"");
						return false;
					}}

					return true;
				}}

				function GetWallet(type) {{

					const valueMap = {{
						'ArgentX': window.starknet_argentX,
						'Braavos': window.starknet_braavos,
					}};

					return valueMap[type] || null;
				}}

				function SendErrorMessage(id, errorMessage) {{
					const message = id + "":"" + 2 + "":"" + errorMessage;
					messageDictionary[id] = message;
				}}

				function SendSuccessMessage(id, data) {{
					const message = id + "":"" + 1 + "":"" + data;
					messageDictionary[id] = message;
				}}

				if(CheckConnection(walletType, id)) {{
					const wallet = GetWallet(walletType);
					await wallet.enable();
				}}

				SendSuccessMessage(id, walletType);
			}})('{walletType}', {id});";
		}

		public static string SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData) {
			return $@"(async function(walletType, id, contractAddress, entryPoint, callData) {{
				function CheckConnection(walletType, id) {{
					
					function GetWallet(type) {{
						const valueMap = {{
							'ArgentX': window.starknet_argentX,
							'Braavos': window.starknet_braavos,
						}};
						return valueMap[type] || null;
					}}
					
					if(!window.starknet) {{
						SendErrorMessage(id, ""There is no wallet at the browser"");
						return false;
					}}

					const wallet = GetWallet(walletType);

					if(!wallet) {{
						SendErrorMessage(id, ""There is no wallet you specified at the browser"");
						return false;
					}}

					return true;
				}}

				function GetWallet(type) {{

					const valueMap = {{
						'ArgentX': window.starknet_argentX,
						'Braavos': window.starknet_braavos,
					}};

					return valueMap[type] || null;
				}}

				function SendErrorMessage(id, errorMessage) {{
					const message = id + "":"" + 2 + "":"" + errorMessage;
					messageDictionary[id] = message;
				}}

				function SendSuccessMessage(id, data) {{
					const message = id + "":"" + 1 + "":"" + data;
					messageDictionary[id] = message;
				}}

				const callDataJson = JSON.parse(callData).callData;

				if(CheckConnection(walletType, id)) {{
					const wallet = GetWallet(walletType);
					await wallet.enable();
			
					if (wallet.selectedAddress) {{
						wallet.account.execute([{{
							contractAddress: contractAddress,
							entrypoint: entryPoint,
							calldata: callDataJson
						}}]).then((response) => {{
							SendSuccessMessage(id, response.transaction_hash);
						}}).catch((error) => {{
							SendErrorMessage(id, error.message);
						}});
					}}
				}}
			}})('{walletType}', {id}, '{contractAddress}', '{entryPoint}', '{callData}');";
		}

		public static string GetAccountInformation()
		{
			return $@"(function() {{
				return window.starknet.account.address;
			}})();";
		}

		public static string CheckWalletConnection()
		{
			return $@"(function() {{
				return window.starknet && window.starknet.isConnected;
			}})();";
		}

		public static string CheckMessage(int id) 
		{
			return $@"(function(id) {{
				if(messageDictionary[id] != null) {{
					let message = messageDictionary[id];
					return true;
				}}
				else return false;
			}})({id});";
		}

		public static string GetMessage(int id) 
		{
			return $@"(function(id) {{
				if(messageDictionary[id] != null) {{
					let message = messageDictionary[id];
					messageDictionary[id] = null;
					return message;
					}}
				else return """";
			}})({id});";
		}

		public static string DebugMessage(string message) 
		{
			return $@"console.log(""{message}"");";
		}

		public static string SetGlobalDictionary()
		{
			return "var messageDictionary = {}";
		}
	}
}
