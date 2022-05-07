//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Framework - Static and Dynamic : Script File.cs
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
namespace FLORENCE_Networking_Framework
{
    public class Framework
    {
//	PUBLIC -==============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	CONSTRUCTOR ******************************************************************************************************************************************
// 	******************************************************************************************************************************************************
        public Framework()
        {
            this.library_Dynamic_Server_Networking = new Dynamic_Server_Networking();
            while(this.library_Dynamic_Server_Networking == null) { }
            
            this.library_Dynamic_Client_Networking = new Dynamic_Client_Networking();
            while(this.library_Dynamic_Client_Networking == null) { }
        }


//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------
        public Dynamic_Server_Networking getDynamicNetworkingBackendFramework()
        {
            return this.library_Dynamic_Server_Networking;
        }
        public Dynamic_Client_Networking getDynamicNetworkingFrontendFramework()
        {
            return this.library_Dynamic_Client_Networking;
        }
//	PROTECTED ============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------



//	PRIVATE ==============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************
        private Dynamic_Server_Networking library_Dynamic_Server_Networking;
        private Dynamic_Client_Networking library_Dynamic_Client_Networking;
        
//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------Framework
          
    }

    public class Dynamic_Server_Networking
    {
//	PUBLIC -==============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	CONSTRUCTOR ******************************************************************************************************************************************
// 	******************************************************************************************************************************************************
        public Dynamic_Server_Networking()
        {
            Valve.Sockets.Library.Initialize();
            this.StartANewServer();
        }
        ~Dynamic_Server_Networking()
        {

        }


        
//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************
        public void StartANewServer()
        {
            // NETWORKING BACKEND
            this.server = new Valve.Sockets.NetworkingSockets();

            uint pollGroup = this.server.CreatePollGroup();

            this.status = (ref Valve.Sockets.StatusInfo info) => {
                switch (info.connectionInfo.state) {
                    case Valve.Sockets.ConnectionState.None:
                        break;

                    case Valve.Sockets.ConnectionState.Connecting:
                        this.server.AcceptConnection(info.connection);
                        this.server.SetConnectionPollGroup(pollGroup, info.connection);
                        break;

                    case Valve.Sockets.ConnectionState.Connected:
                        Console.WriteLine("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                        break;

                    case Valve.Sockets.ConnectionState.ClosedByPeer:
                    case Valve.Sockets.ConnectionState.ProblemDetectedLocally:
                        this.server.CloseConnection(info.connection);
                        Console.WriteLine("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                        break;
                }
            };

            this.utils.SetStatusCallback(this.status);

            this.address = new Valve.Sockets.Address();

            this.address.SetAddress("::0", this.address.port);

            uint listenSocket = this.server.CreateListenSocket(ref this.address);

            #if VALVESOCKETS_SPAN
                this.netMessage = (in Valve.Sockets.NetworkingMessage netMessage) => {
                    Console.WriteLine("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                };
            #else
                const int maxMessages = 20;

                this.netMessages = new Valve.Sockets.NetworkingMessage[maxMessages];
            #endif

            while (!Console.KeyAvailable) {
                this.server.RunCallbacks();

                #if VALVESOCKETS_SPAN
                    this.server.ReceiveMessagesOnPollGroup(pollGroup, this.netMessage, 20);
                #else
                    int netMessagesCount = this.server.ReceiveMessagesOnPollGroup(pollGroup, this.netMessages, maxMessages);
                    
                    this.netMessage = new Valve.Sockets.NetworkingMessage();
                    if (netMessagesCount > 0) {
                        for (int i = 0; i < netMessagesCount; i++) {
                            this.netMessage = this.netMessages[i];

                            Console.WriteLine("Message received from - ID: " + this.netMessage.connection + ", Channel ID: " + this.netMessage.channel + ", Data length: " + this.netMessage.length);

                            this.netMessage.Destroy();
                        }
                    }
                #endif

                Thread.Sleep(15);
            }
            this.server.DestroyPollGroup(pollGroup);
        }

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------
// NETWORKING BACKEND


//	PROTECTED ============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------


//	PRIVATE ==============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************
// NETWORKING BACKEND
    // enum
        private Valve.Sockets.SendFlags sendFlags;
        private Valve.Sockets.IdentityType identityType;
        private Valve.Sockets.ConnectionState connectionState;
        private Valve.Sockets.ConfigurationScope configurationScope;
        private Valve.Sockets.ConfigurationDataType configurationDataType;
        private Valve.Sockets.ConfigurationValue configurationValue;
        private Valve.Sockets.ConfigurationValueResult configurationValueResult;
        private Valve.Sockets.DebugType debugType;
        private Valve.Sockets.Result result;
    // Struct
        private Valve.Sockets.Address address;
        private Valve.Sockets.Configuration configuration;
        private Valve.Sockets.StatusInfo statusInfo;
        private Valve.Sockets.ConnectionInfo connectionInfo;
        private Valve.Sockets.ConnectionStatus connectionStatus;
        private Valve.Sockets.NetworkingIdentity networkingIdentity;
        private Valve.Sockets.NetworkingMessage netMessage;
        private Valve.Sockets.NetworkingMessage[] netMessages;
    // Delegates
        private Valve.Sockets.StatusCallback status;
        private Valve.Sockets.DebugCallback DebugCallback;

    // Instance of Class
        //private Valve.Sockets.ArrayPool arrayPool;//
        private Valve.Sockets.NetworkingSockets server;
        private Valve.Sockets.NetworkingUtils utils;
        //private Valve.Sockets.Extensions extensions;//
        //private Valve.Sockets.Library library;//
        //private Valve.Sockets.Native native;//
//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------
 

    }

    public class Dynamic_Client_Networking
    {
//	PUBLIC -==============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	CONSTRUCTOR ******************************************************************************************************************************************
// 	******************************************************************************************************************************************************
        public Dynamic_Client_Networking()
        {
            Valve.Sockets.Library.Initialize();
            this.StartANewClient();
        }
        ~Dynamic_Client_Networking()
        {

        }


        
//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************
        public void StartANewClient()
        {
            this.client = new Valve.Sockets.NetworkingSockets();

            uint connection = 0;

            this.status = (ref Valve.Sockets.StatusInfo info) => {
                switch (info.connectionInfo.state) {
                    case Valve.Sockets.ConnectionState.None:
                        break;

                    case Valve.Sockets.ConnectionState.Connected:
                        Console.WriteLine("Client connected to server - ID: " + connection);
                        break;

                    case Valve.Sockets.ConnectionState.ClosedByPeer:
                    case Valve.Sockets.ConnectionState.ProblemDetectedLocally:
                        this.client.CloseConnection(connection);
                        Console.WriteLine("Client disconnected from server");
                        break;
                }
            };

            this.utils.SetStatusCallback(status);

            this.address = new Valve.Sockets.Address();

            this.address.SetAddress("::1", this.address.port);

            connection = client.Connect(ref address);

            #if VALVESOCKETS_SPAN
                this.message = (in Valve.Sockets.NetworkingMessage netMessage) => {
                    Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                };
            #else
                const int maxMessages = 20;

                this.netMessages = new Valve.Sockets.NetworkingMessage[maxMessages];
            #endif

            while (!Console.KeyAvailable) {
                this.client.RunCallbacks();

                #if VALVESOCKETS_SPAN
                    this.client.ReceiveMessagesOnConnection(connection, this.message, 20);
                #else
                    int netMessagesCount = client.ReceiveMessagesOnConnection(connection, netMessages, maxMessages);

                    if (netMessagesCount > 0) {
                        for (int i = 0; i < netMessagesCount; i++) {
                            this.netMessage = this.netMessages[i];

                            Console.WriteLine("Message received from server - Channel ID: " + this.netMessage.channel + ", Data length: " + this.netMessage.length);

                            this.netMessage.Destroy();
                        }
                    }
                #endif

                Thread.Sleep(15);
            }
        }        

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------
// NETWORKING BACKEND


//	PROTECTED ============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------


//	PRIVATE ==============================================================================================================================================
//  ======================================================================================================================================================
//  ======================================================================================================================================================
//	CONSTANTS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************


//	REGISTERS ********************************************************************************************************************************************
// 	******************************************************************************************************************************************************
// NETWORKING BACKEND
    // enum
        private Valve.Sockets.SendFlags sendFlags;
        private Valve.Sockets.IdentityType identityType;
        private Valve.Sockets.ConnectionState connectionState;
        private Valve.Sockets.ConfigurationScope configurationScope;
        private Valve.Sockets.ConfigurationDataType configurationDataType;
        private Valve.Sockets.ConfigurationValue configurationValue;
        private Valve.Sockets.ConfigurationValueResult configurationValueResult;
        private Valve.Sockets.DebugType debugType;
        private Valve.Sockets.Result result;
    // Struct
        private Valve.Sockets.Address address;
        private Valve.Sockets.Configuration configuration;
        private Valve.Sockets.StatusInfo statusInfo;
        private Valve.Sockets.ConnectionInfo connectionInfo;
        private Valve.Sockets.ConnectionStatus connectionStatus;
        private Valve.Sockets.NetworkingIdentity networkingIdentity;
        private Valve.Sockets.NetworkingMessage netMessage;
        private Valve.Sockets.NetworkingMessage[] netMessages;
    // Delegates
        private Valve.Sockets.StatusCallback status;
        private Valve.Sockets.DebugCallback DebugCallback;

        #if VALVESOCKETS_SPAN
		    private Valve.Sockets.MessageCallback message;
        #endif
    // Instance of Class
        //private Valve.Sockets.ArrayPool arrayPool;//
        private Valve.Sockets.NetworkingSockets client;
     
        private Valve.Sockets.NetworkingUtils utils;
        //private Valve.Sockets.Extensions extensions;//
        //private Valve.Sockets.Library library;//
        //private Valve.Sockets.Native native;//
//	METHODS **********************************************************************************************************************************************
// 	******************************************************************************************************************************************************

//	GET & SET --------------------------------------------------------------------------------------------------------------------------------------------
 

    }
}
//End <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
