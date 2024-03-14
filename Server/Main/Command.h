#ifndef __COMMAND_H__
#define __COMMAND_H__

/*
 * Invalid Command : 65535
 * C2S = Client to Server
 * S2C = Server to Client
 */

#define INVALID_CMD 65535

// Client to Server
#define CMD_C2S_LOGIN 0
#define CMD_C2S_REGIST 1

// Server to Client
#define CMD_S2C_LOGIN_SUCCESS 0
#define CMD_S2C_LOGIN_FAILED 1
#define CMD_S2C_REGIST_SUCCESS 2
#define CMD_S2C_REGIST_FAILED 3

#endif