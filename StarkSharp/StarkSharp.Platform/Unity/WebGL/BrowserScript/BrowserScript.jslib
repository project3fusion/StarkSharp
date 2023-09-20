mergeInto(LibraryManager.library, {
    ConnectWallet: async function(walletTypeStr, id, callbackObjectNameStr, callbackMethodNameStr) {
        
        function CheckConnection(walletType, id, callbackObjectName, callbackMethodName) {
            
            function GetWallet(type) {
                const valueMap = {
                    'ArgentX': window.starknet_argentX,
                    'Braavos': window.starknet_braavos,
                };
                return valueMap[type] || null;
            }
            
            if(!window.starknet) {
                SendErrorMessage(callbackObjectName, callbackMethodName, id, "There is no wallet at the browser");
                return false;
            }

            const wallet = GetWallet(walletType);

            if(!wallet) {
                SendErrorMessage(callbackObjectName, callbackMethodName, id, "There is no wallet you specified at the browser");
                return false;
            }

            return true;
        }

        function GetWallet(type) {

            const valueMap = {
                'ArgentX': window.starknet_argentX,
                'Braavos': window.starknet_braavos,
            };

            return valueMap[type] || null;
        }

        function SendErrorMessage(callbackObjectName, callbackMethodName, id, errorMessage) {
            myGameInstance.SendMessage(callbackObjectName, callbackMethodName, id + ":" + 2 + ":" + errorMessage);
        }

        function SendSuccessMessage(callbackObjectName, callbackMethodName, id, data) {
            myGameInstance.SendMessage(callbackObjectName, callbackMethodName, id + ":" + 1 + ":" + data);
        }

        const walletType = UTF8ToString(walletTypeStr);
        const callbackObjectName = UTF8ToString(callbackObjectNameStr);
        const callbackMethodName = UTF8ToString(callbackMethodNameStr);

        if(CheckConnection(walletType, id, callbackObjectName, callbackMethodName)) {
            const wallet = GetWallet(walletType);
            await wallet.enable();
        }

        SendSuccessMessage(callbackObjectName, callbackMethodName, id, walletType);
    },

    SendTransaction: async function(walletTypeStr, id, contractAddressStr, entryPointStr, callDataStr, callbackObjectNameStr, callbackMethodNameStr) {
        
        function CheckConnection(walletType, id, callbackObjectName, callbackMethodName) {
            
            function GetWallet(type) {
                const valueMap = {
                    'ArgentX': window.starknet_argentX,
                    'Braavos': window.starknet_braavos,
                };
                return valueMap[type] || null;
            }
            
            if(!window.starknet) {
                SendErrorMessage(callbackObjectName, callbackMethodName, id, "There is no wallet at the browser");
                return false;
            }

            const wallet = GetWallet(walletType);

            if(!wallet) {
                SendErrorMessage(callbackObjectName, callbackMethodName, id, "There is no wallet you specified at the browser");
                return false;
            }

            return true;
        }

        function GetWallet(type) {

            const valueMap = {
                'ArgentX': window.starknet_argentX,
                'Braavos': window.starknet_braavos,
            };

            return valueMap[type] || null;
        }

        function SendErrorMessage(callbackObjectName, callbackMethodName, id, errorMessage) {
            myGameInstance.SendMessage(callbackObjectName, callbackMethodName, id + ":" + 2 + ":" + errorMessage);
        }

        function SendSuccessMessage(callbackObjectName, callbackMethodName, id, data) {
            myGameInstance.SendMessage(callbackObjectName, callbackMethodName, id + ":" + 1 + ":" + data);
        }

        const walletType = UTF8ToString(walletTypeStr);
        const callbackObjectName = UTF8ToString(callbackObjectNameStr);
        const callbackMethodName = UTF8ToString(callbackMethodNameStr);

        const contractAddress = UTF8ToString(contractAddressStr);
        const entryPoint = UTF8ToString(entryPointStr);
        const callData = JSON.parse(UTF8ToString(callDataStr)).callData;

        if(CheckConnection(walletType, id, callbackObjectName, callbackMethodName)) {
            const wallet = GetWallet(walletType);
            await wallet.enable();
    
            if (wallet.selectedAddress) {
                wallet.account.execute([{
                    contractAddress: contractAddress,
                    entrypoint: entryPoint,
                    calldata: callData
                }]).then((response) => {
                    SendSuccessMessage(callbackObjectName, callbackMethodName, id, response.transaction_hash);
                }).catch((error) => { 
                    SendErrorMessage(callbackObjectName, callbackMethodName, id, error.message);
                });
            }
        }
    },

    GetAccountInformation: function() {
        const addressBufferSize = lengthBytesUTF8(window.starknet.account.address) + 1;
        var addressBuffer = _malloc(addressBufferSize);
        stringToUTF8(window.starknet.account.address, addressBuffer, addressBufferSize);
        return addressBuffer;
    },

    CheckWalletConnection: function() {
        return window.starknet && window.starknet.isConnected;
    },

    DebugMessage: function(messageStr) {
        console.log(UTF8ToString(messageStr));
    }
});