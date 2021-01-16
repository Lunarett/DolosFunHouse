using Photon.Realtime;

public class ErrorMsg
{
	public static ErrorMsg Instance;

	public string DisconnectCauses(DisconnectCause cause)
	{
		switch (cause)
		{
			case DisconnectCause.None:
				return "None";
				break;
			case DisconnectCause.ExceptionOnConnect:
				return "The server is not available or the address is wrong.";
				break;
			case DisconnectCause.Exception:
				return "Some internal exception caused the socket code to fail.";
				break;
			case DisconnectCause.ServerTimeout:
				return "The server disconnected this client due to timing out.";
				break;
			case DisconnectCause.ClientTimeout:
				return "This client detected that the server's responses are not received in due time.";
				break;
			case DisconnectCause.DisconnectByServerLogic:
				return "The server disconnected this client from within the room's logic";
				break;
			case DisconnectCause.DisconnectByServerReasonUnknown:
				return "The server disconnected this client for unknown reasons.";
				break;
			case DisconnectCause.InvalidAuthentication:
				return "Authenticate in the Photon Cloud with invalid AppId.";
				break;
			case DisconnectCause.CustomAuthenticationFailed:
				return "Authenticate in the Photon Cloud with invalid client values or custom authentication setup in Cloud Dashboard.";
				break;
			case DisconnectCause.AuthenticationTicketExpired:
				return "Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst.";
				break;
			case DisconnectCause.MaxCcuReached:
				return "Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst.";
				break;
			case DisconnectCause.InvalidRegion:
				return "Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s).";
				break;
			case DisconnectCause.OperationNotAllowedInCurrentState:
				return "Operation that's (currently) not available for this client (not authorized usually).";
				break;
			case DisconnectCause.DisconnectByClientLogic:
				return "The client disconnected from within the logic";
				break;
			case DisconnectCause.DisconnectByOperationLimit:
				return "The client called an operation too frequently and got disconnected due to hitting the OperationLimit. To protect the server, some operations have a limit.";
				break;
			case DisconnectCause.DisconnectByDisconnectMessage:
				return "The client received a \"Disconnect Message\" from the server.";
				break;
			default:
				return "Neither of them";
				break;
		}
	}

	public string NoConnection()
	{
		return "No internet connection. Check your network settings and try again.";
	}
}
