#include <iostream>
#include <process.h>
#include "Server.h"
#include "../Command.h"

unsigned int CALLBACK CallWorkerThread(LPVOID p)
{
	Server* overlappedEvent = (Server*)p;

	overlappedEvent->WorkerThread();

	return 0;
}

Server::Server() : socketInfo(nullptr), listeningSocket(0), iocpHandle(nullptr), workerHandle(nullptr), mySQL(), accept(true), runningWorkerThread(true)
{

}

Server::~Server()
{
	WSACleanup();

	if (socketInfo != nullptr)
	{
		delete[] socketInfo;

		socketInfo = nullptr;
	}

	if (workerHandle != nullptr)
	{
		delete[] workerHandle;

		workerHandle = nullptr;
	}
}

bool Server::Initialize(unsigned short port)
{
	// Initialize winsock v2.2
	WSADATA wsaData;

	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
	{
		return false;
	}

	// Socket initialize
	listeningSocket = WSASocket(AF_INET, SOCK_STREAM, 0, nullptr, 0, WSA_FLAG_OVERLAPPED);

	if (listeningSocket == INVALID_SOCKET)
	{
		return false;
	}

	// Server info settings
	SOCKADDR_IN serverAddrIn;

	serverAddrIn.sin_family = PF_INET;
	serverAddrIn.sin_port = htons(port);
	serverAddrIn.sin_addr.S_un.S_addr = htonl(INADDR_ANY);

	// Socket settings
	if (bind(listeningSocket, (SOCKADDR*)&serverAddrIn, sizeof(SOCKADDR_IN)) == SOCKET_ERROR)
	{
		closesocket(listeningSocket);

		return false;
	}

	// Listening socket settings
	if (listen(listeningSocket, 5) == SOCKET_ERROR)
	{
		closesocket(listeningSocket);

		return false;
	}

	return true;
}

void Server::Start()
{
	// Client info
	SOCKET clientSocket;
	SOCKADDR_IN clientAddrIn;
	int clientAddrInLength = sizeof(SOCKADDR_IN);
	DWORD recvBytes;
	DWORD flags;

	// Create ompletion port
	iocpHandle = CreateIoCompletionPort(INVALID_HANDLE_VALUE, nullptr, 0, 0);

	// Create worker thread
	if (!CreateWorkerThread())
	{
		return;
	}

	while (accept)
	{
		clientSocket = WSAAccept(listeningSocket, (SOCKADDR*)&clientAddrIn, &clientAddrInLength, nullptr, 0);

		if (clientSocket == INVALID_SOCKET)
		{
			return;
		}

		socketInfo = new SocketInfo();
		socketInfo->dataBuffer.len = MAX_BUFFER_SIZE;
		socketInfo->dataBuffer.buf = socketInfo->messageBuffer;
		socketInfo->socket = clientSocket;
		socketInfo->recvBytes = 0;
		socketInfo->sendBytes = 0;

		flags = 0;

		// Specifies a nested socket and passes a function to be executed upon completion
		iocpHandle = CreateIoCompletionPort((HANDLE)clientSocket, iocpHandle, (DWORD)socketInfo, 0);

		if (WSARecv(socketInfo->socket, &socketInfo->dataBuffer, 1, &recvBytes, &flags, &(socketInfo->overlapped), nullptr) == SOCKET_ERROR && WSAGetLastError() != WSA_IO_PENDING)
		{
			return;
		}
	}
}

bool Server::CreateWorkerThread()
{
	unsigned int threadId;

	SYSTEM_INFO systemInfo;

	GetSystemInfo(&systemInfo);

	unsigned int threadCount = systemInfo.dwNumberOfProcessors * 2;

	workerHandle = new HANDLE[threadCount];

	for (unsigned int i = 0; i < threadCount; i++)
	{
		workerHandle[i] = (HANDLE*)_beginthreadex(nullptr, 0, &CallWorkerThread, this, CREATE_SUSPENDED, &threadId);

		if (workerHandle[i] == nullptr)
		{
			return false;
		}

		ResumeThread(workerHandle[i]);
	}

	return true;
}

void Server::WorkerThread()
{
	// Overlapped I/O send & recv size
	DWORD recvBytes;
	DWORD sendBytes;
	DWORD flags = 0;

	SocketInfo* completionKey;
	SocketInfo* socketInfo;

	while (runningWorkerThread)
	{
		if (!GetQueuedCompletionStatus(iocpHandle, &recvBytes, (PULONG_PTR)&completionKey, (LPOVERLAPPED*)&socketInfo, INFINITE) && recvBytes == 0)
		{
			closesocket(socketInfo->socket);
			free(socketInfo);

			continue;
		}

		socketInfo->dataBuffer.len = recvBytes;

		if (recvBytes == 0)
		{
			closesocket(socketInfo->socket);
			free(socketInfo);

			continue;
		}

		// Process server side logic
		char buffer[MAX_BUFFER_SIZE];
		char* context = nullptr;
		char* ptr = strtok_s(socketInfo->dataBuffer.buf, "|", &context);

		switch (atoi(ptr))
		{
		case CMD_C2S_LOGIN:
		{
			char id[100];
			char password[500];
			char result[13];

			strcpy_s(id, 100, strtok_s(nullptr, "|", &context));
			strcpy_s(password, 500, strtok_s(nullptr, "|", &context));

			int uid = mySQL.GetAccountUID(id, password);

			if (uid != 0)
			{
				sprintf_s(result, "%d|%u", CMD_S2C_LOGIN_SUCCESS, uid);
			}
			else
			{
				sprintf_s(result, "%d|", CMD_S2C_LOGIN_FAILED);
			}

			strcpy_s(buffer, MAX_BUFFER_SIZE, result);

			break;
		}
		case CMD_C2S_REGIST:
		{
			char id[100];
			char password[500];
			char result[3];

			strcpy_s(id, 100, strtok_s(nullptr, "|", &context));
			strcpy_s(password, 500, strtok_s(nullptr, "|", &context));

			if (mySQL.CreateAccount(id, password))
			{
				sprintf_s(result, "%d|", CMD_S2C_REGIST_SUCCESS);
			}
			else
			{
				sprintf_s(result, "%d|", CMD_S2C_REGIST_FAILED);
			}

			strcpy_s(buffer, MAX_BUFFER_SIZE, result);

			break;
		}
		default:
			ZeroMemory(buffer, MAX_BUFFER_SIZE);

			break;
		}

		if (buffer != nullptr && strlen(buffer) > 0)
		{
			socketInfo->dataBuffer.len = strlen(buffer);
			socketInfo->dataBuffer.buf = buffer;

			// Send data to client
			if (WSASend(socketInfo->socket, &(socketInfo->dataBuffer), 1, &sendBytes, flags, nullptr, nullptr) == SOCKET_ERROR && WSAGetLastError() != WSA_IO_PENDING)
			{

			}
		}

		// Initialize socket info
		ZeroMemory(&(socketInfo->overlapped), sizeof(OVERLAPPED));

		socketInfo->dataBuffer.len = MAX_BUFFER_SIZE;
		socketInfo->dataBuffer.buf = socketInfo->messageBuffer;

		ZeroMemory(socketInfo->messageBuffer, MAX_BUFFER_SIZE);

		socketInfo->recvBytes = 0;
		socketInfo->sendBytes = 0;
		flags = 0;

		if (WSARecv(socketInfo->socket, &(socketInfo->dataBuffer), 1, &recvBytes, &flags, (LPWSAOVERLAPPED) & (socketInfo->overlapped), nullptr) == SOCKET_ERROR && WSAGetLastError() != WSA_IO_PENDING)
		{

		}
	}
}