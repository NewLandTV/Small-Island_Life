/*
 * Invalid Command : 65535
 * C2S = Client to Server
 * S2C = Server to Client
 */
public static class Command
{
    public const ushort INVALID = 65535;

    // Client to Server
    public const ushort CMD_C2S_LOGIN = 0;
    public const ushort CMD_C2S_REGIST = 1;

    // Server to Client
    public const ushort CMD_S2C_LOGIN_SUCCESS = 0;
    public const ushort CMD_S2C_LOGIN_FAILED = 1;
    public const ushort CMD_S2C_REGIST_SUCCESS = 2;
    public const ushort CMD_S2C_REGIST_FAILED = 3;
}
