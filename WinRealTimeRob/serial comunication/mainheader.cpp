#include <iostream>
#include <windows.h>
#include <psapi.h>
#include <processthreadsapi.h>
#include <tchar.h>
#include <winbase.h>
#include <TlHelp32.h>
#include "mainheader.h"


//process id of the process you want to change the priority of
int ProcessID;
wchar_t ProcessName[MAX_PATH]; //process name of the process you want to change the priority of

//variables
DWORD dwError, dwPriClass;
DWORD_PTR Mask = 0x000000000000008; // 0xE (1110) for the last 3 cores reserved for real time
DWORD_PTR processAffinityMask;
DWORD_PTR systemAffinityMask;
HANDLE hProcess, hThreadSnap = INVALID_HANDLE_VALUE, hThread;
THREADENTRY32 te32;

//Gets the process id + error handling
int Get_Process_Handle(DWORD ProcessAccessInfo, DWORD processID) {
	//asking for process handle and information
	hProcess = OpenProcess(ProcessAccessInfo, FALSE, processID);
	if (hProcess == NULL) {
		_tprintf(TEXT("= OpenProcess failed (%d)\n"), GetLastError());
		return ERROR_OCCURRED;
	}
	else {
		processID = GetProcessId(hProcess);
		_tprintf(TEXT("- Current process ID is %lu\n"), processID);
		_tprintf(TEXT("- Process handle is %p\n"), hProcess);
		return SUCCESS;
	}
}

//sets the priority of the process to the desiered class
int Set_Process_Priority(HANDLE process_handle, DWORD priorityclass) {

	
	if (!SetPriorityClass(process_handle, priorityclass)) {
		_tprintf(TEXT("= Failed to set priority class (%d)\n"), GetLastError());
		return ERROR_OCCURRED;
	}
	else if (GetPriorityClass(process_handle) == priorityclass) {
		_tprintf(TEXT("- Priority class is set to 0x%x\n"), priorityclass);
		return SUCCESS;
	}
	/*else {
		_tprintf(TEXT("- Priority class is not set to 0x%x\n"), priorityclass);
		return ERROR_OCCURRED;
	}*/
}

// set affinity mask (on how many cores do you want the procss to run)
int Set_Process_Affinity(HANDLE process_handle, DWORD_PTR dwProcessAffinityMask) {

	if (!SetProcessAffinityMask(process_handle, dwProcessAffinityMask)) {
		dwError = GetLastError();
		if (ERROR_INVALID_PARAMETER == dwError) { // ERROR_INVALID_PARAMETER for a not configured peocessor
			//_tprintf(TEXT("mask is cheanged succesfully!"));
			_tprintf(TEXT("= ERROR_INVALID Processor configuerd\n"));
			return ERROR_OCCURRED;
		}
		else {
			_tprintf(TEXT("= mask did'nt change :("));
			return ERROR_OCCURRED;
			
		}
	}
	else {
		if (!GetProcessAffinityMask(process_handle, &processAffinityMask, &systemAffinityMask))
		{
			_tprintf(TEXT("= Failed to get process affinity mask (%d)\n"), GetLastError());
			
		}
		_tprintf(TEXT("- Current process affinity Mask is %p\n"), processAffinityMask);
		return SUCCESS;
	}
}

// get the threads snapshot
int Get_Process_Snapshot(DWORD processID) {

	hThreadSnap = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, processID); //get the thread snapshot
	if (hThreadSnap == INVALID_HANDLE_VALUE) {
		dwError = GetLastError();
		_tprintf(TEXT("= CreateToolhelpSnapshot (of Threads) failed with %d\n", dwError));
		return ERROR_OCCURRED;
	}
	else {
		_tprintf(TEXT("- CreateToolhelpSnapshot (of Threads) is successful\n"));
		return SUCCESS;
	}

}

// sets the priority of the thread to the desiered class
int Set_Threads_Priority(HANDLE thread_snap, int Priority) {

	te32.dwSize = sizeof(THREADENTRY32); //set the size of the structure
	if (!Thread32First(thread_snap, &te32)) { //get the first thread
		_tprintf(TEXT("= Thread32First failed with %d\n"), GetLastError()); 
		CloseHandle(thread_snap); 
		CloseHandle(hProcess);
		return ERROR_OCCURRED;
	}
	else {
		_tprintf(TEXT("- Thread32First is successful\n"));
		do {
			if (te32.th32OwnerProcessID == ProcessID) { //check if the thread belongs to the process
				hThread = OpenThread(THREAD_ALL_ACCESS, FALSE, te32.th32ThreadID); //open the thread and get the handle
				_tprintf(TEXT("  1   Thread handle   = %p\n"), hThread);
				SetThreadPriority(hThread, Priority);//sets the priority of the thread to the desiered class
				//SetThreadAffinityMask(hThread, Mask); // set affinity mask (on how many cores do you want the procss to run)
				_tprintf(TEXT("  2   THREAD ID      = %lu\n"), te32.th32ThreadID);
				_tprintf(TEXT("  3   Base priority  = %d\n"), te32.tpBasePri);
				_tprintf(TEXT("  4   Delta priority = %d\n"), te32.tpDeltaPri);
				//GetProcessAffinityMask(hThread, &processAffinityMask, &systemAffinityMask);
				//_tprintf(TEXT("     Thread affinity mask = %p\n"), (void*)processAffinityMask);
				_tprintf(TEXT("  5   Thread priority = %d\n"), GetThreadPriority(hThread));
				_tprintf(TEXT("\n"));
			}
		} while (Thread32Next(hThreadSnap, &te32)); //get the next thread
		return SUCCESS;
	}
}

//setting thread affinity should be avoided as it can cause the system to hang and cause shedualer to not work properly
// use this function with caution
int Set_Threads_Affinity(HANDLE thread_snap, DWORD_PTR dwThreadAffinityMask) {

	te32.dwSize = sizeof(THREADENTRY32); //set the size of the structure
	if (!Thread32First(thread_snap, &te32)) { //get the first thread
		_tprintf(TEXT("= Thread32First failed with %d\n"), GetLastError());

		return ERROR_OCCURRED;
	}
	else {
		_tprintf(TEXT("- Thread32First is successful\n"));
		do {
			if (te32.th32OwnerProcessID == ProcessID) { //check if the thread belongs to the process
				hThread = OpenThread(THREAD_ALL_ACCESS, FALSE, te32.th32ThreadID); //open the thread and get the handle
				_tprintf(TEXT("  1   Thread handle   =  %p\n"), hThread);
				SetThreadAffinityMask(hThread, dwThreadAffinityMask); // set affinity mask (on how many cores do you want the procss to run)
				_tprintf(TEXT("  2   THREAD ID       =  %lu\n"), te32.th32ThreadID);
				_tprintf(TEXT("  3   Base priority   =  %d\n"), te32.tpBasePri);
				_tprintf(TEXT("  4   Delta priority  =  %d\n"), te32.tpDeltaPri);
				_tprintf(TEXT("  5   Thread affinity =  %p\n"), Mask);
				_tprintf(TEXT("  6   Thread priority =  %d\n"), GetThreadPriority(hThread));
				_tprintf(TEXT("\n"));
			}
		} while (Thread32Next(hThreadSnap, &te32)); //get the next thread
		return SUCCESS;
	}
}